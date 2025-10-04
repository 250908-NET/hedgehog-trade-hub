using TradeHub.Api.Models;

namespace TradeHub.Api.Repository.Interfaces;

public interface IItemRepository
{
    Task<List<Item>> GetAllAsync();
    Task<Item?> GetByIdAsync(long id);
    Task<Item> CreateAsync(Item newItem);
    Task<bool> UpdateAsync(Item item);
    Task<bool> DeleteAsync(Item item);
}
