using TradeHub.Api.Models;

namespace TradeHub.Api.Repository.Interfaces;

public interface ITradeRepository
{
    Task<List<Trade>> GetAllTradesAsync();
    Task<List<Trade>> GetTradesByUser(int userId);
    Task<Trade?> GetTradeByIdAsync(int tradeId);
    Task<Trade> CreateTradeAsync(Trade trade);
    Task<Trade> UpdateTradeAsync(Trade trade);
    Task DeleteTradeAsync(int tradeId);

     // New method for completing a trade
        Task<bool> MarkTradeAsCompletedAsync(long tradeId);
}
