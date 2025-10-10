using Microsoft.AspNetCore.Mvc;
using TradeHub.Api.Models.DTOs;
using TradeHub.Api.Services.Interfaces;

namespace TradeHub.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OffersController(IOfferService offerService) : ControllerBase
    {
        private readonly IOfferService _offerService = offerService;

        // POST: api/offers
        // Create a new offer

        [HttpPost]
        public async Task<ActionResult<OfferDTO>> CreateOffer([FromBody] CreateOfferDTO dto)
        {
            var offer = await _offerService.CreateOfferAsync(dto);
            return Ok(offer);
        }

        // POST: api/offers/{offerId}/items
        // Add an item to an existing offer
        [HttpPost("{offerId}/items")]
        public async Task<ActionResult<OfferItemViewDto>> AddItemToOffer(
            int offerId,
            [FromBody] OfferItemCreateDto dto
        )
        {
            var offerItem = await _offerService.AddItemToOfferAsync(offerId, dto);
            return Ok(offerItem);
        }

        // GET: api/offers/trade/{tradeId}
        // Get all offers for a specific trade
        [HttpGet("trade/{tradeId}")]
        public async Task<ActionResult<IEnumerable<OfferDTO>>> GetAllOffersInTrade(int tradeId)
        {
            var offers = await _offerService.GetAllOffersInTradeAsync(tradeId);
            return Ok(offers);
        }

        // GET: api/offers/{offerId}
        // Get details of a specific offer
        [HttpGet("{offerId}")]
        public async Task<ActionResult<OfferDTO>> GetOffer(int offerId)
        {
            var offer = await _offerService.GetOfferAsync(offerId);
            if (offer == null)
                return NotFound();

            return Ok(offer);
        }

        // GET: api/offers/received/{userId}
        // Get all offers received by a user
        [HttpGet("received/{userId}")]
        public async Task<ActionResult<IEnumerable<OfferDTO>>> GetReceivedOffers(long userId)
        {
            var offers = await _offerService.GetReceivedOffersAsync(userId);
            return Ok(offers);
        }

        // PUT: api/offers/{offerId}
        // Update an existing offer (e.g., notes)
        [HttpPut("{offerId}")]
        public async Task<ActionResult> UpdateOffer(int offerId, [FromBody] OfferDTO dto)
        {
            if (offerId != dto.Id)
                return BadRequest("Offer ID mismatch");

            var success = await _offerService.UpdateOfferAsync(dto);
            if (!success)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/offers/{offerId}
        // Delete an offer
        [HttpDelete("{offerId}")]
        public async Task<ActionResult> DeleteOffer(int offerId)
        {
            var success = await _offerService.DeleteOfferAsync(offerId);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
