// to send trade, offers, items to the  frontend

namespace TradeHub.Api.Models.DTOs
{
    public class ReceivedOfferDTO
    {
        public long OfferId { get; set; }
        public long TradeId { get; set; }
        public string TradeItemCondition { get; set; } = string.Empty;
        public string? TradeNotes { get; set; }
        public int OwnerReputation { get; set; }
        public DateTimeOffset OfferCreated { get; set; }

        // Items included in the offer
        public List<OfferItemViewDto> Items { get; set; } = [];

        //status, e.g., Pending, Accepted, Rejected
        public TradeStatus Status { get; set; }
    }
}
