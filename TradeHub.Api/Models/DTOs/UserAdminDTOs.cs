// Models/DTOs/UserAdminDTOs.cs
namespace TradeHub.Api.Models.DTOs;

public sealed class CreateUserDTO
{
    public required string UserName { get; set; }
    public string? Description { get; set; } = "";
    public required string Email { get; set; }
    public required string Password { get; set; }
    public byte Role { get; set; } = 0; // e.g., 1 = Admin
}

public sealed class UpdateUserDTO
{
    public string? UserName { get; set; }
    public string? Description { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public byte? Role { get; set; }
}
