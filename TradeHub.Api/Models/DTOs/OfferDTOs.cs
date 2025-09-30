using Microsoft.EntityFrameworkCore;
namespace TradeHub.API.Models.DTOs
{
    public class OfferDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TradeId { get; set; }
        public DateTimeOffset Created { get; set; }
    }
}