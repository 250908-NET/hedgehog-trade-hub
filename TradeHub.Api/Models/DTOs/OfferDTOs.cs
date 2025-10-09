namespace TradeHub.API.Models.DTOs
{
    // to create an offer
    public class CreateOfferDTO
    {
        public long UserId { get; set; }
        public long TradeId { get; set; }

        public string? ItemOffered { get; set; }
        public string? Notes { get; set; }
    }

    // DTO for returning the offer details
    //related with - who created it what trade belongs too, notes
    // not include any item details

    public class OfferDTO
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long TradeId { get; set; }
        public string? ItemOffered { get; set; }
        public string? Notes { get; set; }
        public DateTimeOffset Created { get; set; }

        // NEW: include items in the offer
        public List<OfferItemViewDto> Items { get; set; } = new List<OfferItemViewDto>();
    }
}
