using AutoMapper;
using TradeHub.Api.Models;
using TradeHub.Api.Models.DTOs;
using TradeHub.Api.Repository.Interfaces;
using TradeHub.Api.Services.Interfaces;
using TradeHub.Api.Utilities;

namespace TradeHub.Api.Services;

public class ItemService(
    IItemRepository itemRepository,
    IMapper mapper,
    ILogger<ItemService> logger,
    ILLMService? llmService = null
) : IItemService
{
    private readonly IItemRepository _itemRepository = itemRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<ItemService> _logger = logger;
    private readonly ILLMService? _llmService = llmService;

    /// <summary>
    /// Fetch all items from database.
    /// </summary>
    /// <returns>A list of item DTOs</returns>
    public async Task<List<ItemDTO>> GetAllItemsAsync(
        int page = 1,
        int pageSize = 10,
        decimal? minValue = null,
        decimal? maxValue = null,
        Condition? condition = null,
        Availability? availability = null,
        string? search = null
    )
    {
        _logger.LogInformation("Fetching all items with filters and pagination...");
        List<Item> items = await _itemRepository.GetAllAsync(
            page,
            pageSize,
            minValue,
            maxValue,
            condition,
            availability,
            search
        );
        _logger.LogInformation("Found {Count} items matching the criteria!", items.Count);
        return _mapper.Map<List<ItemDTO>>(items);
    }

    /// <summary>
    /// Fetch item by id from database.
    /// </summary>
    /// <param name="id">The id of the item to fetch.</param>
    /// <returns>An item DTO if found.</returns>
    /// <exception cref="NotFoundException">Thrown if the item is not found.</exception>
    public async Task<ItemDTO> GetItemByIdAsync(long id)
    {
        _logger.LogInformation("Fetching item {Id}...", id);
        Item item =
            await _itemRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Item with ID {id} was not found.");
        _logger.LogInformation("Fetched item {Name} of (ID: {Id})!", item.Name, item.Id);
        return _mapper.Map<ItemDTO>(item);
    }

    /// <summary>
    /// Create a new item in the database.
    /// </summary>
    /// <param name="dto">The item to create.</param>
    /// <returns>The created item DTO.</returns>
    public async Task<ItemDTO> CreateItemAsync(CreateItemDTO dto)
    {
        _logger.LogInformation("Creating item {Name}...", dto.Name);

        // TODO: ensure user exists

        // value estimation
        if (_llmService is not null && dto.Value == 0 && dto.EstimateValue)
        {
            try
            {
                decimal estimatedValue = await _llmService.EstimateItemValueAsync(
                    dto.Name,
                    dto.Description,
                    dto.Condition.ToString()
                );

                dto = dto with { Value = estimatedValue };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to estimate item value for {Name}.", dto.Name);
                // on fail, proceed with the original DTO (with Value == 0)
            }
        }

        Item createdItem = await _itemRepository.CreateAsync(_mapper.Map<Item>(dto));
        _logger.LogInformation(
            "Created item {Name} of (ID: {Id})!",
            createdItem.Name,
            createdItem.Id
        );
        return _mapper.Map<ItemDTO>(createdItem);
    }

    public async Task<ItemDTO> UpdateItemAsync(long id, UpdateItemDTO dto)
    {
        _logger.LogInformation("Updating item {Name} (ID: {Id})...", dto.Name, id);

        // TODO: ensure user exists

        Item existingItem =
            await _itemRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Item with ID {id} was not found to update.");

        _mapper.Map(dto, existingItem); // update item in-place

        // value estimation
        if (
            _llmService is not null
            && dto.Value == 0
            && dto.EstimateValue
            && dto.Name is not null
            && dto.Description is not null
            && dto.Condition is not null
        )
        {
            try
            {
                _logger.LogInformation("Estimating value of item {Name} using LLM...", dto.Name);

                decimal estimatedValue = await _llmService.EstimateItemValueAsync(
                    dto.Name,
                    dto.Description,
                    (dto.Condition ?? Condition.UsedAcceptable).ToString() // should always be not null
                );

                existingItem.Value = estimatedValue;
                existingItem.IsValueEstimated = true;
                _logger.LogInformation(
                    "Estimated item value for {Name} as {Value}!",
                    dto.Name,
                    estimatedValue
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to estimate item value for {Name}.", dto.Name);
                // on fail, proceed with the original DTO (with Value == 0)
            }
        }

        Item updatedItem = await _itemRepository.UpdateAsync(existingItem);
        _logger.LogInformation(
            "Updated item {Name} of (ID: {Id})!",
            updatedItem.Name,
            updatedItem.Id
        );
        return _mapper.Map<ItemDTO>(updatedItem);
    }

    public async Task<bool> DeleteItemAsync(long id)
    {
        // check if item exists
        Item existingItem =
            await _itemRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Item with ID {id} was not found to delete.");

        _logger.LogInformation(
            "Deleting item {Name} (ID: {Id})...",
            existingItem.Name,
            existingItem.Id
        );
        return await _itemRepository.DeleteAsync(existingItem);
    }
}
