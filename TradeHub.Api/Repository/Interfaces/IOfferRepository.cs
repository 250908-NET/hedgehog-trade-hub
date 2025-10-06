using TradeHub.Api.Models;

namespace TradeHub.Api.Repository.Interfaces;

public interface IOfferRepository
{
    Task<IEnumerable<Offer>> GetAllOffersInTradeAsync(long tradeId);
    Task<Offer?> GetOfferAsync(long offerId);
    Task<Offer> CreateOfferAsync(Offer offer);
    Task<bool> UpdateOfferAsync(Offer offer);
    Task<bool> DeleteOfferAsync(Offer offer);
}
