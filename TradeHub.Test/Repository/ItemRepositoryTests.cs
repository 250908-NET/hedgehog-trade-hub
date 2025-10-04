using Microsoft.EntityFrameworkCore;
using TradeHub.Api.Models;
using TradeHub.Api.Repository;
using TradeHub.Api.Utilities;

namespace TradeHub.Test
{
    public class ItemRepositoryTests
    {
        private readonly DbContextOptions<TradeHubContext> _dbContextOptions;

        public ItemRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<TradeHubContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
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
            Assert.True(result);
            await using var assertContext = new TradeHubContext(_dbContextOptions);
            var updatedItem = await assertContext.Items.FindAsync(item.Id);
            Assert.NotNull(updatedItem);
            Assert.Equal("Updated Name", updatedItem.Name);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowConflictException_OnConcurrencyConflict()
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
            item.Name = "My change"; // Try to update the now-deleted item
            await Assert.ThrowsAsync<ConflictException>(() => repository1.UpdateAsync(item));
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
}
