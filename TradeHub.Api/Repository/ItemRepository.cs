using TradeHub.Api.Models;
using TradeHub.Api.Repository.Interfaces;

namespace TradeHub.Api.Repository;

public class ItemRepository(TradeHubContext context) // : IItemRepository
{
    private readonly TradeHubContext _context = context;

    Task<List<Item>> GetAllAsync(TradeHubContext context)
    {
        throw new NotImplementedException();
    }

    Task<Item?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    Task AddAsync(Item Item)
    {
        throw new NotImplementedException();
    }

    Task SaveChangesAsync()
    {
        throw new NotImplementedException();
    }

    Task UpdateAsync(int id, Item updatedItem)
    {
        throw new NotImplementedException();
    }

    Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}
