using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TradeHub.API.Models
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

        public byte Status { get; set; }


          // Trade review details
        [Required]
        public string ItemCondition { get; set; } = string.Empty;

        public string? Notes { get; set; }

        public int OwnerReputation { get; set; }


        // Navigation properties
        [ForeignKey("InitiatedId")]
        public User? InitiatedUser { get; set; }

        [ForeignKey("ReceivedId")]
        public User? ReceivedUser { get; set; }

        public ICollection<Item>? TradeItems { get; set; } = new List<Item>();
        public ICollection<Offer>? Offers { get; set; } = new List<Offer>();

      

        
    }
}
