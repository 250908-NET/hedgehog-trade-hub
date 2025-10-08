using TradeHub.Api.Models;
using TradeHub.Api.Repository.Interfaces;
using TradeHub.Api.Services.Interfaces;

namespace TradeHub.Api.Services;

public class TradeService : ITradeService
{
    private readonly ITradeRepository _repository;

    public TradeService(ITradeRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<List<Trade>> GetAllTradesAsync()
    {
        return await _repository.GetAllTradesAsync();
    }

    public async Task<List<Trade>> GetTradesByUserAsync(int userId)
    {
        return await _repository.GetTradesByUser(userId);
    }

    public async Task<Trade?> GetTradeByIdAsync(int tradeId)
    {
        return await _repository.GetTradeByIdAsync(tradeId);
    }

    public async Task<Trade> CreateTradeAsync(Trade trade)
    {
        return await _repository.CreateTradeAsync(trade);
    }

    public async Task<Trade> UpdateTradeAsync(Trade trade)
    {
        return await _repository.UpdateTradeAsync(trade);
    }

    public async Task DeleteTradeAsync(int tradeId)
    {
        await _repository.DeleteTradeAsync(tradeId);
    }

    // to complete the trade

public async Task<bool> ConfirmTradeCompletionAsync(long tradeId, long userId)
{
    var trade = await _tradeRepository.GetTradeByIdAsync(tradeId);
    if (trade == null) return false;

    // Track confirmations
    // Example: using Status as flags: 0 = pending, 1 = initiated confirmed, 2 = received confirmed, 3 = both confirmed (completed

    if (trade.InitiatedId == userId)
    {
        trade.Status |= 1; 
    }
    else if (trade.ReceivedId == userId)
    {
        trade.Status |= 2;
    }
    else
    {
        return false; // user not part of trade
    }

    // If both bits set, mark as completed (status = 3)
    if (trade.Status == 3)
    {
        // fully completed
    }

    await _tradeRepository.UpdateTradeAsync(trade);
    return true;
}

}