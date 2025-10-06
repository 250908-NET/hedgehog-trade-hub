using Microsoft.EntityFrameworkCore;
using TradeHub.Api.Models;
using TradeHub.Api.Repository.Interfaces;

namespace TradeHub.Api.Repository;

public class TradeRepository(TradeHubContext context) : ITradeRepository
{
    private readonly TradeHubContext _context = context;

    public async Task<Trade> CreateTradeAsync(Trade trade)
    {
        _context.Trades.Add(trade);
        await _context.SaveChangesAsync(trade);
        return trade;
    }

    public async Task DeleteTradeAsync(long tradeId)
    {
        var trade = await _context.Trades.FindAsync(tradeId);
        _context.Trades.Remove(trade);
        await _context.SaveChangesAsync();
    }

    
    public async Task<Trade?> GetTradeByIdAsync(long tradeId)
    {
        return await _context
            .Trades.Include(t => t.InitiatedUser)
            .Include(t => t.ReceivedUser)
            .FirstOrDefaultAsync(t => t.Id == tradeId);
    }

    // public async Task<List<Trade>> GetTradesByUser(long userId)
    // {
    //     return await _context
    //         .Trades.Include(t => t.TradeItems)
    //         .Where(t => t.InitiatedId == userId || t.ReceivedId == userId)
    //         .ToListAsync();
    // }

    public async Task<Trade?> UpdateTradeAsync(Trade trade)
    {
        var exisitingTrade = await _context.Trades.FindAsync(trade.Id);
        if (exisitingTrade == null)
            return null;

        _context.Entry(exisitingTrade).CurrentValues.SetValues(trade);

        return exisitingTrade;
    }

    public async Task<Trade?> AcceptTradeAsync(int tradeId)
    {
        var trade = await _context.Trades.FindAsync(tradeId);
        if (trade is null)
        {
            return null;
        }

        trade.Status = TradeStatuses.Accepted;
        await _context.SaveChangesAsync();
        return trade;
    }

    public Task<List<Trade>> GetAllTradesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<Trade>> GetTradesByUser(long userId)
    {
        throw new NotImplementedException();
    }

    public Task<Trade?> GetTradeByIdAsync(long tradeId)
    {
        throw new NotImplementedException();
    }

    public Task DeleteTradeAsync(long tradeId)
    {
        throw new NotImplementedException();
    }
}
