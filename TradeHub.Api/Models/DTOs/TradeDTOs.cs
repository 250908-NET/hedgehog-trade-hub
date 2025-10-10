namespace TradeHub.API.Models.DTOs
{
    public class TradeDTO
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public long InitiateId { get; set; }
        public long ReceiveId { get; set; }
        public byte Status { get; set; }

        public string ItemCondition { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public int OwnerReputation { get; set; }

        public List<ItemDTO>? TradeItems { get; set; }
        public List<OfferDTO>? Offers { get; set; }
    }
}
