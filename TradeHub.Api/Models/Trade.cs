using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradeHub.API.Models;

namespace TradeHub.Api.Models
{

    public static class TradeStatuses
    {
        public const byte Pending = 0;
        public const byte Accepted = 1;
        public const byte Rejected = 2;
        public const byte Cancelled = 3;
        public const byte Completed = 4;
    }
    public class Trade
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public long InitiatedId { get; set; }

        [Required]
        public long ReceivedId { get; set; }

        public byte Status { get; set; } = TradeStatuses.Pending;


        // Navigation properties
        [ForeignKey(nameof(InitiatedId))]
        public User InitiatedUser { get; set; }

        [ForeignKey(nameof(ReceivedId))]
        public User ReceivedUser { get; set; }

        public ICollection<Offer> Offers { get; set; } = new List<Offer>();

    

    }
}
