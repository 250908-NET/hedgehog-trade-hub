// for adding items to the offer
namespace TradeHub.Api.Models.DTOs
{
    public class OfferItemCreateDto
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; } = 1;
        public string? Notes { get; set; }
    }
}
