// Services/IAdminUserService.cs
using System.Security.Claims;
using TradeHub.Api.Models.DTOs;   // ✅ the ONLY DTO namespace

namespace TradeHub.Api.Services
{
    public interface IAdminUserService
    {
        Task<IEnumerable<UserDTO>> GetAllAsync(ClaimsPrincipal actor);
        Task<UserDTO?> GetByIdAsync(long id, ClaimsPrincipal actor);
        Task<UserDTO> CreateAsync(CreateUserDTO dto, ClaimsPrincipal actor);
        Task<UserDTO?> UpdateAsync(long id, UpdateUserDTO dto, ClaimsPrincipal actor);
        Task<bool> DeleteAsync(long id, ClaimsPrincipal actor);
    }
}
