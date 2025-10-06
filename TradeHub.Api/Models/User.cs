using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TradeHub.Api.Models;

public class User
{
    [Key]
    public long Id { get; set; }

    [Required]
    [MaxLength(32)]
    public string Username { get; set; } = null!;

    [Column(TypeName = "text")]
    public string Description { get; set; } = "";

    [Required]
    public string Email { get; set; } = null!;

    [Required]
    [MaxLength(60)]
    [Column(TypeName = "char(60)")]
    public string PasswordHash { get; set; } = null!;

    public byte Role { get; set; }

    // Navigation properties
    public ICollection<Item> OwnedItems { get; set; } = [];
    public ICollection<Trade> InitiatedTrades { get; set; } = [];
    public ICollection<Trade> ReceivedTrades { get; set; } = [];
    public ICollection<Offer> Offers { get; set; } = [];
}
