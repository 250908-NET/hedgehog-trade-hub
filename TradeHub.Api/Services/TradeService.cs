using TradeHub.API.Models;
using TradeHub.API.Repository.Interfaces;
using TradeHub.API.Services.Interfaces;

namespace TradeHub.API.Services;

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
}