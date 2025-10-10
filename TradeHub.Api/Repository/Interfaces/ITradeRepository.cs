using TradeHub.API.Models;

namespace TradeHub.API.Repository.Interfaces;

public interface ITradeRepository
{
    Task<List<Trade>> GetAllTradesAsync();
    Task<List<Trade>> GetTradesByUser(int userId);
    Task<Trade?> GetTradeByIdAsync(int tradeId);
    Task<Trade> CreateTradeAsync(Trade trade);
    Task<Trade?> UpdateTradeAsync(Trade trade);
    Task DeleteTradeAsync(int tradeId);

    Task<List<Trade>> GetTradesByStatusAsync(TradeStatus status);
}
