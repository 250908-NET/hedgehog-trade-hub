using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using TradeHub.API.Models;
using TradeHub.API.Models.DTOs;
using TradeHub.API.Repository.Interfaces;
using TradeHub.API.Services;
using TradeHub.API.Services.Interfaces;

namespace TradeHub.Test.Services;

public class ItemServiceTests
{
    private readonly Mock<IItemRepository> _mockItemRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILLMService> _mockLLMService;
    private readonly Mock<ILogger<ItemService>> _mockLogger;
    private readonly ItemService _itemService;

    public ItemServiceTests()
    {
        _mockItemRepository = new Mock<IItemRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLLMService = new Mock<ILLMService>();
        _mockLogger = new Mock<ILogger<ItemService>>();

        // setup for in-place mapping for UpdateItemAsync
        // neccessary for UpdateItemAsync LLM estimation tests to work
        _mockMapper
            .Setup(m => m.Map(It.IsAny<UpdateItemDTO>(), It.IsAny<Item>()))
            .Callback<UpdateItemDTO, Item>(
                (dto, item) =>
                {
                    // simulate AutoMapper's behavior for updating an existing entity
                    if (dto.Name != null)
                        item.Name = dto.Name;
                    if (dto.Description != null)
                        item.Description = dto.Description;
                    if (dto.Image != null)
                        item.Image = dto.Image;
                    if (dto.Value != null)
                        item.Value = dto.Value.Value;
                    if (dto.OwnerId != null)
                        item.OwnerId = dto.OwnerId.Value;
                    if (dto.Tags != null)
                        item.Tags = dto.Tags;
                    if (dto.Condition != null)
                        item.Condition = dto.Condition.Value;
                    if (dto.Availability != null)
                        item.Availability = dto.Availability.Value;
                    // IsValueEstimated is not directly mapped from UpdateItemDTO
                    // RowVersion is handled by the repository/EF Core and not part of DTO mapping here
                }
            );

        _itemService = new ItemService(
            _mockItemRepository.Object,
            _mockMapper.Object,
            _mockLogger.Object,
            _mockLLMService.Object
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

        _mockItemRepository
            .Setup(repo =>
                repo.GetAllAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<decimal?>(),
                    It.IsAny<decimal?>(),
                    It.IsAny<Condition?>(),
                    It.IsAny<Availability?>(),
                    It.IsAny<string>()
                )
            )
            .ReturnsAsync(testItems);
        _mockMapper.Setup(mapper => mapper.Map<List<ItemDTO>>(testItems)).Returns(testItemDTOs);

        // Act
        var result = await _itemService.GetAllItemsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Same(testItemDTOs, result);
        _mockItemRepository.Verify(
            repo =>
                repo.GetAllAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<decimal?>(),
                    It.IsAny<decimal?>(),
                    It.IsAny<Condition?>(),
                    It.IsAny<Availability?>(),
                    It.IsAny<string>()
                ),
            Times.Once
        );
        _mockMapper.Verify(mapper => mapper.Map<List<ItemDTO>>(testItems), Times.Once);
    }

    [Fact]
    public async Task GetAllItemsAsync_ShouldReturnEmptyList_WhenNoItemsExist()
    {
        // Arrange
        var emptyItems = new List<Item>();
        var emptyItemDTOs = new List<ItemDTO>();

        _mockItemRepository
            .Setup(repo =>
                repo.GetAllAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<decimal?>(),
                    It.IsAny<decimal?>(),
                    It.IsAny<Condition?>(),
                    It.IsAny<Availability?>(),
                    It.IsAny<string>()
                )
            )
            .ReturnsAsync(emptyItems);
        _mockMapper.Setup(mapper => mapper.Map<List<ItemDTO>>(emptyItems)).Returns(emptyItemDTOs);

        // Act
        var result = await _itemService.GetAllItemsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _mockItemRepository.Verify(
            repo =>
                repo.GetAllAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<decimal?>(),
                    It.IsAny<decimal?>(),
                    It.IsAny<Condition?>(),
                    It.IsAny<Availability?>(),
                    It.IsAny<string>()
                ),
            Times.Once
        );
        _mockMapper.Verify(mapper => mapper.Map<List<ItemDTO>>(emptyItems), Times.Once);
    }

    [Fact]
    public async Task GetAllItemsAsync_ShouldPassPaginationParametersToRepository()
    {
        // Arrange
        var testItems = new List<Item>
        {
            new("Item 1", "Desc 1", "", 10, 1, "", Condition.New, Availability.Available),
        };
        var testItemDTOs = new List<ItemDTO>
        {
            new(1, "Item 1", "Desc 1", "", 10, 1, "", Condition.New, Availability.Available),
        };

        _mockItemRepository
            .Setup(repo =>
                repo.GetAllAsync(
                    2, // page
                    5, // pageSize
                    It.IsAny<decimal?>(),
                    It.IsAny<decimal?>(),
                    It.IsAny<Condition?>(),
                    It.IsAny<Availability?>(),
                    It.IsAny<string>()
                )
            )
            .ReturnsAsync(testItems);
        _mockMapper.Setup(mapper => mapper.Map<List<ItemDTO>>(testItems)).Returns(testItemDTOs);

        // Act
        var result = await _itemService.GetAllItemsAsync(page: 2, pageSize: 5);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        _mockItemRepository.Verify(
            repo => repo.GetAllAsync(2, 5, null, null, null, null, null),
            Times.Once
        );
    }

    [Fact]
    public async Task GetAllItemsAsync_ShouldPassMinValueFilterToRepository()
    {
        // Arrange
        var testItems = new List<Item>
        {
            new("Item 2", "Desc 2", "", 20, 1, "", Condition.UsedGood, Availability.Available),
        };
        var testItemDTOs = new List<ItemDTO>
        {
            new(2, "Item 2", "Desc 2", "", 20, 1, "", Condition.UsedGood, Availability.Available),
        };

        _mockItemRepository
            .Setup(repo =>
                repo.GetAllAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    15m, // minValue
                    It.IsAny<decimal?>(),
                    It.IsAny<Condition?>(),
                    It.IsAny<Availability?>(),
                    It.IsAny<string>()
                )
            )
            .ReturnsAsync(testItems);
        _mockMapper.Setup(mapper => mapper.Map<List<ItemDTO>>(testItems)).Returns(testItemDTOs);

        // Act
        var result = await _itemService.GetAllItemsAsync(minValue: 15m);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        _mockItemRepository.Verify(
            repo => repo.GetAllAsync(1, 10, 15m, null, null, null, null),
            Times.Once
        );
    }

    [Fact]
    public async Task GetAllItemsAsync_ShouldPassMaxValueFilterToRepository()
    {
        // Arrange
        var testItems = new List<Item>
        {
            new("Item 1", "Desc 1", "", 10, 1, "", Condition.New, Availability.Available),
        };
        var testItemDTOs = new List<ItemDTO>
        {
            new(1, "Item 1", "Desc 1", "", 10, 1, "", Condition.New, Availability.Available),
        };

        _mockItemRepository
            .Setup(repo =>
                repo.GetAllAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<decimal?>(),
                    15m, // maxValue
                    It.IsAny<Condition?>(),
                    It.IsAny<Availability?>(),
                    It.IsAny<string>()
                )
            )
            .ReturnsAsync(testItems);
        _mockMapper.Setup(mapper => mapper.Map<List<ItemDTO>>(testItems)).Returns(testItemDTOs);

        // Act
        var result = await _itemService.GetAllItemsAsync(maxValue: 15m);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        _mockItemRepository.Verify(
            repo => repo.GetAllAsync(1, 10, null, 15m, null, null, null),
            Times.Once
        );
    }

    [Fact]
    public async Task GetAllItemsAsync_ShouldPassConditionFilterToRepository()
    {
        // Arrange
        var testItems = new List<Item>
        {
            new("Item 2", "Desc 2", "", 20, 1, "", Condition.UsedGood, Availability.Available),
        };
        var testItemDTOs = new List<ItemDTO>
        {
            new(2, "Item 2", "Desc 2", "", 20, 1, "", Condition.UsedGood, Availability.Available),
        };

        _mockItemRepository
            .Setup(repo =>
                repo.GetAllAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<decimal?>(),
                    It.IsAny<decimal?>(),
                    Condition.UsedGood, // condition
                    It.IsAny<Availability?>(),
                    It.IsAny<string>()
                )
            )
            .ReturnsAsync(testItems);
        _mockMapper.Setup(mapper => mapper.Map<List<ItemDTO>>(testItems)).Returns(testItemDTOs);

        // Act
        var result = await _itemService.GetAllItemsAsync(condition: Condition.UsedGood);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        _mockItemRepository.Verify(
            repo => repo.GetAllAsync(1, 10, null, null, Condition.UsedGood, null, null),
            Times.Once
        );
    }

    [Fact]
    public async Task GetAllItemsAsync_ShouldPassAvailabilityFilterToRepository()
    {
        // Arrange
        var testItems = new List<Item>
        {
            new("Item 1", "Desc 1", "", 10, 1, "", Condition.New, Availability.Available),
        };
        var testItemDTOs = new List<ItemDTO>
        {
            new(1, "Item 1", "Desc 1", "", 10, 1, "", Condition.New, Availability.Available),
        };

        _mockItemRepository
            .Setup(repo =>
                repo.GetAllAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<decimal?>(),
                    It.IsAny<decimal?>(),
                    It.IsAny<Condition?>(),
                    Availability.Available, // availability
                    It.IsAny<string>()
                )
            )
            .ReturnsAsync(testItems);
        _mockMapper.Setup(mapper => mapper.Map<List<ItemDTO>>(testItems)).Returns(testItemDTOs);

        // Act
        var result = await _itemService.GetAllItemsAsync(availability: Availability.Available);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        _mockItemRepository.Verify(
            repo => repo.GetAllAsync(1, 10, null, null, null, Availability.Available, null),
            Times.Once
        );
    }

    [Fact]
    public async Task GetAllItemsAsync_ShouldPassSearchTermToRepository()
    {
        // Arrange
        var testItems = new List<Item>
        {
            new(
                "Searchable Item",
                "This item is for searching",
                "",
                10,
                1,
                "",
                Condition.New,
                Availability.Available
            ),
        };
        var testItemDTOs = new List<ItemDTO>
        {
            new(
                1,
                "Searchable Item",
                "This item is for searching",
                "",
                10,
                1,
                "",
                Condition.New,
                Availability.Available
            ),
        };

        _mockItemRepository
            .Setup(repo =>
                repo.GetAllAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<decimal?>(),
                    It.IsAny<decimal?>(),
                    It.IsAny<Condition?>(),
                    It.IsAny<Availability?>(),
                    "search term" // search
                )
            )
            .ReturnsAsync(testItems);
        _mockMapper.Setup(mapper => mapper.Map<List<ItemDTO>>(testItems)).Returns(testItemDTOs);

        // Act
        var result = await _itemService.GetAllItemsAsync(search: "search term");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        _mockItemRepository.Verify(
            repo => repo.GetAllAsync(1, 10, null, null, null, null, "search term"),
            Times.Once
        );
    }

    [Fact]
    public async Task GetAllItemsAsync_ShouldPassAllParametersToRepository()
    {
        // Arrange
        var testItems = new List<Item>
        {
            new(
                "Specific Item",
                "Specific Description",
                "",
                25,
                1,
                "",
                Condition.UsedGood,
                Availability.Available
            ),
        };
        var testItemDTOs = new List<ItemDTO>
        {
            new(
                1,
                "Specific Item",
                "Specific Description",
                "",
                25,
                1,
                "",
                Condition.UsedGood,
                Availability.Available
            ),
        };

        _mockItemRepository
            .Setup(repo =>
                repo.GetAllAsync(
                    2, // page
                    5, // pageSize
                    20m, // minValue
                    30m, // maxValue
                    Condition.UsedGood, // condition
                    Availability.Available, // availability
                    "Specific" // search
                )
            )
            .ReturnsAsync(testItems);
        _mockMapper.Setup(mapper => mapper.Map<List<ItemDTO>>(testItems)).Returns(testItemDTOs);

        // Act
        var result = await _itemService.GetAllItemsAsync(
            page: 2,
            pageSize: 5,
            minValue: 20m,
            maxValue: 30m,
            condition: Condition.UsedGood,
            availability: Availability.Available,
            search: "Specific"
        );

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        _mockItemRepository.Verify(
            repo =>
                repo.GetAllAsync(
                    2,
                    5,
                    20m,
                    30m,
                    Condition.UsedGood,
                    Availability.Available,
                    "Specific"
                ),
            Times.Once
        );
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
        await Assert.ThrowsAsync<API.Utilities.NotFoundException>(() =>
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

    #region CreateItemAsync (LLM Value Estimation)

    [Fact]
    public async Task CreateItemAsync_ShouldEstimateValue_WhenLLMServiceAvailableAndEstimateValueIsTrueAndValueIsZero()
    {
        // Arrange
        var createDto = new CreateItemDTO(
            "New Item",
            "New Desc",
            "",
            0, // Value is 0
            1,
            "",
            Condition.New,
            Availability.Available,
            EstimateValue: true // EstimateValue is true
        );
        var estimatedValue = 250m;
        var itemToCreate = new Item(
            "New Item",
            "New Desc",
            "",
            estimatedValue, // Expected estimated value
            1,
            "",
            Condition.New,
            Availability.Available
        );
        var createdItemWithId = new Item(
            "New Item",
            "New Desc",
            "",
            estimatedValue,
            1,
            "",
            Condition.New,
            Availability.Available
        )
        {
            Id = 123L,
        };
        var finalItemDto = new ItemDTO(
            123L,
            "New Item",
            "New Desc",
            "",
            estimatedValue,
            1,
            "",
            Condition.New,
            Availability.Available
        );

        _mockLLMService
            .Setup(s =>
                s.EstimateItemValueAsync(
                    createDto.Name,
                    createDto.Description,
                    createDto.Condition.ToString(),
                    "USD"
                )
            )
            .ReturnsAsync(estimatedValue);
        _mockMapper.Setup(m => m.Map<Item>(It.IsAny<CreateItemDTO>())).Returns(itemToCreate);
        _mockItemRepository.Setup(r => r.CreateAsync(itemToCreate)).ReturnsAsync(createdItemWithId);
        _mockMapper.Setup(m => m.Map<ItemDTO>(createdItemWithId)).Returns(finalItemDto);

        // Act
        var result = await _itemService.CreateItemAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(estimatedValue, result.Value);
        _mockLLMService.Verify(
            s =>
                s.EstimateItemValueAsync(
                    createDto.Name,
                    createDto.Description,
                    createDto.Condition.ToString(),
                    "USD"
                ),
            Times.Once
        );
        _mockItemRepository.Verify(
            r => r.CreateAsync(It.Is<Item>(i => i.Value == estimatedValue)),
            Times.Once
        );
    }

    [Fact]
    public async Task CreateItemAsync_ShouldNotEstimateValue_WhenLLMServiceIsNull()
    {
        // Arrange
        // Create ItemService with null LLMService
        var itemServiceWithoutLLM = new ItemService(
            _mockItemRepository.Object,
            _mockMapper.Object,
            _mockLogger.Object,
            null // LLMService is null
        );

        var createDto = new CreateItemDTO(
            "New Item",
            "New Desc",
            "",
            0, // Value is 0
            1,
            "",
            Condition.New,
            Availability.Available,
            EstimateValue: true // EstimateValue is true
        );
        var itemToCreate = new Item(
            "New Item",
            "New Desc",
            "",
            0, // Expected original value
            1,
            "",
            Condition.New,
            Availability.Available
        );
        var createdItemWithId = new Item(
            "New Item",
            "New Desc",
            "",
            0,
            1,
            "",
            Condition.New,
            Availability.Available
        )
        {
            Id = 123L,
        };
        var finalItemDto = new ItemDTO(
            123L,
            "New Item",
            "New Desc",
            "",
            0,
            1,
            "",
            Condition.New,
            Availability.Available
        );

        _mockMapper.Setup(m => m.Map<Item>(It.IsAny<CreateItemDTO>())).Returns(itemToCreate);
        _mockItemRepository.Setup(r => r.CreateAsync(itemToCreate)).ReturnsAsync(createdItemWithId);
        _mockMapper.Setup(m => m.Map<ItemDTO>(createdItemWithId)).Returns(finalItemDto);

        // Act
        var result = await itemServiceWithoutLLM.CreateItemAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0m, result.Value);
        _mockLLMService.Verify(
            s =>
                s.EstimateItemValueAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ),
            Times.Never
        );
        _mockItemRepository.Verify(r => r.CreateAsync(It.Is<Item>(i => i.Value == 0)), Times.Once);
    }

    [Fact]
    public async Task CreateItemAsync_ShouldNotEstimateValue_WhenEstimateValueIsFalse()
    {
        // Arrange
        var createDto = new CreateItemDTO(
            "New Item",
            "New Desc",
            "",
            0, // Value is 0
            1,
            "",
            Condition.New,
            Availability.Available,
            EstimateValue: false // EstimateValue is false
        );
        var itemToCreate = new Item(
            "New Item",
            "New Desc",
            "",
            0, // Expected original value
            1,
            "",
            Condition.New,
            Availability.Available
        );
        var createdItemWithId = new Item(
            "New Item",
            "New Desc",
            "",
            0,
            1,
            "",
            Condition.New,
            Availability.Available
        )
        {
            Id = 123L,
        };
        var finalItemDto = new ItemDTO(
            123L,
            "New Item",
            "New Desc",
            "",
            0,
            1,
            "",
            Condition.New,
            Availability.Available
        );

        _mockMapper.Setup(m => m.Map<Item>(It.IsAny<CreateItemDTO>())).Returns(itemToCreate);
        _mockItemRepository.Setup(r => r.CreateAsync(itemToCreate)).ReturnsAsync(createdItemWithId);
        _mockMapper.Setup(m => m.Map<ItemDTO>(createdItemWithId)).Returns(finalItemDto);

        // Act
        var result = await _itemService.CreateItemAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0m, result.Value);
        _mockLLMService.Verify(
            s =>
                s.EstimateItemValueAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ),
            Times.Never
        );
        _mockItemRepository.Verify(r => r.CreateAsync(It.Is<Item>(i => i.Value == 0)), Times.Once);
    }

    [Fact]
    public async Task CreateItemAsync_ShouldNotEstimateValue_WhenValueIsNonZero()
    {
        // Arrange
        var createDto = new CreateItemDTO(
            "New Item",
            "New Desc",
            "",
            100, // Value is non-zero
            1,
            "",
            Condition.New,
            Availability.Available,
            EstimateValue: true // EstimateValue is true
        );
        var itemToCreate = new Item(
            "New Item",
            "New Desc",
            "",
            100, // Expected original value
            1,
            "",
            Condition.New,
            Availability.Available
        );
        var createdItemWithId = new Item(
            "New Item",
            "New Desc",
            "",
            100,
            1,
            "",
            Condition.New,
            Availability.Available
        )
        {
            Id = 123L,
        };
        var finalItemDto = new ItemDTO(
            123L,
            "New Item",
            "New Desc",
            "",
            100,
            1,
            "",
            Condition.New,
            Availability.Available
        );

        _mockMapper.Setup(m => m.Map<Item>(It.IsAny<CreateItemDTO>())).Returns(itemToCreate);
        _mockItemRepository.Setup(r => r.CreateAsync(itemToCreate)).ReturnsAsync(createdItemWithId);
        _mockMapper.Setup(m => m.Map<ItemDTO>(createdItemWithId)).Returns(finalItemDto);

        // Act
        var result = await _itemService.CreateItemAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(100m, result.Value);
        _mockLLMService.Verify(
            s =>
                s.EstimateItemValueAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ),
            Times.Never
        );
        _mockItemRepository.Verify(
            r => r.CreateAsync(It.Is<Item>(i => i.Value == 100)),
            Times.Once
        );
    }

    [Fact]
    public async Task CreateItemAsync_ShouldProceedWithOriginalValue_WhenLLMServiceEstimationFails()
    {
        // Arrange
        var createDto = new CreateItemDTO(
            "New Item",
            "New Desc",
            "",
            0, // Value is 0
            1,
            "",
            Condition.New,
            Availability.Available,
            EstimateValue: true // EstimateValue is true
        );
        var itemToCreate = new Item(
            "New Item",
            "New Desc",
            "",
            0, // Expected original value
            1,
            "",
            Condition.New,
            Availability.Available
        );
        var createdItemWithId = new Item(
            "New Item",
            "New Desc",
            "",
            0,
            1,
            "",
            Condition.New,
            Availability.Available
        )
        {
            Id = 123L,
        };
        var finalItemDto = new ItemDTO(
            123L,
            "New Item",
            "New Desc",
            "",
            0,
            1,
            "",
            Condition.New,
            Availability.Available
        );

        _mockLLMService
            .Setup(s =>
                s.EstimateItemValueAsync(
                    createDto.Name,
                    createDto.Description,
                    createDto.Condition.ToString(),
                    "USD"
                )
            )
            .ThrowsAsync(new Exception("LLM service error")); // LLM service throws exception
        _mockMapper.Setup(m => m.Map<Item>(It.IsAny<CreateItemDTO>())).Returns(itemToCreate);
        _mockItemRepository.Setup(r => r.CreateAsync(itemToCreate)).ReturnsAsync(createdItemWithId);
        _mockMapper.Setup(m => m.Map<ItemDTO>(createdItemWithId)).Returns(finalItemDto);

        // Act
        var result = await _itemService.CreateItemAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0m, result.Value); // Value should remain 0
        _mockLLMService.Verify(
            s =>
                s.EstimateItemValueAsync(
                    createDto.Name,
                    createDto.Description,
                    createDto.Condition.ToString(),
                    "USD"
                ),
            Times.Once
        );
        _mockItemRepository.Verify(r => r.CreateAsync(It.Is<Item>(i => i.Value == 0)), Times.Once);
        _mockLogger.Verify(
            x =>
                x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>(
                        (o, t) =>
                            o.ToString()!.Contains("Failed to estimate item value for New Item.")
                    ),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
            Times.Once
        );
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
        await Assert.ThrowsAsync<API.Utilities.NotFoundException>(() =>
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
            .ThrowsAsync(new API.Utilities.ConflictException("Concurrency conflict"));

        // Act & Assert
        await Assert.ThrowsAsync<API.Utilities.ConflictException>(() =>
            _itemService.UpdateItemAsync(itemId, updateDto)
        );

        _mockItemRepository.Verify(r => r.GetByIdAsync(itemId), Times.Once);
        _mockItemRepository.Verify(r => r.UpdateAsync(existingItem), Times.Once);
        _mockMapper.Verify(m => m.Map<ItemDTO>(It.IsAny<Item>()), Times.Never);
    }

    #endregion

    #region UpdateItemAsync (LLM Value Estimation)

    [Fact]
    public async Task UpdateItemAsync_ShouldEstimateValue_WhenLLMServiceAvailableAndEstimateValueIsTrueAndValueIsZeroAndRequiredFieldsAreNotNull()
    {
        // Arrange
        const long itemId = 1L;
        var updateDto = new UpdateItemDTO(
            "Updated Name",
            "Updated Desc",
            null,
            0m, // Value is 0
            null,
            null,
            Condition.UsedGood, // Condition is not null
            null,
            EstimateValue: true // EstimateValue is true
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
        var estimatedValue = 250m;
        var updatedItemFromRepo = new Item(
            "Updated Name",
            "Updated Desc",
            "",
            estimatedValue, // Expected estimated value
            1,
            "",
            Condition.UsedGood,
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
            estimatedValue,
            1,
            "",
            Condition.UsedGood,
            Availability.Available
        );

        _mockItemRepository.Setup(r => r.GetByIdAsync(itemId)).ReturnsAsync(existingItem);
        _mockLLMService
            .Setup(s =>
                s.EstimateItemValueAsync(
                    updateDto.Name!,
                    updateDto.Description!,
                    updateDto.Condition!.Value.ToString(),
                    "USD"
                )
            )
            .ReturnsAsync(estimatedValue);
        _mockItemRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Item>()))
            .ReturnsAsync(updatedItemFromRepo);
        _mockMapper.Setup(m => m.Map<ItemDTO>(updatedItemFromRepo)).Returns(finalItemDto);

        // Act
        var result = await _itemService.UpdateItemAsync(itemId, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(estimatedValue, result.Value);
        _mockLLMService.Verify(
            s =>
                s.EstimateItemValueAsync(
                    updateDto.Name!,
                    updateDto.Description!,
                    updateDto.Condition!.Value.ToString(),
                    "USD"
                ),
            Times.Once
        );
        // This verify expects the item's value to be the estimated value after mapping
        _mockItemRepository.Verify(
            r =>
                r.UpdateAsync(
                    It.Is<Item>(i => i.Value == estimatedValue && i.IsValueEstimated == true)
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateItemAsync_ShouldNotEstimateValue_WhenLLMServiceIsNull()
    {
        // Arrange
        // Create ItemService with null LLMService
        var itemServiceWithoutLLM = new ItemService(
            _mockItemRepository.Object,
            _mockMapper.Object,
            _mockLogger.Object,
            null // LLMService is null
        );

        const long itemId = 1L;
        var updateDto = new UpdateItemDTO(
            "Updated Name",
            "Updated Desc",
            null,
            0m, // Value is 0
            null,
            null,
            Condition.UsedGood,
            null,
            EstimateValue: true // EstimateValue is true
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
            0m, // Expected value from DTO after mapping
            1,
            "",
            Condition.UsedGood,
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
            0m, // Expected value from DTO after mapping
            1,
            "",
            Condition.UsedGood,
            Availability.Available
        );

        _mockItemRepository.Setup(r => r.GetByIdAsync(itemId)).ReturnsAsync(existingItem);
        _mockItemRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Item>()))
            .ReturnsAsync(updatedItemFromRepo);
        _mockMapper.Setup(m => m.Map<ItemDTO>(updatedItemFromRepo)).Returns(finalItemDto);

        // Act
        var result = await itemServiceWithoutLLM.UpdateItemAsync(itemId, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0m, result.Value); // Value should be from DTO
        _mockLLMService.Verify(
            s =>
                s.EstimateItemValueAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ),
            Times.Never
        );
        // Verify that the item was updated with the DTO's value (0m)
        _mockItemRepository.Verify(
            r => r.UpdateAsync(It.Is<Item>(i => i.Value == 0 && i.IsValueEstimated == false)),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateItemAsync_ShouldNotEstimateValue_WhenEstimateValueIsFalse()
    {
        // Arrange
        const long itemId = 1L;
        var updateDto = new UpdateItemDTO(
            "Updated Name",
            "Updated Desc",
            null,
            0m, // Value is 0
            null,
            null,
            Condition.UsedGood,
            null,
            EstimateValue: false // EstimateValue is false
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
            0m, // Expected value from DTO after mapping
            1,
            "",
            Condition.UsedGood,
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
            0m, // Expected value from DTO after mapping
            1,
            "",
            Condition.UsedGood,
            Availability.Available
        );

        _mockItemRepository.Setup(r => r.GetByIdAsync(itemId)).ReturnsAsync(existingItem);
        _mockItemRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Item>()))
            .ReturnsAsync(updatedItemFromRepo);
        _mockMapper.Setup(m => m.Map<ItemDTO>(updatedItemFromRepo)).Returns(finalItemDto);

        // Act
        var result = await _itemService.UpdateItemAsync(itemId, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0m, result.Value); // Value should be from DTO
        _mockLLMService.Verify(
            s =>
                s.EstimateItemValueAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ),
            Times.Never
        );
        // Verify that the item was updated with the DTO's value (0m)
        _mockItemRepository.Verify(
            r => r.UpdateAsync(It.Is<Item>(i => i.Value == 0 && i.IsValueEstimated == false)),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateItemAsync_ShouldNotEstimateValue_WhenValueIsNonZero()
    {
        // Arrange
        const long itemId = 1L;
        var updateDto = new UpdateItemDTO(
            "Updated Name",
            "Updated Desc",
            null,
            50m, // Value is non-zero
            null,
            null,
            Condition.UsedGood,
            null,
            EstimateValue: true // EstimateValue is true
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
            50m, // Expected value from DTO
            1,
            "",
            Condition.UsedGood,
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
            50m,
            1,
            "",
            Condition.UsedGood,
            Availability.Available
        );

        _mockItemRepository.Setup(r => r.GetByIdAsync(itemId)).ReturnsAsync(existingItem);
        _mockItemRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Item>()))
            .ReturnsAsync(updatedItemFromRepo);
        _mockMapper.Setup(m => m.Map<ItemDTO>(updatedItemFromRepo)).Returns(finalItemDto);

        // Act
        var result = await _itemService.UpdateItemAsync(itemId, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(50m, result.Value); // Value should be from DTO
        _mockLLMService.Verify(
            s =>
                s.EstimateItemValueAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ),
            Times.Never
        );
        // Verify that the item was updated with the DTO's value (50m)
        _mockItemRepository.Verify(
            r => r.UpdateAsync(It.Is<Item>(i => i.Value == 50 && i.IsValueEstimated == false)),
            Times.Once
        );
    }

    [Theory]
    [InlineData(null, "Updated Desc", Condition.UsedGood)] // Name is null
    [InlineData("Updated Name", null, Condition.UsedGood)] // Description is null
    [InlineData("Updated Name", "Updated Desc", null)] // Condition is null
    public async Task UpdateItemAsync_ShouldNotEstimateValue_WhenRequiredFieldsAreNull(
        string? name,
        string? description,
        Condition? condition
    )
    {
        // Arrange
        const long itemId = 1L;
        var updateDto = new UpdateItemDTO(
            name,
            description,
            null,
            0m, // Value is 0
            null,
            null,
            condition,
            null,
            EstimateValue: true // EstimateValue is true
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
            name ?? "Original Name", // Mapper will use original if null
            description ?? "Original Desc",
            "",
            0m, // Expected value from DTO after mapping
            1,
            "",
            condition ?? Condition.New,
            Availability.Available
        )
        {
            Id = itemId,
        };
        var finalItemDto = new ItemDTO(
            itemId,
            name ?? "Original Name",
            description ?? "Original Desc",
            "",
            0m, // Expected value from DTO after mapping
            1,
            "",
            condition ?? Condition.New,
            Availability.Available
        );

        _mockItemRepository.Setup(r => r.GetByIdAsync(itemId)).ReturnsAsync(existingItem);
        _mockItemRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Item>()))
            .ReturnsAsync(updatedItemFromRepo);
        _mockMapper.Setup(m => m.Map<ItemDTO>(updatedItemFromRepo)).Returns(finalItemDto);

        // Act
        var result = await _itemService.UpdateItemAsync(itemId, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0m, result.Value); // Value should be from DTO
        _mockLLMService.Verify(
            s =>
                s.EstimateItemValueAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ),
            Times.Never
        );
        // Verify that the item was updated with the DTO's value (0m)
        _mockItemRepository.Verify(
            r => r.UpdateAsync(It.Is<Item>(i => i.Value == 0 && i.IsValueEstimated == false)),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateItemAsync_ShouldProceedWithOriginalValue_WhenLLMServiceEstimationFails()
    {
        // Arrange
        const long itemId = 1L;
        var updateDto = new UpdateItemDTO(
            "Updated Name",
            "Updated Desc",
            null,
            0m, // Value is 0
            null,
            null,
            Condition.UsedGood,
            null,
            EstimateValue: true // EstimateValue is true
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
            0m, // Expected value from DTO after mapping
            1,
            "",
            Condition.UsedGood,
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
            0m, // Expected value from DTO after mapping
            1,
            "",
            Condition.UsedGood,
            Availability.Available
        );

        _mockItemRepository.Setup(r => r.GetByIdAsync(itemId)).ReturnsAsync(existingItem);
        _mockLLMService
            .Setup(s =>
                s.EstimateItemValueAsync(
                    updateDto.Name!,
                    updateDto.Description!,
                    updateDto.Condition!.Value.ToString(),
                    "USD"
                )
            )
            .ThrowsAsync(new Exception("LLM service error")); // LLM service throws exception
        _mockItemRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Item>()))
            .ReturnsAsync(updatedItemFromRepo);
        _mockMapper.Setup(m => m.Map<ItemDTO>(updatedItemFromRepo)).Returns(finalItemDto);

        // Act
        var result = await _itemService.UpdateItemAsync(itemId, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0m, result.Value); // Value should be from DTO
        _mockLLMService.Verify(
            s =>
                s.EstimateItemValueAsync(
                    updateDto.Name!,
                    updateDto.Description!,
                    updateDto.Condition!.Value.ToString(),
                    "USD"
                ),
            Times.Once
        );
        // Verify that the item was updated with the DTO's value (0m)
        _mockItemRepository.Verify(
            r => r.UpdateAsync(It.Is<Item>(i => i.Value == 0 && i.IsValueEstimated == false)),
            Times.Once
        );
        _mockLogger.Verify(
            x =>
                x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>(
                        (o, t) =>
                            o.ToString()!
                                .Contains("Failed to estimate item value for Updated Name.")
                    ),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
            Times.Once
        );
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
        await Assert.ThrowsAsync<API.Utilities.NotFoundException>(() =>
            _itemService.DeleteItemAsync(itemId)
        );

        _mockItemRepository.Verify(r => r.GetByIdAsync(itemId), Times.Once);
        _mockItemRepository.Verify(r => r.DeleteAsync(It.IsAny<Item>()), Times.Never);
    }

    #endregion
}
