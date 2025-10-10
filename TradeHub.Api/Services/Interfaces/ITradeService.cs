using TradeHub.Api.Models;

namespace TradeHub.Api.Services.Interfaces;

public interface ITradeService
{
    Task<List<Trade>> GetAllTradesAsync();
    Task<List<Trade>> GetTradesByUserAsync(long userId);
    Task<Trade?> GetTradeByIdAsync(long tradeId);
    Task<Trade> CreateTradeAsync(Trade trade);
    Task<Trade?> UpdateTradeAsync(Trade trade);
    Task DeleteTradeAsync(long tradeId);
    Task<Trade> ConfirmTradeAsync(long tradeId, long userId);
}
