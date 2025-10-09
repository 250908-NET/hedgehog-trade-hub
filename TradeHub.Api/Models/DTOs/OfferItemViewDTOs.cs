// returningh items to the frontend
// only for items indide an offer
//returned when you want o display all items included in an offer

namespace TradeHub.API.Models.DTOs
{
    public class OfferItemViewDto
    {
        public long Id { get; set; }
        public long ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public int Quantity { get; set; }
    }
}
