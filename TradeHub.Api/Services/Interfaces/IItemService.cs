using TradeHub.API.Models;
using TradeHub.API.Models.DTOs;

namespace TradeHub.API.Services.Interfaces;

public interface IItemService
{
    Task<List<ItemDTO>> GetAllItemsAsync(
        int page,
        int pageSize,
        decimal? minValue,
        decimal? maxValue,
        Condition? condition,
        Availability? availability,
        string? search
    );
    Task<ItemDTO> GetItemByIdAsync(long id);
    Task<ItemDTO> CreateItemAsync(CreateItemDTO dto);
    Task<ItemDTO> UpdateItemAsync(long id, UpdateItemDTO dto);
    Task<bool> DeleteItemAsync(long id);
}
