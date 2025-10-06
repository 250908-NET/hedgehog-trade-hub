using TradeHub.API.Models.DTOs;

namespace TradeHub.Api.Services.Interfaces;

public interface IOfferService
{
    Task<OfferDTO> CreateAsync(long tradeId, CreateOfferRequest request, CancellationToken ct = default);
    Task<OfferDTO?> GetAsync(long offerId, CancellationToken ct = default);
    Task<IEnumerable<OfferDTO>> GetAllInTradeAsync(long tradeId, CancellationToken ct = default);
}
