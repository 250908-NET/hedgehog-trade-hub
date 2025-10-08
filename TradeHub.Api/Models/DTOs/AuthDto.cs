using System.ComponentModel.DataAnnotations;

namespace TradeHub.DTO
{
    public class RegisterUserDto
{
    [Required]
    [MaxLength(32)]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    public string Description { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }
}
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class AuthResponseDto
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
    }
}