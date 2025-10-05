using TradeHub.Api.Models;
using TradeHub.Api.Repository.Interfaces;
namespace TradeHub.Api.Services;
public class ItemService {
    private readonly IItemRepository _repo;
    public ItemService(IItemRepository repo) {
        if (repo == null) { throw new ArgumentNullException(nameof(repo)); }
        _repo = repo;
    }
    public async Task<List<Item>> GetAllAsync() => await _repo.GetAllAsync();
    public async Task<Item?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);
    public async Task<Item> CreateAsync(Item Item)
    {
        Item createdItem = await _repo.AddAsync(Item);
        return createdItem;
    }
    public async Task UpdateAsync(int id, Item course)
    {
        await _repo.UpdateAsync(id, course);
    }
    public async Task DeleteAsync(int id)
    {
        await _repo.DeleteAsync(id);
    }
    // public async  Task<bool> Exists(int id) => await _repo.Exists(id);
}