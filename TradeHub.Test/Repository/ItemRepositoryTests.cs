using Microsoft.EntityFrameworkCore;
using TradeHub.Api.Models;
using TradeHub.Api.Repository;
using TradeHub.Api.Utilities;
using TradeHub.Test.Helpers;

namespace TradeHub.Test.Repository;

public class ItemRepositoryTests
{
    private readonly DbContextOptions<TradeHubContext> _dbContextOptions;

    public ItemRepositoryTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<TradeHubContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .AddInterceptors(new RowVersionInterceptor())
            .Options;
    }

    private static async Task<User> SeedUserAsync(TradeHubContext context)
    {
        var user = new User
        {
            Username = "testuser",
            Email = "test@test.com",
            PasswordHash = "password",
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    #region GetAllAsync

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoItemsExist()
    {
        // Arrange
        await using var context = new TradeHubContext(_dbContextOptions);
        var repository = new ItemRepository(context);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllItems_WhenItemsExist()
    {
        // Arrange
        await using var context = new TradeHubContext(_dbContextOptions);
        var user = await SeedUserAsync(context);
        var item1 = new Item(
            "Item 1",
            "Desc 1",
            "",
            10,
            user.Id,
            "",
            Condition.New,
            Availability.Available
        );
        var item2 = new Item(
            "Item 2",
            "Desc 2",
            "",
            20,
            user.Id,
            "",
            Condition.UsedGood,
            Availability.Available
        );
        context.Items.AddRange(item1, item2);
        await context.SaveChangesAsync();
        var repository = new ItemRepository(context);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count);
    }

    #endregion

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenItemDoesNotExist()
    {
        // Arrange
        await using var context = new TradeHubContext(_dbContextOptions);
        var repository = new ItemRepository(context);

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnItem_WhenItemExists()
    {
        // Arrange
        await using var context = new TradeHubContext(_dbContextOptions);
        var user = await SeedUserAsync(context);
        var item = new Item(
            "Test Item",
            "Desc",
            "",
            10,
            user.Id,
            "",
            Condition.New,
            Availability.Available
        );
        context.Items.Add(item);
        await context.SaveChangesAsync();
        var repository = new ItemRepository(context);

        // Act
        var result = await repository.GetByIdAsync(item.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(item.Id, result.Id);
        Assert.Equal("Test Item", result.Name);
    }

    #endregion

    #region CreateAsync

    [Fact]
    public async Task CreateAsync_ShouldAddItemToDatabase()
    {
        // Arrange
        await using var context = new TradeHubContext(_dbContextOptions);
        var user = await SeedUserAsync(context);
        var repository = new ItemRepository(context);
        var newItem = new Item(
            "New Item",
            "Desc",
            "",
            50,
            user.Id,
            "",
            Condition.New,
            Availability.Available
        );

        // Act
        var createdItem = await repository.CreateAsync(newItem);

        // Assert
        Assert.NotEqual(0, createdItem.Id);
        await using var assertContext = new TradeHubContext(_dbContextOptions);
        var itemInDb = await assertContext.Items.FindAsync(createdItem.Id);
        Assert.NotNull(itemInDb);
        Assert.Equal("New Item", itemInDb.Name);
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_ShouldUpdateItemInDatabase()
    {
        // Arrange
        await using var context = new TradeHubContext(_dbContextOptions);
        var user = await SeedUserAsync(context);
        var item = new Item(
            "Original Name",
            "Desc",
            "",
            10,
            user.Id,
            "",
            Condition.New,
            Availability.Available
        );
        context.Items.Add(item);
        await context.SaveChangesAsync();
        context.Entry(item).State = EntityState.Detached; // Detach to simulate a new request

        var repository = new ItemRepository(context);
        item.Name = "Updated Name";

        // Act
        var result = await repository.UpdateAsync(item);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);
        Assert.NotEqual(item.RowVersion, result.RowVersion); // The RowVersion should have been updated

        await using var assertContext = new TradeHubContext(_dbContextOptions);
        var updatedItemInDb = await assertContext.Items.FindAsync(item.Id);
        Assert.NotNull(updatedItemInDb);
        Assert.Equal("Updated Name", updatedItemInDb.Name);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnItem_WhenNoChangesAreMade()
    {
        // Arrange
        await using var context = new TradeHubContext(_dbContextOptions);
        var user = await SeedUserAsync(context);
        var item = new Item(
            "Original Name",
            "Desc",
            "",
            10,
            user.Id,
            "",
            Condition.New,
            Availability.Available
        );
        context.Items.Add(item);
        await context.SaveChangesAsync();

        var repository = new ItemRepository(context);

        // Act
        var result = await repository.UpdateAsync(item);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(item.Id, result.Id);
        Assert.Equal(item.Name, result.Name);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowNotFoundException_WhenItemIsDeletedByAnotherUser()
    {
        // Arrange
        await using var context1 = new TradeHubContext(_dbContextOptions);
        var user = await SeedUserAsync(context1);
        var item = new Item(
            "Original Name",
            "Desc",
            "",
            10,
            user.Id,
            "",
            Condition.New,
            Availability.Available
        );
        context1.Items.Add(item);
        await context1.SaveChangesAsync();
        // Detach the entity to simulate it coming from a different request
        context1.Entry(item).State = EntityState.Detached;

        // Simulate another user deleting the item
        await using var context2 = new TradeHubContext(_dbContextOptions);
        var itemFromDb2 = await context2.Items.FindAsync(item.Id);
        Assert.NotNull(itemFromDb2);
        context2.Items.Remove(itemFromDb2);
        await context2.SaveChangesAsync();

        // Act & Assert: Try to update the now-deleted item using a new context
        item.Name = "My change";
        await using var context3 = new TradeHubContext(_dbContextOptions);
        var repository = new ItemRepository(context3);
        await Assert.ThrowsAsync<NotFoundException>(() => repository.UpdateAsync(item));
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowConflictException_WhenItemIsUpdatedByAnotherUser()
    {
        // Arrange: Seed the initial item
        await using var setupContext = new TradeHubContext(_dbContextOptions);
        var user = await SeedUserAsync(setupContext);
        var initialItem = new Item(
            "Original",
            "Desc",
            "",
            10,
            user.Id,
            "",
            Condition.New,
            Availability.Available
        );
        setupContext.Items.Add(initialItem);
        await setupContext.SaveChangesAsync();

        // Simulate User 1 fetching the item (like a DTO sent to a client)
        await using var context1 = new TradeHubContext(_dbContextOptions);
        var itemForUser1 = await context1
            .Items.AsNoTracking()
            .SingleAsync(i => i.Id == initialItem.Id);

        // Simulate User 2 updating the item. The RowVersionInterceptor will handle the update.
        await using var context2 = new TradeHubContext(_dbContextOptions);
        var itemForUser2 = await context2.Items.SingleAsync(i => i.Id == initialItem.Id);
        itemForUser2.Name = "User 2's Update";
        await context2.SaveChangesAsync();

        // Act & Assert: User 1 attempts to update using their stale data (old RowVersion)
        itemForUser1.Name = "User 1's Conflicting Update";
        await using var context3 = new TradeHubContext(_dbContextOptions);
        var repo3 = new ItemRepository(context3);

        // This call should now fail because itemForUser1 has a stale RowVersion
        await Assert.ThrowsAsync<ConflictException>(() => repo3.UpdateAsync(itemForUser1));
    }

    #endregion

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_ShouldRemoveItemFromDatabase()
    {
        // Arrange
        await using var context = new TradeHubContext(_dbContextOptions);
        var user = await SeedUserAsync(context);
        var item = new Item(
            "To Be Deleted",
            "Desc",
            "",
            10,
            user.Id,
            "",
            Condition.New,
            Availability.Available
        );
        context.Items.Add(item);
        await context.SaveChangesAsync();
        var repository = new ItemRepository(context);

        // Act
        var result = await repository.DeleteAsync(item);

        // Assert
        Assert.True(result);
        await using var assertContext = new TradeHubContext(_dbContextOptions);
        var itemInDb = await assertContext.Items.FindAsync(item.Id);
        Assert.Null(itemInDb);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowException_WhenDeletingSameItemTwice()
    {
        // Arrange
        await using var context = new TradeHubContext(_dbContextOptions);
        var user = await SeedUserAsync(context);
        var item = new Item(
            "To Be Deleted",
            "Desc",
            "",
            10,
            user.Id,
            "",
            Condition.New,
            Availability.Available
        );
        context.Items.Add(item);
        await context.SaveChangesAsync();
        var repository = new ItemRepository(context);

        // Act
        var firstDeleteResult = await repository.DeleteAsync(item);

        // Assert
        Assert.True(firstDeleteResult);
        // After the first delete, the entity is detached. Calling delete again on the same instance
        // will cause EF to re-attach it, mark it as deleted, and then fail on SaveChanges
        // because the row is already gone from the database, causing a concurrency exception.
        await Assert.ThrowsAsync<ConflictException>(() => repository.DeleteAsync(item));
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowConflictException_OnConcurrencyConflict()
    {
        // Arrange
        await using var context1 = new TradeHubContext(_dbContextOptions);
        var user = await SeedUserAsync(context1);
        var item = new Item(
            "Original Name",
            "Desc",
            "",
            10,
            user.Id,
            "",
            Condition.New,
            Availability.Available
        );
        context1.Items.Add(item);
        await context1.SaveChangesAsync();

        var repository1 = new ItemRepository(context1);

        await using var context2 = new TradeHubContext(_dbContextOptions);
        var itemFromDb2 = await context2.Items.FindAsync(item.Id);
        Assert.NotNull(itemFromDb2);
        context2.Items.Remove(itemFromDb2); // Another user deletes the item
        await context2.SaveChangesAsync();

        // Act & Assert
        // Now try to delete the already-deleted item
        await Assert.ThrowsAsync<ConflictException>(() => repository1.DeleteAsync(item));
    }

    #endregion
}
