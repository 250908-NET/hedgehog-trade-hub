using Microsoft.EntityFrameworkCore;
using TradeHub.Api.Models;
using TradeHub.Api.Repository.Interfaces;

namespace TradeHub.Api.Repository;

public class ItemRepository(TradeHubContext context) : IItemRepository
{
    private readonly TradeHubContext _context = context;


//get all items
    public async Task<List<Item>> GetAllAsync()
    {
        return await _context.Items.ToListAsync();
    }


//get item by id
    public async Task<Item?> GetByIdAsync(int id)
    {
        return await _context.Items.FindAsync((long)id);
    }

// add an item
    public async Task AddAsync(Item item)
    {
        await _context.Items.AddAsync(item);
        await _context.SaveChangesAsync();
    }

// update item
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
            existingItem.Condition = updatedItem.Condition;
            existingItem.Availability = updatedItem.Availability;

            _context.Items.Update(existingItem);
        }
    }


// delete
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
