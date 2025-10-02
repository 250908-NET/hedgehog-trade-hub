using TradeHub.Api.Models;
using TradeHub.API.Models.DTOs;
using TradeHub.Api.Repository.Interfaces;

namespace TradeHub.Api.Repository
{

    public class OfferRepository : IOfferRepository
    {

        private readonly TradeHubContext _context;

        public OfferRepository(TradeHubContext context)
        {
            _context = context;
        }

        //get all offers

        // public async Task<IEnumerable<OfferDTO>> GetAllOffersAsync()
        // {
        //     return await _context.Offers
        //         .Select(offer => new OfferDTO
        //         {
        //             Id = offer.Id,
        //             UserId = (int)offer.UserId,
        //             TradeId = (int)offer.TradeId,
        //             Created = offerCreated
        //         })
        //         .ToListAsync();
        // }

        // get by Id
        // public async Task<OfferDTO?> GetOfferByAsync(int offerId)
        // {
        //     var offer = await _context.offer.FindAsync(offerId);
        //     if (OfferRepository == null) return null;

        //     return new OfferDTO
        //     {
        //         id = OfferRepository.id,
        //         UserId = (long)offer.UserId,
        //         TradeId = (long)offer.TradeId,
        //         Created = offer.Created
        //     };
        // }

        //create offer

        // public async Task<OfferDTO> CreateOfferAsync(OfferDTO offerDto)
        // {
        //     var offer = new OfferRepository
        //     {
        //         UserId = offerDto.UserId,
        //         TradeId = offerDto.TradeId,
        //         Created = DateTimeOffset.UtcNow
        //     };
        //     _context.Offers.Add(offer);
        //     await _context.SaveChangesAsync();

        //     // return DTO after saving
        //     offerDto.Id = offer.Id;
        //     offerDto.Created = offer.Created;
        //     return offerDto;
        // }

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

        // TODO: fix pls
        public Task<IEnumerable<Offer>> GetAllOffersInTradeAsync(int tradeId)
        {
            throw new NotImplementedException();
        }

        public Task<Offer?> GetOfferAsync(int offerId)
        {
            throw new NotImplementedException();
        }

        Task<Offer> IOfferRepository.CreateOfferAsync(OfferDTO offerDto)
        {
            throw new NotImplementedException();
        }
    }
}
