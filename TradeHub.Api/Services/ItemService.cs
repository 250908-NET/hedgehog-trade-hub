using TradeHub.Api.Models;
using TradeHub.Api.Models.DTOs;
using TradeHub.Api.Repository.Interfaces;
using TradeHub.Api.Services.Interfaces;

namespace TradeHub.Api.Services;

public class ItemService : IItemService
{
    private readonly IItemRepository _repository;

    public ItemService(IItemRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<List<Item>> GetAllItemsAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Item?> GetItemByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Item> CreateItemAsync(CreateItemDTO itemDto)
    {
        var item = new Item(
            itemDto.Name,
            itemDto.Description,
            itemDto.Image,
            itemDto.Value,
            itemDto.OwnerId,
            itemDto.Tags,
            itemDto.Condition,
            itemDto.Availability
        );

        var createdItem = await _repository.AddAsync(item);
        await _repository.SaveChangesAsync();
        return createdItem;
    }

    public async Task<bool> UpdateItemAsync(int id, UpdateItemDTO itemDto)
    {
        var existingItem = await _repository.GetByIdAsync(id);
        if (existingItem == null)
            return false;

        // Only update fields that are provided (not null)
        if (itemDto.Name is not null)
            existingItem.Name = itemDto.Name;
        if (itemDto.Description is not null)
            existingItem.Description = itemDto.Description;
        if (itemDto.Image is not null)
            existingItem.Image = itemDto.Image;
        if (itemDto.Value is not null)
            existingItem.Value = itemDto.Value.Value;
        if (itemDto.OwnerId is not null)
            existingItem.OwnerId = itemDto.OwnerId.Value;
        if (itemDto.Tags is not null)
            existingItem.Tags = itemDto.Tags;
        if (itemDto.Condition is not null)
            existingItem.Condition = itemDto.Condition.Value;
        if (itemDto.Availability is not null)
            existingItem.Availability = itemDto.Availability.Value;

        await _repository.UpdateAsync(id, existingItem);
        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteItemAsync(int id)
    {
        var deleted = await _repository.DeleteAsync(id);
        if (deleted)
        {
            await _repository.SaveChangesAsync();
        }
        return deleted;
    }
}