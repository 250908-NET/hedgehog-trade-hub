using Microsoft.EntityFrameworkCore;
namespace TradeHub.API.Models.DTOs
{
    public class OfferDTO
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        public long TradeId { get; set; }
        public DateTimeOffset Created { get; set; }
    }
}