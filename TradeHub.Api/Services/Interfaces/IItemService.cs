using TradeHub.Api.Models;
using TradeHub.Api.Models.DTOs;

namespace TradeHub.Api.Services.Interfaces;

public interface IItemService
{
    Task<List<Item>> GetAllItemsAsync();
    Task<Item?> GetItemByIdAsync(int id);
    Task<Item> CreateItemAsync(CreateItemDTO itemDto);
    Task<bool> UpdateItemAsync(int id, UpdateItemDTO itemDto);
    Task<bool> DeleteItemAsync(int id);
}
