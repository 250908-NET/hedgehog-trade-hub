using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using TradeHub.Api.Models;
using TradeHub.Api.Models.DTOs;
using TradeHub.Api.Repository.Interfaces;
using TradeHub.Api.Services;

namespace TradeHub.Test.Services;

public class ItemServiceTests
{
    private readonly Mock<IItemRepository> _mockItemRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<ItemService>> _mockLogger;
    private readonly ItemService _itemService;

    public ItemServiceTests()
    {
        _mockItemRepository = new Mock<IItemRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<ItemService>>();

        _itemService = new ItemService(
            _mockItemRepository.Object,
            _mockMapper.Object,
            _mockLogger.Object
        );
    }

    #region GetAllItemsAsync

    [Fact]
    public async Task GetAllItemsAsync_ShouldReturnListOfItemDTOs_WhenItemsExist()
    {
        // Arrange
        var testItems = new List<Item>
        {
            new("Item 1", "Desc 1", "", 10, 1, "", Condition.New, Availability.Available),
            new("Item 2", "Desc 2", "", 20, 1, "", Condition.UsedGood, Availability.Available),
        };

        var testItemDTOs = new List<ItemDTO>
        {
            new(1, "Item 1", "Desc 1", "", 10, 1, "", Condition.New, Availability.Available),
            new(2, "Item 2", "Desc 2", "", 20, 1, "", Condition.UsedGood, Availability.Available),
        };

        _mockItemRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(testItems);
        _mockMapper.Setup(mapper => mapper.Map<List<ItemDTO>>(testItems)).Returns(testItemDTOs);

        // Act
        var result = await _itemService.GetAllItemsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Same(testItemDTOs, result);
        _mockItemRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        _mockMapper.Verify(mapper => mapper.Map<List<ItemDTO>>(testItems), Times.Once);
    }

    [Fact]
    public async Task GetAllItemsAsync_ShouldReturnEmptyList_WhenNoItemsExist()
    {
        // Arrange
        var emptyItems = new List<Item>();
        var emptyItemDTOs = new List<ItemDTO>();

        _mockItemRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(emptyItems);
        _mockMapper.Setup(mapper => mapper.Map<List<ItemDTO>>(emptyItems)).Returns(emptyItemDTOs);

        // Act
        var result = await _itemService.GetAllItemsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _mockItemRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        _mockMapper.Verify(mapper => mapper.Map<List<ItemDTO>>(emptyItems), Times.Once);
    }

    #endregion

    #region GetItemByIdAsync


    [Fact]
    public async Task GetItemByIdAsync_ShouldReturnItemDTO_WhenItemExists()
    {
        // Arrange
        const long itemId = 1L;
        var testItem = new Item(
            "Test Item",
            "Desc",
            "",
            100,
            1,
            "",
            Condition.New,
            Availability.Available
        )
        {
            Id = itemId,
        };
        var testItemDto = new ItemDTO(
            itemId,
            "Test Item",
            "Desc",
            "",
            100,
            1,
            "",
            Condition.New,
            Availability.Available
        );

        _mockItemRepository.Setup(repo => repo.GetByIdAsync(itemId)).ReturnsAsync(testItem);
        _mockMapper.Setup(mapper => mapper.Map<ItemDTO>(testItem)).Returns(testItemDto);

        // Act
        var result = await _itemService.GetItemByIdAsync(itemId);

        // Assert
        Assert.NotNull(result);
        Assert.Same(testItemDto, result);
        _mockItemRepository.Verify(repo => repo.GetByIdAsync(itemId), Times.Once);
        _mockMapper.Verify(mapper => mapper.Map<ItemDTO>(testItem), Times.Once);
    }

    [Fact]
    public async Task GetItemByIdAsync_ShouldThrowNotFoundException_WhenItemDoesNotExist()
    {
        // Arrange
        const long itemId = 99L;
        _mockItemRepository.Setup(repo => repo.GetByIdAsync(itemId)).ReturnsAsync((Item?)null);

        // Act & Assert
        await Assert.ThrowsAsync<TradeHub.Api.Utilities.NotFoundException>(() =>
            _itemService.GetItemByIdAsync(itemId)
        );

        _mockItemRepository.Verify(repo => repo.GetByIdAsync(itemId), Times.Once);
        _mockMapper.Verify(mapper => mapper.Map<ItemDTO>(It.IsAny<Item>()), Times.Never);
    }

    #endregion

    #region CreateItemAsync

    [Fact]
    public async Task CreateItemAsync_ShouldReturnCreatedItemDTO_WhenSuccessful()
    {
        // Arrange
        // 1. The DTO coming into the service
        var createDto = new CreateItemDTO(
            "New Item",
            "New Desc",
            "",
            150,
            1,
            "",
            Condition.New,
            Availability.Available
        );

        // 2. The Item entity after the first mapping (no ID yet)
        var itemToCreate = new Item(
            "New Item",
            "New Desc",
            "",
            150,
            1,
            "",
            Condition.New,
            Availability.Available
        );

        // 3. The Item entity returned from the repository (now has an ID)
        var createdItemWithId = new Item(
            "New Item",
            "New Desc",
            "",
            150,
            1,
            "",
            Condition.New,
            Availability.Available
        )
        {
            Id = 123L,
        };

        // 4. The final DTO returned by the service
        var finalItemDto = new ItemDTO(
            123L,
            "New Item",
            "New Desc",
            "",
            150,
            1,
            "",
            Condition.New,
            Availability.Available
        );

        // Setup the mock behaviors
        _mockMapper.Setup(m => m.Map<Item>(createDto)).Returns(itemToCreate);
        _mockItemRepository.Setup(r => r.CreateAsync(itemToCreate)).ReturnsAsync(createdItemWithId);
        _mockMapper.Setup(m => m.Map<ItemDTO>(createdItemWithId)).Returns(finalItemDto);

        // Act
        var result = await _itemService.CreateItemAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Same(finalItemDto, result);
        Assert.Equal(123L, result.Id);

        _mockMapper.Verify(m => m.Map<Item>(createDto), Times.Once);
        _mockItemRepository.Verify(r => r.CreateAsync(itemToCreate), Times.Once);
        _mockMapper.Verify(m => m.Map<ItemDTO>(createdItemWithId), Times.Once);
    }

    #endregion

    #region UpdateItemAsync

    [Fact]
    public async Task UpdateItemAsync_ShouldReturnUpdatedItemDTO_WhenItemExists()
    {
        // Arrange
        const long itemId = 1L;
        var updateDto = new UpdateItemDTO(
            "Updated Name",
            "Updated Desc",
            null,
            150m,
            null,
            null,
            null,
            null
        );
        var existingItem = new Item(
            "Original Name",
            "Original Desc",
            "",
            100m,
            1,
            "",
            Condition.New,
            Availability.Available
        )
        {
            Id = itemId,
        };
        var updatedItemFromRepo = new Item(
            "Updated Name",
            "Updated Desc",
            "",
            150m,
            1,
            "",
            Condition.New,
            Availability.Available
        )
        {
            Id = itemId,
        };
        var finalItemDto = new ItemDTO(
            itemId,
            "Updated Name",
            "Updated Desc",
            "",
            150m,
            1,
            "",
            Condition.New,
            Availability.Available
        );

        _mockItemRepository.Setup(r => r.GetByIdAsync(itemId)).ReturnsAsync(existingItem);
        _mockItemRepository
            .Setup(r => r.UpdateAsync(existingItem))
            .ReturnsAsync(updatedItemFromRepo);
        _mockMapper.Setup(m => m.Map<ItemDTO>(updatedItemFromRepo)).Returns(finalItemDto);

        // Act
        var result = await _itemService.UpdateItemAsync(itemId, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Same(finalItemDto, result);

        _mockItemRepository.Verify(r => r.GetByIdAsync(itemId), Times.Once);
        _mockMapper.Verify(m => m.Map(updateDto, existingItem), Times.Once);
        _mockItemRepository.Verify(r => r.UpdateAsync(existingItem), Times.Once);
        _mockMapper.Verify(m => m.Map<ItemDTO>(updatedItemFromRepo), Times.Once);
    }

    [Fact]
    public async Task UpdateItemAsync_ShouldThrowNotFoundException_WhenItemDoesNotExist()
    {
        // Arrange
        const long itemId = 99L;
        var updateDto = new UpdateItemDTO("Updated Name", null, null, null, null, null, null, null);
        _mockItemRepository.Setup(r => r.GetByIdAsync(itemId)).ReturnsAsync((Item?)null);

        // Act & Assert
        await Assert.ThrowsAsync<TradeHub.Api.Utilities.NotFoundException>(() =>
            _itemService.UpdateItemAsync(itemId, updateDto)
        );

        _mockItemRepository.Verify(r => r.GetByIdAsync(itemId), Times.Once);
        _mockItemRepository.Verify(r => r.UpdateAsync(It.IsAny<Item>()), Times.Never);
        _mockMapper.Verify(m => m.Map(It.IsAny<UpdateItemDTO>(), It.IsAny<Item>()), Times.Never);
    }

    [Fact]
    public async Task UpdateItemAsync_ShouldPropagateConflictException_WhenRepositoryThrows()
    {
        // Arrange
        const long itemId = 1L;
        var updateDto = new UpdateItemDTO("Updated Name", null, null, null, null, null, null, null);
        var existingItem = new Item(
            "Original Name",
            "Desc",
            "",
            100m,
            1,
            "",
            Condition.New,
            Availability.Available
        )
        {
            Id = itemId,
        };

        _mockItemRepository.Setup(r => r.GetByIdAsync(itemId)).ReturnsAsync(existingItem);
        _mockItemRepository
            .Setup(r => r.UpdateAsync(existingItem))
            .ThrowsAsync(new TradeHub.Api.Utilities.ConflictException("Concurrency conflict"));

        // Act & Assert
        await Assert.ThrowsAsync<TradeHub.Api.Utilities.ConflictException>(() =>
            _itemService.UpdateItemAsync(itemId, updateDto)
        );

        _mockItemRepository.Verify(r => r.GetByIdAsync(itemId), Times.Once);
        _mockItemRepository.Verify(r => r.UpdateAsync(existingItem), Times.Once);
        _mockMapper.Verify(m => m.Map<ItemDTO>(It.IsAny<Item>()), Times.Never);
    }

    #endregion

    #region DeleteItemAsync

    [Fact]
    public async Task DeleteItemAsync_ShouldReturnTrue_WhenItemExistsAndDeleteIsSuccessful()
    {
        // Arrange
        const long itemId = 1L;
        var itemToDelete = new Item(
            "Test",
            "Desc",
            "",
            1,
            1,
            "",
            Condition.New,
            Availability.Available
        )
        {
            Id = itemId,
        };

        _mockItemRepository.Setup(r => r.GetByIdAsync(itemId)).ReturnsAsync(itemToDelete);
        _mockItemRepository.Setup(r => r.DeleteAsync(itemToDelete)).ReturnsAsync(true);

        // Act
        var result = await _itemService.DeleteItemAsync(itemId);

        // Assert
        Assert.True(result);
        _mockItemRepository.Verify(r => r.GetByIdAsync(itemId), Times.Once);
        _mockItemRepository.Verify(r => r.DeleteAsync(itemToDelete), Times.Once);
    }

    [Fact]
    public async Task DeleteItemAsync_ShouldReturnFalse_WhenDeleteFailsInRepository()
    {
        // Arrange
        const long itemId = 1L;
        var itemToDelete = new Item(
            "Test",
            "Desc",
            "",
            1,
            1,
            "",
            Condition.New,
            Availability.Available
        )
        {
            Id = itemId,
        };

        _mockItemRepository.Setup(r => r.GetByIdAsync(itemId)).ReturnsAsync(itemToDelete);
        _mockItemRepository.Setup(r => r.DeleteAsync(itemToDelete)).ReturnsAsync(false);

        // Act
        var result = await _itemService.DeleteItemAsync(itemId);

        // Assert
        Assert.False(result);
        _mockItemRepository.Verify(r => r.GetByIdAsync(itemId), Times.Once);
        _mockItemRepository.Verify(r => r.DeleteAsync(itemToDelete), Times.Once);
    }

    [Fact]
    public async Task DeleteItemAsync_ShouldThrowNotFoundException_WhenItemDoesNotExist()
    {
        // Arrange
        const long itemId = 99L;
        _mockItemRepository.Setup(r => r.GetByIdAsync(itemId)).ReturnsAsync((Item?)null);

        // Act & Assert
        await Assert.ThrowsAsync<TradeHub.Api.Utilities.NotFoundException>(() =>
            _itemService.DeleteItemAsync(itemId)
        );

        _mockItemRepository.Verify(r => r.GetByIdAsync(itemId), Times.Once);
        _mockItemRepository.Verify(r => r.DeleteAsync(It.IsAny<Item>()), Times.Never);
    }

    #endregion
}
