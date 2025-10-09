using System.ComponentModel.DataAnnotations;

namespace TradeHub.Api.Models.DTOs;

public class RegisterUserDTO
{
    [Required]
    [MaxLength(32)]
    public required string Username { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    public required string Description { get; set; }

    [Required]
    [MinLength(6)]
    public required string Password { get; set; }
}

public class LoginDTO
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class AuthResponseDTO
{
    public required string Token { get; set; }
    public required string Email { get; set; }
    public required List<string> Roles { get; set; }
}