

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using TradeHub.API.Models;

namespace TradeHub.Api.Models;

public class User
{
    [Key]
    public long Id { get; set; }

    [Required]
    [MaxLength(32)]
    public string Username { get; set; }

    [Column(TypeName = "text")]
    public string Description { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    [MaxLength(60)]
    [Column(TypeName = "char(60)")]
    public string PasswordHash { get; set; }

    public byte Role { get; set; }

    // Navigation properties
    public ICollection<Item> OwnedItems { get; set; }
    public ICollection<Trace> InitiatedTrades { get; set; }
    public ICollection<Trade> ReceivedTrades { get; set; }
    public ICollection<Offer> Offers { get; set; }
}
