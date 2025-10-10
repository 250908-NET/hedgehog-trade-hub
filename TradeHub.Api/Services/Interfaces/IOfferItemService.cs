using TradeHub.API.Models;
using TradeHub.API.Models.DTOs;

namespace TradeHub.API.Services.Interfaces
{
    public interface IOfferItemService
    {
        Task<OfferItem> AddItemToOfferAsync(int offerId, OfferItemCreateDto dto);
        Task<IEnumerable<OfferItem>> GetItemsByOfferAsync(int offerId);
        Task<bool> RemoveItemAsync(int offerItemId);
    }
}
