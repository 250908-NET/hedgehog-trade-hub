using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace TradeHub.API.Models;

public class User : IdentityUser<long>
{
    [Column(TypeName = "text")]
    public string Description { get; set; } = "";

    // Navigation properties
    public ICollection<Item> OwnedItems { get; set; } = [];
    public ICollection<Trade> InitiatedTrades { get; set; } = [];
    public ICollection<Trade> ReceivedTrades { get; set; } = [];
    public ICollection<Offer> Offers { get; set; } = [];
}
