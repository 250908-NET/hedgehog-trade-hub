using Microsoft.EntityFrameworkCore;
using TradeHub.Api.Models;
using TradeHub.Api.Repository.Interfaces;
using TradeHub.Api.Utilities;

namespace TradeHub.Api.Repository;

public class ItemRepository(TradeHubContext context) : IItemRepository
{
    private readonly TradeHubContext _context = context;

    /// <summary>
    /// Get all items
    /// </summary>
    /// <returns>A list containing all items</returns>
    public async Task<List<Item>> GetAllAsync()
    {
        return await _context.Items.AsNoTracking().ToListAsync();
    }

    /// <summary>
    /// Get item by id
    /// </summary>
    /// <param name="id">The id of the item to find</param>
    /// <returns>The item if found, otherwise null</returns>
    public async Task<Item?> GetByIdAsync(long id)
    {
        return await _context.Items.FindAsync(id);
    }

    /// <summary>
    /// Create a new item
    /// </summary>
    /// <param name="newItem">A new item, not including id</param>
    /// <returns>The created item, including id</returns>
    public async Task<Item> CreateAsync(Item newItem)
    {
        await _context.Items.AddAsync(newItem);
        await _context.SaveChangesAsync();

        await _context.Entry(newItem).ReloadAsync();
        return newItem;
    }

    /// <summary>
    /// Update item, replacing all fields with those of the provided item
    /// </summary>
    /// <param name="updatedItem">An item with updated fields</param>
    /// <returns>True if successful, false if no update was made</returns>
    /// <exception cref="ConflictException">Thrown if the item was modified by another user</exception>
    public async Task<bool> UpdateAsync(Item updatedItem)
    {
        _context.Entry(updatedItem).State = EntityState.Modified;
        try
        {
            return await _context.SaveChangesAsync() > 0;
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConflictException(
                "The item you are trying to update has been modified by another user. Please refresh and try again."
            );
        }
    }

    /// <summary>
    /// Delete existing item
    /// </summary>
    /// <param name="id">The </param>
    /// <returns></returns>
    public async Task<bool> DeleteAsync(Item item)
    {
        _context.Items.Remove(item);
        try
        {
            return await _context.SaveChangesAsync() > 0;
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConflictException(
                "The item you are trying to delete has been modified by another user. Please refresh and try again."
            );
        }
    }
}
