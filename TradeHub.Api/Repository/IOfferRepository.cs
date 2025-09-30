using TradeHub.Api.Models;

namespace TradeHub.Api.Repository;

public interface IOfferRepository
{
    Task<IEnumerable<Offer>> GetAllOffersInTradeAsync(int tradeId);
    Task<Offer?> GetOfferAsync(int offerId);
    Task<Offer> CreateOfferAsync(Offer offer);
    Task<bool> UpdateOfferAsync(Offer offer);
    Task<bool> DeleteOfferAsync(Offer offer);
}
