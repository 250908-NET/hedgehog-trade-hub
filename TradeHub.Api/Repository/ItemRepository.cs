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
    public async Task<List<Item>> GetAllAsync(
        int page = 1,
        int pageSize = 10,
        decimal? minValue = null,
        decimal? maxValue = null,
        Condition? condition = null,
        Availability? availability = null,
        string? search = null
    )
    {
        IQueryable<Item> query = _context.Items.AsNoTracking().AsQueryable();

        // apply filters
        if (minValue != null)
            query = query.Where(i => i.Value >= minValue.Value);
        if (maxValue != null)
            query = query.Where(i => i.Value <= maxValue.Value);
        if (condition != null)
            query = query.Where(i => i.Condition == condition.Value);
        if (availability != null)
            query = query.Where(i => i.Availability == availability.Value);
        if (!string.IsNullOrWhiteSpace(search))
        {
            string cleanedSearch = search.Trim().ToLower();
            query = query.Where(i =>
                // must use without overload for ef core to recognize
                i.Name.ToLower().Contains(cleanedSearch)
                || i.Description.ToLower().Contains(cleanedSearch)
                || i.Tags.ToLower().Contains(cleanedSearch)
            );
        }

        // apply pagination
        int skip = (Math.Max(1, page) - 1) * Math.Max(1, pageSize);
        query = query.Skip(skip).Take(pageSize);

        return await query.ToListAsync();
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
    public async Task<Item> UpdateAsync(Item updatedItem)
    {
        // check if item exists
        Item? item =
            await _context.Items.FindAsync(updatedItem.Id)
            ?? throw new NotFoundException("Item not found.");

        // update existing item
        _context.Entry(item).CurrentValues.SetValues(updatedItem);
        _context.Entry(item).Property(i => i.RowVersion).OriginalValue = updatedItem.RowVersion; // preserve concurrency from original

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConflictException(
                "The item you are trying to update has been modified by another user. Please refresh and try again."
            );
        }

        return item;
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
