using TradeHub.Api.Models;

namespace TradeHub.Api.Repository.Interfaces;

public interface ITradeRepository
{
    Task<List<Trade>> GetAllTradesAsync();
    Task<List<Trade>> GetTradesByUser(long userId);
    Task<Trade?> GetTradeByIdAsync(long tradeId);
    Task<Trade> CreateTradeAsync(Trade trade);
    Task<Trade> UpdateTradeAsync(Trade trade);
    Task DeleteTradeAsync(long tradeId);
    Task<Trade?> AcceptTradeAsync(long tradeId);
}
