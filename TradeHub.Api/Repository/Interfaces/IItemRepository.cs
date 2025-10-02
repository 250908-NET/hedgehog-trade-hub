using TradeHub.Api.Models;

namespace TradeHub.Api.Repository.Interfaces;

public interface IItemRepository
{
    Task<List<ItemDTO>> GetAllAsync();
    Task<ItemDTO?> GetByIdAsync(long id);
    Task <ItemDTO>AddAsync(ItemDTO ItemDto);
  
    Task<bool> UpdateAsync(ItemDTO itemDto);
    Task<bool> DeleteAsync(long id);
}
