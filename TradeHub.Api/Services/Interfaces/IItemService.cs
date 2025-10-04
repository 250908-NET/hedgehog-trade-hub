using TradeHub.Api.Models.DTOs;

namespace TradeHub.Api.Services.Interfaces;

public interface IItemService
{
    Task<List<ItemDTO>> GetAllItemsAsync();
    Task<ItemDTO> GetItemByIdAsync(long id);
    Task<ItemDTO> CreateItemAsync(CreateItemDTO item);
    Task<ItemDTO> UpdateItemAsync(long id, UpdateItemDTO item);
    Task<bool> DeleteItemAsync(long id);
}
