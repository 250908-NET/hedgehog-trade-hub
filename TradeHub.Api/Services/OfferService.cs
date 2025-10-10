using TradeHub.API.Models;
using TradeHub.API.Models.DTOs;
using TradeHub.API.Repository.Interfaces;
using TradeHub.API.Services.Interfaces;

namespace TradeHub.API.Services;

public class OfferService(IOfferRepository repository) : IOfferService
{
    private readonly IOfferRepository _repository =
        repository ?? throw new ArgumentNullException(nameof(repository));

    public async Task<IEnumerable<Offer>> GetAllOffersInTradeAsync(long tradeId)
    {
        return await _repository.GetAllOffersInTradeAsync(tradeId);
    }

    public async Task<Offer?> GetOfferAsync(long offerId)
    {
        return await _repository.GetOfferAsync(offerId);
    }

    public async Task<Offer> CreateOfferAsync(CreateOfferDTO offerDto)
    {
        return await _repository.CreateOfferAsync(offerDto);
    }

    public async Task<bool> UpdateOfferAsync(OfferDTO offerDto)
    {
        return await _repository.UpdateOfferAsync(offerDto);
    }

    public async Task<bool> DeleteOfferAsync(long offerId)
    {
        return await _repository.DeleteOfferAsync(offerId);
    }

    public async Task<IEnumerable<ReceivedOfferDTO>> GetReceivedOffersAsync(long userId)
    {
        return await _repository.GetReceivedOffersAsync(userId);
    }

    public Task<OfferItemViewDto> AddItemToOfferAsync(long offerId, OfferItemCreateDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<OfferItemViewDto>> GetOfferItemsAsync(long offerId)
    {
        throw new NotImplementedException();
    }
}
