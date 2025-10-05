using TradeHub.Api.Models;
using TradeHub.API.Models.DTOs;

namespace TradeHub.Api.Services.Interfaces;

public interface IOfferService
{
    Task<IEnumerable<Offer>> GetAllOffersInTradeAsync(int tradeId);
    Task<Offer?> GetOfferAsync(int offerId);
    Task<Offer> CreateOfferAsync(OfferDTO offerDto);
    Task<bool> UpdateOfferAsync(OfferDTO offerDto);
    Task<bool> DeleteOfferAsync(int offerId);
}