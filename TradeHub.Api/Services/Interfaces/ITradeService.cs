using TradeHub.API.Models;

namespace TradeHub.API.Services.Interfaces;

public interface ITradeService
{
    Task<List<Trade>> GetAllTradesAsync();
    Task<List<Trade>> GetTradesByUserAsync(int userId);
    Task<Trade?> GetTradeByIdAsync(int tradeId);
    Task<Trade> CreateTradeAsync(Trade trade);
    Task<Trade?> UpdateTradeAsync(Trade trade);
    Task DeleteTradeAsync(int tradeId);

    // method for confirm the trade
    Task<Trade> ConfirmTradeAsync(int tradeId, int userId);
}
