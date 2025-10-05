using TradeHub.Api.Models;

namespace TradeHub.Api.Services.Interfaces;

public interface ITradeService
{
    Task<List<Trade>> GetAllTradesAsync();
    Task<List<Trade>> GetTradesByUserAsync(int userId);
    Task<Trade?> GetTradeByIdAsync(int tradeId);
    Task<Trade> CreateTradeAsync(Trade trade);
    Task<Trade> UpdateTradeAsync(Trade trade);
    Task DeleteTradeAsync(int tradeId);
}
