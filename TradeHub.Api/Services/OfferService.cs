using TradeHub.API.Models;
using TradeHub.API.Models.DTOs;
using TradeHub.API.Repository.Interfaces;
using TradeHub.API.Services.Interfaces;

namespace TradeHub.API.Services;

public class OfferService : IOfferService
{
    private readonly IOfferRepository _repository;

    public OfferService(IOfferRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<IEnumerable<Offer>> GetAllOffersInTradeAsync(int tradeId)
    {
        return await _repository.GetAllOffersInTradeAsync(tradeId);
    }

    public async Task<Offer?> GetOfferAsync(int offerId)
    {
        return await _repository.GetOfferAsync(offerId);
    }

    public async Task<Offer> CreateOfferAsync(OfferDTO offerDto)
    {
        return await _repository.CreateOfferAsync(offerDto);
    }

    public async Task<bool> UpdateOfferAsync(OfferDTO offerDto)
    {
        return await _repository.UpdateOfferAsync(offerDto);
    }

    public async Task<bool> DeleteOfferAsync(int offerId)
    {
        return await _repository.DeleteOfferAsync(offerId);
    }
}