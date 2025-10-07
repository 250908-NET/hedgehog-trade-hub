using Microsoft.EntityFrameworkCore;
using TradeHub.Api.Models;
using TradeHub.Api.Repository.Interfaces;

namespace TradeHub.Api.Repository;

public class TradeRepository : ITradeRepository
{
    private readonly TradeHubContext _context;
    public TradeRepository(TradeHubContext context)
    {
        _context = context;
    }


 public async Task<Trade> CreateTradeAsync(Trade trade)
    {
        _context.Trades.Add(trade);
        await _context.SaveChangesAsync();
        return trade;
    }

    public async Task DeleteTradeAsync(int tradeId)
    {
        var trade = await _context.Trades.FindAsync(tradeId);
        if (trade == null)
        {
            throw new KeyNotFoundException($"Trade with Id {tradeId} not found.");
        }
        _context.Trades.Remove(trade);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Trade>> GetAllTradesAsync()
    {
        return await _context.Trades.Include(t => t.TradeItems).ToListAsync();
    }

    public async Task<Trade?> GetTradeByIdAsync(int tradeId)
    {
        return await _context
            .Trades.Include(t => t.InitiatedUser)
            .Include(t => t.ReceivedUser)
            .FirstOrDefaultAsync(t => t.Id == tradeId);
    }

    public async Task<List<Trade>> GetTradesByUser(int userId)
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

    public async Task<List<Trade>> GetTradesByStatusAsync(TradeStatus status)
    {
        return await _context.Trades
          .Include(t => t.TradeItems)
        .Include(t => t.InitiatedUser)
        .Include(t => t.ReceivedUser)
        .Include(t => t.Offers)
        .Where(t => t.Status == status)
        .ToListAsync();
    }
}
