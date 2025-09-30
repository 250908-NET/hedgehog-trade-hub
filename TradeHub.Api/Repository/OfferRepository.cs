using Microsoft.EntityFrameworkCore;
using TradeHub.Api.Models;
using TradeHub.Api.Utilities;

namespace TradeHub.Api.Repository;

public class OfferRepository(TradeHubContext context) : IOfferRepository
{
    private readonly TradeHubContext _context = context;

    public async Task<Offer> CreateOfferAsync(Offer offer)
    {
        _context.Offers.Add(offer);
        await _context.SaveChangesAsync();

        // eager load navigation properties
        await _context.Entry(offer).Reference(o => o.Trade).LoadAsync();
        await _context.Entry(offer).Reference(o => o.User).LoadAsync();

        return offer;
    }

    public async Task<bool> DeleteOfferAsync(Offer offer)
    {
        _context.Offers.Remove(offer);
        try
        {
            return await _context.SaveChangesAsync() > 0;
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConflictException(
                "The card you are trying to delete has been modified by another user. Please refresh and try again."
            );
        }
    }

    public async Task<IEnumerable<Offer>> GetAllOffersInTradeAsync(int tradeId)
    {
        // include navigation properties as readonly
        return await _context
            .Offers.Where(o => o.TradeId == tradeId)
            .AsNoTracking()
            .Include(o => o.User)
            .Include(o => o.Trade)
            .ToListAsync();
    }

    public async Task<Offer?> GetOfferAsync(int offerId)
    {
        // include navigation properties
        return await _context
            .Offers.Include(o => o.Trade)
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == offerId);
    }

    public async Task<bool> UpdateOfferAsync(Offer offer)
    {
        _context.Entry(offer).State = EntityState.Modified;
        try
        {
            return await _context.SaveChangesAsync() > 0;
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConflictException(
                "The card you are trying to update has been modified by another user. Please refresh and try again."
            );
        }
    }
}
