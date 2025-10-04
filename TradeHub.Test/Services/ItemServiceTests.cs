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
        var fakeItems = new List<Item>
        {
            new("Item 1", "Desc 1", "", 10, 1, "", Condition.New, Availability.Available),
            new("Item 2", "Desc 2", "", 20, 1, "", Condition.UsedGood, Availability.Available),
        };

        var fakeItemDTOs = new List<ItemDTO>
        {
            new(1, "Item 1", "Desc 1", "", 10, 1, "", Condition.New, Availability.Available),
            new(2, "Item 2", "Desc 2", "", 20, 1, "", Condition.UsedGood, Availability.Available),
        };

        _mockItemRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(fakeItems);
        _mockMapper.Setup(mapper => mapper.Map<List<ItemDTO>>(fakeItems)).Returns(fakeItemDTOs);

        // Act
        var result = await _itemService.GetAllItemsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Same(fakeItemDTOs, result);
        _mockItemRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        _mockMapper.Verify(mapper => mapper.Map<List<ItemDTO>>(fakeItems), Times.Once);
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
        var fakeItem = new Item(
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
        var fakeItemDto = new ItemDTO(
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

        _mockItemRepository.Setup(repo => repo.GetByIdAsync(itemId)).ReturnsAsync(fakeItem);
        _mockMapper.Setup(mapper => mapper.Map<ItemDTO>(fakeItem)).Returns(fakeItemDto);

        // Act
        var result = await _itemService.GetItemByIdAsync(itemId);

        // Assert
        Assert.NotNull(result);
        Assert.Same(fakeItemDto, result);
        _mockItemRepository.Verify(repo => repo.GetByIdAsync(itemId), Times.Once);
        _mockMapper.Verify(mapper => mapper.Map<ItemDTO>(fakeItem), Times.Once);
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
}
