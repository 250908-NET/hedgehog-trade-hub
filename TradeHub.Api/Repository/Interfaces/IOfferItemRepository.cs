using TradeHub.Api.Models;
using TradeHub.Api.Models.DTOs;

namespace TradeHub.Api.Repository.Interfaces
{
    public interface IOfferItemRepository
    {
        Task<OfferItem> AddItemToOfferAsync(int offerId, OfferItemCreateDto dto);
        Task<IEnumerable<OfferItem>> GetItemsByOfferAsync(int offerId);
        Task<bool> RemoveItemAsync(int offerItemId);
    }
}
