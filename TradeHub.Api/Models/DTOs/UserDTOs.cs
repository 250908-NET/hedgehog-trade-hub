namespace TradeHub.Api.Models.DTOs;

public class UserDTO
{
    public long Id { get; set; }
    public required string Username { get; set; }
    public required string Description { get; set; }
    public required string Email { get; set; }
    public byte Role { get; set; }
}
