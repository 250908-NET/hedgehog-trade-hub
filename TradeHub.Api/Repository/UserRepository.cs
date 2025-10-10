using Microsoft.EntityFrameworkCore;
using TradeHub.API.Models;
using TradeHub.API.Repository.Interfaces;

public class UserRepository : IUserRepository
{
    private readonly TradeHubContext _context;

    public UserRepository(TradeHubContext context)
    {
        _context = context;
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var user = await GetByIdAsync(id);

        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await _context.Users.AnyAsync(user => user.Id == id);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> GetByIdAsync(long id)
    {
        return await _context
            .Users.Include(u => u.OwnedItems)
            .Include(u => u.InitiatedTrades)
            .Include(u => u.ReceivedTrades)
            .Include(u => u.Offers)
            .FirstOrDefaultAsync(user => user.Id == id);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
}
