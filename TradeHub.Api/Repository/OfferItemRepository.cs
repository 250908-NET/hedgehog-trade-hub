using Microsoft.EntityFrameworkCore;
using TradeHub.API.Models;
using TradeHub.API.Models.DTOs;
using TradeHub.API.Repository.Interfaces;

namespace TradeHub.API.Repository.Implementations
{
    public class OfferItemRepository : IOfferItemRepository
    {
        private readonly TradeHubContext _context;

        public OfferItemRepository(TradeHubContext context)
        {
            _context = context;
        }

        public async Task<OfferItem> AddItemToOfferAsync(int offerId, OfferItemCreateDto dto)
        {
            var offerItem = new OfferItem
            {
                OfferId = offerId,
                ItemId = dto.ItemId,
                Quantity = dto.Quantity,
                Notes = dto.Notes
            };

            _context.OfferItems.Add(offerItem);
            await _context.SaveChangesAsync();

            // Optionally include the navigation property
            await _context.Entry(offerItem).Reference(oi => oi.Item).LoadAsync();

            return offerItem;
        }

        public async Task<IEnumerable<OfferItem>> GetItemsByOfferAsync(int offerId)
        {
            return await _context.OfferItems
                                 .Include(oi => oi.Item) // include item details
                                 .Where(oi => oi.OfferId == offerId)
                                 .ToListAsync();
        }

        public async Task<bool> RemoveItemAsync(int offerItemId)
        {
            var item = await _context.OfferItems.FindAsync(offerItemId);
            if (item == null)
                return false;

            _context.OfferItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

