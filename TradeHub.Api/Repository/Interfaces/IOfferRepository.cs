using TradeHub.Api.Models;
using TradeHub.Api.Models.DTOs;

namespace TradeHub.Api.Repository.Interfaces;

public interface IOfferRepository
{
    Task<IEnumerable<Offer>> GetAllOffersInTradeAsync(long tradeId);
    Task<Offer?> GetOfferAsync(long offerId);
    Task<Offer> CreateOfferAsync(CreateOfferDTO offerDto);
    Task<bool> UpdateOfferAsync(OfferDTO offerDto);
    Task<bool> DeleteOfferAsync(long offerId);
    Task<IEnumerable<ReceivedOfferDTO>> GetReceivedOffersAsync(long userId);
}
