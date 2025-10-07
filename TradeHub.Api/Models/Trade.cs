using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TradeHub.Api.Models
{
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

        public TradeStatus Status { get; set; } = TradeStatus.Pending;

        // for tracking completion status
        public bool InitiatedConfirmed { get; set; } = false;
        public bool ReceivedConfirmed { get; set; } = false;

        [NotMapped]
        public bool IsCompleted => InitiatedConfirmed && ReceivedConfirmed;

        // Navigation properties
        [ForeignKey("InitiatedId")]
        public User? InitiatedUser { get; set; }

        [ForeignKey("ReceivedId")]
        public User? ReceivedUser { get; set; }

        public ICollection<Item>? TradeItems { get; set; }
        public ICollection<Offer> ? Offers { get; set; }
    }
}
