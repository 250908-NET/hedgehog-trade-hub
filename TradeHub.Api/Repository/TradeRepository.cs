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
        await _context.SaveChangesAsync();
        return trade;
    }

    public async Task DeleteTradeAsync(long tradeId)
    {
        var trade = await _context.Trades.FindAsync(tradeId);

        // DELETE is idempotent, so don't throw an error if the trade doesn't exist
        if (trade == null)
            return;

        _context.Trades.Remove(trade);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Trade>> GetAllTradesAsync()
    {
        return await _context.Trades.Include(t => t.TradeItems).ToListAsync();
    }

    public async Task<Trade?> GetTradeByIdAsync(long tradeId)
    {
        return await _context
            .Trades.Include(t => t.InitiatedUser)
            .Include(t => t.ReceivedUser)
            .FirstOrDefaultAsync(t => t.Id == tradeId);
    }

    public async Task<List<Trade>> GetTradesByUser(long userId)
    {
        return await _context
            .Trades.Include(t => t.TradeItems)
            .Where(t => t.InitiatedId == userId || t.ReceivedId == userId)
            .ToListAsync();
    }

    public async Task<Trade?> UpdateTradeAsync(Trade trade)
    {
        var exisitingTrade = await _context.Trades.FindAsync(trade.Id);
        if (exisitingTrade == null)
            return null;

        _context.Entry(exisitingTrade).CurrentValues.SetValues(trade);

        return exisitingTrade;
    }

    // // Implementation for completing a trade
    // public async Task<bool> MarkTradeAsCompletedAsync(long tradeId)
    // {
    //     var trade = await _context.Trades.FindAsync(tradeId);
    //     if (trade == null)
    //         return false;

    //     trade.Status = 1; // 1 = completed
    //     _context.Trades.Update(trade);
    //     await _context.SaveChangesAsync();
    //     return true;
    // }

    public async Task<List<Trade>> GetTradesByStatusAsync(
        bool initiatedConfirmed,
        bool receivedConfirmed
    )
    {
        return await _context
            .Trades.Include(t => t.TradeItems)
            .Include(t => t.InitiatedUser)
            .Include(t => t.ReceivedUser)
            .Include(t => t.Offers)
            .Where(t =>
                t.InitiatedConfirmed == initiatedConfirmed
                && t.ReceivedConfirmed == receivedConfirmed
            )
            .ToListAsync();
    }
}
