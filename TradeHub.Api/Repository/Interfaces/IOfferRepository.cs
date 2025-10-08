using TradeHub.API.Models;
using TradeHub.API.Models.DTOs;

namespace TradeHub.API.Repository.Interfaces;

public interface IOfferRepository
{
    Task<IEnumerable<Offer>> GetAllOffersInTradeAsync(int tradeId);
    Task<Offer?> GetOfferAsync(int offerId);
    Task<Offer> CreateOfferAsync(OfferDTO offerDto);
    Task<bool> UpdateOfferAsync(OfferDTO offerDto);
    Task<bool> DeleteOfferAsync(int offerId);
    Task<IEnumerable<ReceivedOfferDto>> GetReceivedOffersAsync(long userId);
}
