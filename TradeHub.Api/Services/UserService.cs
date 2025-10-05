using TradeHub.Api.Models;
using TradeHub.Api.Repository.Interfaces;
using TradeHub.Api.Services.Interfaces;

namespace TradeHub.Api.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }
    public async Task<User> CreateUserAsync(User user)
    {
        await _repository.AddAsync(user);
        return user;
    }

    public async Task<bool> DeleteUserAsync(long id)
    {
        if (!await _repository.ExistsAsync(id)) return false;
        await _repository.DeleteAsync(id);
        return true;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<User> GetUserByIdAsync(long id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<bool> UpdateUserAsync(long id, User user)
    {
        if (!await _repository.ExistsAsync(id)) return false;

        await _repository.UpdateAsync(user);
        return true;
    }
}