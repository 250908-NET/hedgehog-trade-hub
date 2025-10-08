using TradeHub.API.Models;
using TradeHub.API.Models.DTOs;

namespace TradeHub.API.Services.Interfaces;

public interface IOfferService
{
    Task<IEnumerable<Offer>> GetAllOffersInTradeAsync(int tradeId);
    Task<Offer?> GetOfferAsync(int offerId);
    Task<Offer> CreateOfferAsync(OfferDTO offerDto);
    Task<bool> UpdateOfferAsync(OfferDTO offerDto);
    Task<bool> DeleteOfferAsync(int offerId);


    // to receive trade proposal

    Task<IEnumerable<OfferDTO>> GetReceivedOffersAsync(long userId);
    Task<OfferItemViewDto> AddItemToOfferAsync(int offerId, OfferItemCreateDto dto);
    Task<IEnumerable<OfferItemViewDto>> GetOfferItemsAsync(int offerId)
}