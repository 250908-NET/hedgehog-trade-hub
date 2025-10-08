
using TradeHub.API.Models;
using TradeHub.API.Models.DTOs;

namespace TradeHub.API.Repository.Interfaces
{
    public interface IOfferItemRepository
    {
        Task<OfferItem> AddItemToOfferAsync(int offerId, OfferItemCreateDto dto);
        Task<IEnumerable<OfferItem>> GetItemsByOfferAsync(int offerId);
        Task<bool> RemoveItemAsync(int offerItemId);
    }
}
