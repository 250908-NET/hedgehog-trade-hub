
//one offer can have multiple offers

using System.ComponentModel.DataAnnotations;

namespace TradeHub.API.Models
{
    public class OfferItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OfferId { get; set; }
        public Offer Offer { get; set; } = null!; // Navigation property

        [Required]
        public int ItemId { get; set; }
        public Item Item { get; set; } = null!; // Navigation property

        [Required]
        public int Quantity { get; set; } = 1;

        public string? Notes { get; set; }
    }
}
