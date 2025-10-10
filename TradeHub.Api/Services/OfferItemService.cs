using TradeHub.Api.Models;
using TradeHub.Api.Models.DTOs;
using TradeHub.Api.Repository.Interfaces;
using TradeHub.Api.Services.Interfaces;

namespace TradeHub.Api.Services
{
    public class OfferItemService(IOfferItemRepository offerItemRepository) : IOfferItemService
    {
        private readonly IOfferItemRepository _offerItemRepository = offerItemRepository;

        public async Task<OfferItem> AddItemToOfferAsync(int offerId, OfferItemCreateDto dto)
        {
            // You can add any business logic here before adding
            if (dto.Quantity <= 0)
                throw new ArgumentException("Quantity must be at least 1");

            return await _offerItemRepository.AddItemToOfferAsync(offerId, dto);
        }

        public async Task<IEnumerable<OfferItem>> GetItemsByOfferAsync(int offerId)
        {
            // Additional filtering or business rules can go here
            return await _offerItemRepository.GetItemsByOfferAsync(offerId);
        }

        public async Task<bool> RemoveItemAsync(int offerItemId)
        {
            // Business rules: maybe you cannot remove an item from a completed offer
            return await _offerItemRepository.RemoveItemAsync(offerItemId);
        }
    }
}
