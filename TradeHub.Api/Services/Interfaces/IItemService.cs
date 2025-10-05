using TradeHub.Api.Models.DTOs;

namespace TradeHub.Api.Services.Interfaces;

public interface IItemService
{
    Task<List<ItemDTO>> GetAllItemsAsync();
    Task<ItemDTO> GetItemByIdAsync(long id);
    Task<ItemDTO> CreateItemAsync(CreateItemDTO dto);
    Task<ItemDTO> UpdateItemAsync(long id, UpdateItemDTO dto);
    Task<bool> DeleteItemAsync(long id);
}
