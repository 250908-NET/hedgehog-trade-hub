using Microsoft.EntityFrameworkCore;
using TradeHub.API.Models;
using TradeHub.API.Models.DTOs;
using TradeHub.API.Repository.Interfaces;

namespace TradeHub.API.Repository
{

    public class OfferRepository : IOfferRepository
    {

        private readonly TradeHubContext _context;

        public OfferRepository(TradeHubContext context)
        {
            _context = context;
        }

        // get all offers

        public async Task<IEnumerable<OfferDTO>> GetAllOffersAsync()
        {
            return await _context.Offers
                .Select(offer => new OfferDTO
                {
                    Id = offer.Id,
                    UserId = (int)offer.UserId,
                    TradeId = (int)offer.TradeId,
                    Created = offer.Created
                })
                .ToListAsync();
        }

        //get by Id
        public async Task<OfferDTO?> GetOfferByAsync(int offerId)
        {
            var offer = await _context.Offers.FindAsync(offerId);
            if (offer == null) return null;

            return new OfferDTO
            {
                Id = offer.Id,
                UserId = offer.UserId,
                TradeId = offer.TradeId,
                Created = offer.Created
            };
        }
        // update offer

        public async Task<bool> UpdateOfferAsync(OfferDTO offerDto)
        {
            var offer = await _context.Offers.FindAsync(offerDto.Id);
            if (offer == null) return false;

            offer.UserId = offerDto.UserId;
            offer.TradeId = offerDto.TradeId;

            _context.Offers.Update(offer);
            await _context.SaveChangesAsync();
            return true;
        }

        //delete offer

        public async Task<bool> DeleteOfferAsync(int offerId)
        {
            var offer = await _context.Offers.FindAsync(offerId);
            if (offer == null) return false;

            _context.Offers.Remove(offer);
            await _context.SaveChangesAsync();
            return true;

        }

        //create offer

        public async Task<Offer> CreateOfferAsync(OfferDTO offerDto)
        {
            var offer = new Offer
            {
                UserId = offerDto.UserId,
                TradeId = offerDto.TradeId,
                Notes = offerDto.Notes,
                Created = DateTimeOffset.UtcNow
            };

            _context.Offers.Add(offer);
            await _context.SaveChangesAsync();

            return offer;
        }


        // get all the offers for a specific trade

        public async Task<IEnumerable<Offer>> GetAllOffersInTradeAsync(int tradeId)
        {
            return await _context.Offers
            .Where(offer => offer.TradeId == tradeId)
            .Include(o => o.User)
            .Include(o => o.Trade)
             .ToListAsync();
        }

        // get specific offer by its id
        public async Task<Offer?> GetOfferAsync(int offerId)
        {
            return await _context.Offers
                .Include(o => o.User)
                .Include(o => o.Trade)
                .FirstOrDefaultAsync(o => o.Id == offerId);
        }

    }
}




