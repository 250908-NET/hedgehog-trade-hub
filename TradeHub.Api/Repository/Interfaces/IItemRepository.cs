using TradeHub.Api.Models;

namespace TradeHub.Api.Repository.Interfaces;

public interface IItemRepository
{
    Task<List<Item>> GetAllAsync();
    Task<Item?> GetByIdAsync(int id);
    Task AddAsync(Item Item);
    Task SaveChangesAsync();
    Task UpdateAsync(int id, Item updatedItem);
    Task<bool> DeleteAsync(int id);
}
