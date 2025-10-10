using TradeHub.Api.Models;
using TradeHub.Api.Repository.Interfaces;
using TradeHub.Api.Services.Interfaces;

namespace TradeHub.Api.Services;

public class TradeService(ITradeRepository repository) : ITradeService
{
    private readonly ITradeRepository _repository =
        repository ?? throw new ArgumentNullException(nameof(repository));

    public async Task<List<Trade>> GetAllTradesAsync()
    {
        return await _repository.GetAllTradesAsync();
    }

    public async Task<List<Trade>> GetTradesByUserAsync(long userId)
    {
        return await _repository.GetTradesByUser(userId);
    }

    public async Task<Trade?> GetTradeByIdAsync(long tradeId)
    {
        return await _repository.GetTradeByIdAsync(tradeId);
    }

    public async Task<Trade> CreateTradeAsync(Trade trade)
    {
        return await _repository.CreateTradeAsync(trade);
    }

    public async Task<Trade?> UpdateTradeAsync(Trade trade)
    {
        return await _repository.UpdateTradeAsync(trade);
    }

    public async Task DeleteTradeAsync(long tradeId)
    {
        await _repository.DeleteTradeAsync(tradeId);
    }

    // implementaions for confirm trade

    public async Task<Trade> ConfirmTradeAsync(long tradeId, long userId)
    {
        var trade =
            await _repository.GetTradeByIdAsync(tradeId) ?? throw new Exception("Trade not found");

        // only trade participants can confirm
        if (userId != trade.InitiatedId && userId != trade.ReceivedId)
            throw new Exception("User not part of this trade");

        // to update the confirmation
        if (userId == trade.InitiatedId)
            trade.InitiatedConfirmed = true;
        else if (userId == trade.ReceivedId)
            trade.ReceivedConfirmed = true;

        return await _repository.UpdateTradeAsync(trade)
            ?? throw new Exception("Failed to update trade");
    }
}
