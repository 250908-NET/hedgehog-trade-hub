using TradeHub.API.Models;
using TradeHub.API.Models.DTOs;

namespace TradeHub.API.Repository.Interfaces;

public interface IOfferRepository
{
    Task<IEnumerable<Offer>> GetAllOffersInTradeAsync(long tradeId);
    Task<Offer?> GetOfferAsync(long offerId);
    Task<Offer> CreateOfferAsync(CreateOfferDTO offerDto);
    Task<bool> UpdateOfferAsync(OfferDTO offerDto);
    Task<bool> DeleteOfferAsync(long offerId);
    Task<IEnumerable<ReceivedOfferDto>> GetReceivedOffersAsync(long userId);
}
