using TradeHub.Api.Models;
using TradeHub.Api.Models.DTOs;

namespace TradeHub.Api.Services.Interfaces;

public interface IOfferService
{
    Task<IEnumerable<Offer>> GetAllOffersInTradeAsync(long tradeId);
    Task<Offer?> GetOfferAsync(long offerId);
    Task<Offer> CreateOfferAsync(CreateOfferDTO offerDto);
    Task<bool> UpdateOfferAsync(OfferDTO offerDto);
    Task<bool> DeleteOfferAsync(long offerId);

    // to receive trade proposal

    Task<IEnumerable<ReceivedOfferDTO>> GetReceivedOffersAsync(long userId);
    Task<OfferItemViewDto> AddItemToOfferAsync(long offerId, OfferItemCreateDto dto);
    Task<IEnumerable<OfferItemViewDto>> GetOfferItemsAsync(long offerId);
}
