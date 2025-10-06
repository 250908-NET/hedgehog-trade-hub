using Microsoft.EntityFrameworkCore;
using TradeHub.Api.Models;
using TradeHub.Api.Repository.Interfaces;
using TradeHub.Api.Services.Interfaces;
using TradeHub.API.Models.DTOs;

namespace TradeHub.Api.Services;

public class OfferService : IOfferService
{
    private readonly IOfferRepository _offerRepository;

    public OfferService(IOfferRepository offerRepository)
    {
        _offerRepository = offerRepository;
    }

    public async Task<OfferDTO> CreateAsync(long tradeId, CreateOfferRequest request, CancellationToken ct = default)
    {
        var offer = new Offer
        {
            UserId = request.UserId,
            TradeId = tradeId,
            Created = DateTimeOffset.UtcNow
        };

        var createdOffer = await _offerRepository.CreateOfferAsync(offer);
        return new OfferDTO
        {
            Id = createdOffer.Id,
            UserId = createdOffer.UserId,
            TradeId = createdOffer.TradeId,
            Created = createdOffer.Created
        };
    }

    public async Task<OfferDTO?> GetAsync(long offerId, CancellationToken ct = default)
    {
        var offer = await _offerRepository.GetOfferAsync(offerId);
        if (offer == null) return null;

        return new OfferDTO
        {
            Id = offer.Id,
            UserId = offer.UserId,
            TradeId = offer.TradeId,
            Created = offer.Created
        };
    }

    public async Task<IEnumerable<OfferDTO>> GetAllInTradeAsync(long tradeId, CancellationToken ct = default)
    {
        var offers = await _offerRepository.GetAllOffersInTradeAsync(tradeId);
        return offers.Select(offer => new OfferDTO
        {
            Id = offer.Id,
            UserId = offer.UserId,
            TradeId = offer.TradeId,
            Created = offer.Created
        });
    }
}