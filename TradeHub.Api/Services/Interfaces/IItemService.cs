using TradeHub.Api.Models;

namespace TradeHub.Api.Services

public interface IItemService{
        public Task<List<Item>> GetAllAsync();
        public Task<Item?> GetByIdAsync(int id);
        public Task<Item> CreateAsync(Item Item);
        public Task DeleteAsync(int id);
        public Task UpdateAsync(int id, Item Item);
        public Task<bool> Exists(int id);
}