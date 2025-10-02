using TradeHub.Api.Models;

namespace TradeHub.Api.Repository.Interfaces;

public interface IOfferRepository
{
    Task<IEnumerable<Offer>> GetAllOffersInTradeAsync(int tradeId);
    Task<Offer?> GetOfferAsync(int offerId);
    Task<Offer> CreateOfferAsync(OfferDTO offerDto);
    Task<bool> UpdateOfferAsync(OfferDTO offerDto);
    Task<bool> DeleteOfferAsync(int offerId);
}
