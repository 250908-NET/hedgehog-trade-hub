using TradeHub.Api.Models;

namespace TradeHub.Api.Repository.Interfaces;

public interface IItemRepository
{
    Task<List<Item>> GetAllAsync();
    Task<Item?> GetByIdAsync(long id);
    Task<Item> AddAsync(Item ItemDto);

    Task<bool> UpdateAsync(Item itemDto);
    Task<bool> DeleteAsync(long id);
}
