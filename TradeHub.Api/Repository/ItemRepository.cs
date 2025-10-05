using Microsoft.EntityFrameworkCore;
using TradeHub.Api.Models;
using TradeHub.Api.Repository.Interfaces;

namespace TradeHub.Api.Repository;

public class ItemRepository(TradeHubContext context) : IItemRepository
{
    private readonly TradeHubContext _context = context;

    public async Task<List<Item>> GetAllAsync()
    {
        return await _context.Items.ToListAsync();
    }

    public async Task<Item?> GetByIdAsync(int id)
    {
        return await _context.Items.FindAsync((long)id);
    }

    public async Task<Item> AddAsync(Item item)
    {
        await _context.Items.AddAsync(item);
        return item;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, Item updatedItem)
    {
        var existingItem = await _context.Items.FindAsync((long)id);
        if (existingItem != null)
        {
            existingItem.Description = updatedItem.Description;
            existingItem.Image = updatedItem.Image;
            existingItem.Value = updatedItem.Value;
            existingItem.Owner = updatedItem.Owner;
            existingItem.Tags = updatedItem.Tags;
            
            _context.Items.Update(existingItem);
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _context.Items.FindAsync((long)id);
        if (item != null)
        {
            _context.Items.Remove(item);
            return true;
        }
        return false;
    }
}
