// Services/AdminUserService.cs
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using TradeHub.Api.Models;
using TradeHub.Api.Models.DTOs;
using TradeHub.Api.Repository.Interfaces;

namespace TradeHub.Api.Services;

public sealed class AdminUserService(IUserRepository users, IPasswordHasher<User> hasher)
    : IAdminUserService
{
    private readonly IUserRepository _users = users;
    private readonly IPasswordHasher<User> _hasher = hasher;

    public async Task<IEnumerable<UserDTO>> GetAllAsync(ClaimsPrincipal actor)
    {
        EnsureAdmin(actor);
        var all = await _users.GetAllAsync();
        return all.Select(MapToDto);
    }

    public async Task<UserDTO?> GetByIdAsync(long id, ClaimsPrincipal actor)
    {
        EnsureAdmin(actor);
        var user = await _users.GetByIdAsync(id);
        return user is null ? null : MapToDto(user);
    }

    public async Task<UserDTO> CreateAsync(CreateUserDTO dto, ClaimsPrincipal actor)
    {
        EnsureAdmin(actor);

        if (string.IsNullOrWhiteSpace(dto.UserName) || dto.UserName.Length > 32)
            throw new ArgumentException("Username is required and must be ≤ 32 characters.");
        if (string.IsNullOrWhiteSpace(dto.Email) || !dto.Email.Contains('@'))
            throw new ArgumentException("A valid Email is required.");
        if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 8)
            throw new ArgumentException("Password must be at least 8 characters.");

        if (await _users.GetByUsernameAsync(dto.UserName) is not null)
            throw new InvalidOperationException("Username is already taken.");
        var existsEmail = (await _users.GetAllAsync()).Any(u =>
            (u.Email ?? "").Equals(dto.Email, StringComparison.OrdinalIgnoreCase)
        );
        if (existsEmail)
            throw new InvalidOperationException("Email is already in use.");

        var user = new User
        {
            UserName = dto.UserName.Trim(),
            Email = dto.Email.Trim(),
            Description = dto.Description?.Trim() ?? "",
        };
        user.PasswordHash = _hasher.HashPassword(user, dto.Password);

        await _users.AddAsync(user);
        return MapToDto(user);
    }

    public async Task<UserDTO?> UpdateAsync(long id, UpdateUserDTO dto, ClaimsPrincipal actor)
    {
        EnsureAdmin(actor);
        var user = await _users.GetByIdAsync(id);
        if (user is null)
            return null;

        if (
            !string.IsNullOrWhiteSpace(dto.UserName)
            && !dto.UserName.Equals(user.UserName, StringComparison.Ordinal)
        )
        {
            var taken = await _users.GetByUsernameAsync(dto.UserName);
            if (taken is not null && taken.Id != user.Id)
                throw new InvalidOperationException("Username is already taken.");
            user.UserName = dto.UserName.Trim();
        }

        if (
            !string.IsNullOrWhiteSpace(dto.Email)
            && !dto.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)
        )
        {
            var existsEmail = (await _users.GetAllAsync()).Any(u =>
                (u.Email ?? "").Equals(dto.Email, StringComparison.OrdinalIgnoreCase)
                && u.Id != user.Id
            );
            if (existsEmail)
                throw new InvalidOperationException("Email is already in use.");
            user.Email = dto.Email.Trim();
        }

        if (dto.Description is not null)
            user.Description = dto.Description.Trim();
        if (!string.IsNullOrWhiteSpace(dto.Password))
            user.PasswordHash = _hasher.HashPassword(user, dto.Password);

        await _users.UpdateAsync(user);
        return MapToDto(user);
    }

    public async Task<bool> DeleteAsync(long id, ClaimsPrincipal actor)
    {
        EnsureAdmin(actor);

        var idClaim = actor.FindFirstValue(ClaimTypes.NameIdentifier);
        if (long.TryParse(idClaim, out var actorId) && actorId == id)
            throw new InvalidOperationException("Admins cannot delete themselves.");

        return await _users.DeleteAsync(id);
    }

    private static void EnsureAdmin(ClaimsPrincipal actor)
    {
        if (!actor.IsInRole("Admin"))
            throw new UnauthorizedAccessException("Admin privileges required.");
    }

    private static UserDTO MapToDto(User u) =>
        new()
        {
            Id = u.Id,
            UserName = u.UserName ?? "",
            Description = u.Description,
            Email = u.Email ?? "",
        };
}
