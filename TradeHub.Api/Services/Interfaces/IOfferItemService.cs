using TradeHub.Api.Models;
using TradeHub.Api.Models.DTOs;

namespace TradeHub.Api.Services.Interfaces
{
    public interface IOfferItemService
    {
        Task<OfferItem> AddItemToOfferAsync(int offerId, OfferItemCreateDto dto);
        Task<IEnumerable<OfferItem>> GetItemsByOfferAsync(int offerId);
        Task<bool> RemoveItemAsync(int offerItemId);
    }
}
