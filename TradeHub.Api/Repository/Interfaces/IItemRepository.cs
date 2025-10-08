using TradeHub.Api.Models;

namespace TradeHub.API.Repository.Interfaces;

public interface IItemRepository
{
    Task<List<Item>> GetAllAsync(
        int page,
        int pageSize,
        decimal? minValue,
        decimal? maxValue,
        Condition? condition,
        Availability? availability,
        string? search
    );
    Task<Item?> GetByIdAsync(long id);
    Task<Item> CreateAsync(Item newItem);
    Task<Item> UpdateAsync(Item item);
    Task<bool> DeleteAsync(Item item);
}
