using TradeHub.API.Models;

namespace TradeHub.API.Services.Interfaces;

public interface IUserService
{
    Task<User> GetUserByIdAsync(long id);
    Task<User> CreateUserAsync(User user);
    Task<bool> UpdateUserAsync(long id, User user);
    Task<bool> DeleteUserAsync(long id);
    Task<IEnumerable<User>> GetAllUsersAsync();
}
