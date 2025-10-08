using TradeHub.API.Models;

namespace TradeHub.API.Repository.Interfaces;

public interface IUserRepository
{
    Task<User> GetByIdAsync(long id);
    Task<User> GetByUsernameAsync(string username);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task<bool> DeleteAsync(long id);
    Task<IEnumerable<User>> GetAllAsync();
    Task<bool> ExistsAsync(long id);
}
