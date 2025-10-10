namespace TradeHub.API.Models.DTOs;

public class UserDTO
{
    public long Id { get; set; }
    public required string Username { get; set; }
    public string Description { get; set; } = "";
    public string Email { get; set; } = "";
    public byte Role { get; set; }
}
