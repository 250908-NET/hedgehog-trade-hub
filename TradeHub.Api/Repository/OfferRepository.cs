using Microsoft.EntityFrameworkCore;
using TradeHub.API.Models;
using TradeHub.API.Models.DTOs;
using TradeHub.API.Repository.Interfaces;

namespace TradeHub.API.Repository
{
    public class OfferRepository(TradeHubContext context) : IOfferRepository
    {
        private readonly TradeHubContext _context = context;

        // get all offers
        public async Task<IEnumerable<OfferDTO>> GetAllOffersAsync()
        {
            return await _context
                .Offers.Select(offer => new OfferDTO
                {
                    Id = offer.Id,
                    UserId = (int)offer.UserId,
                    TradeId = (int)offer.TradeId,
                    Created = offer.CreatedAt,
                })
                .ToListAsync();
        }

        //get by Id
        public async Task<OfferDTO?> GetOfferByAsync(long offerId)
        {
            var offer = await _context.Offers.FindAsync(offerId);
            if (offer == null)
                return null;

            return new OfferDTO
            {
                Id = offer.Id,
                UserId = offer.UserId,
                TradeId = offer.TradeId,
                Created = offer.CreatedAt,
            };
        }

        // update offer

        public async Task<bool> UpdateOfferAsync(OfferDTO offerDto)
        {
            var offer = await _context.Offers.FindAsync(offerDto.Id);
            if (offer == null)
                return false;

            offer.UserId = offerDto.UserId;
            offer.TradeId = offerDto.TradeId;

            _context.Offers.Update(offer);
            await _context.SaveChangesAsync();
            return true;
        }

        //delete offer

        public async Task<bool> DeleteOfferAsync(long offerId)
        {
            var offer = await _context.Offers.FindAsync(offerId);
            if (offer == null)
                return false;

            _context.Offers.Remove(offer);
            await _context.SaveChangesAsync();
            return true;
        }

        //create offer

        public async Task<Offer> CreateOfferAsync(CreateOfferDTO offerDto)
        {
            var offer = new Offer
            {
                UserId = offerDto.UserId,
                TradeId = offerDto.TradeId,
                Notes = offerDto.Notes,
                CreatedAt = DateTimeOffset.UtcNow,
            };

            _context.Offers.Add(offer);
            await _context.SaveChangesAsync();

            return offer;
        }

        // get all the offers for a specific trade

        public async Task<IEnumerable<Offer>> GetAllOffersInTradeAsync(long tradeId)
        {
            return await _context
                .Offers.Where(offer => offer.TradeId == tradeId)
                .Include(o => o.User)
                .Include(o => o.Trade)
                .ToListAsync();
        }

        // get specific offer by its id
        public async Task<Offer?> GetOfferAsync(long offerId)
        {
            return await _context
                .Offers.Include(o => o.User)
                .Include(o => o.Trade)
                .FirstOrDefaultAsync(o => o.Id == offerId);
        }

        public async Task<IEnumerable<ReceivedOfferDto>> GetReceivedOffersAsync(long userId)
        {
            var offers = await _context
                .Offers.Include(o => o.Trade)
                .Include(o => o.OfferItems)
                .ThenInclude(oi => oi.Item)
                .Where(o => o.Trade.ReceivedId == userId) // only received trades
                .Select(o => new ReceivedOfferDto
                {
                    OfferId = o.Id,
                    TradeId = o.TradeId,
                    TradeItemCondition = o.Trade.ItemCondition,
                    TradeNotes = o.Trade.Notes,
                    OwnerReputation = o.Trade.OwnerReputation,
                    OfferCreated = o.CreatedAt,
                    Status = o.Trade.Status,
                    Items = o
                        .OfferItems.Select(oi => new OfferItemViewDto
                        {
                            Id = oi.Id,
                            ItemId = oi.ItemId,
                            ItemName = oi.Item.Name,
                            Quantity = oi.Quantity,
                            Notes = oi.Notes,
                        })
                        .ToList(),
                })
                .ToListAsync();

            return offers;
        }
    }
}
