

using TradeHub.Api.Models;

namespace TradeHub.Api.Services.Interfaces
{
    public interface ITradeService
    {
        Task<Trade?> AcceptAsync(long tradeId);
    }
}