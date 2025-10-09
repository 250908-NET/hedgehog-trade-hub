namespace TradeHub.Api.Models.DTOs;

public class TradeDTO
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public long InitiateId { get; set; }
    public long ReceiveId { get; set; }
    public byte Status { get; set; }
}