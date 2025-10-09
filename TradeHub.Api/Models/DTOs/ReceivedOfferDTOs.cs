// to send trade, offers, items to the  frontend

namespace TradeHub.API.Models.DTOs
{
    public class ReceivedOfferDto
    {
        public int OfferId { get; set; }
        public long TradeId { get; set; }
        public string TradeItemCondition { get; set; } = string.Empty;
        public string? TradeNotes { get; set; }
        public int OwnerReputation { get; set; }
        public DateTimeOffset OfferCreated { get; set; }

        // Items included in the offer
        public List<OfferItemViewDto> Items { get; set; } = new List<OfferItemViewDto>();

        //status, e.g., Pending, Accepted, Rejected
        public byte Status { get; set; }
    }
}
