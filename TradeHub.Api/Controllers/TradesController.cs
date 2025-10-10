using Microsoft.AspNetCore.Mvc;
using TradeHub.Api.Models;
using TradeHub.Api.Services.Interfaces;

namespace TradeHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TradesController : ControllerBase
    {
        private readonly ITradeService _tradeService;

        public TradesController(ITradeService tradeService)
        {
            _tradeService = tradeService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Trade>>> GetAllTrades()
        {
            var trades = await _tradeService.GetAllTradesAsync();
            return Ok(trades);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<Trade>>> GetTradesByUser(long userId)
        {
            var trades = await _tradeService.GetTradesByUserAsync(userId);
            return Ok(trades);
        }

        [HttpGet("{tradeId}")]
        public async Task<ActionResult<Trade>> GetTrade(long tradeId)
        {
            var trade = await _tradeService.GetTradeByIdAsync(tradeId);
            if (trade == null)
                return NotFound();
            return Ok(trade);
        }

        [HttpPost]
        public async Task<ActionResult<Trade>> CreateTrade([FromBody] Trade trade)
        {
            var newTrade = await _tradeService.CreateTradeAsync(trade);
            return Ok(newTrade);
        }

        [HttpPut("{tradeId}")]
        public async Task<ActionResult<Trade>> UpdateTrade(long tradeId, [FromBody] Trade trade)
        {
            if (tradeId != trade.Id)
                return BadRequest();
            var updatedTrade = await _tradeService.UpdateTradeAsync(trade);
            return Ok(updatedTrade);
        }

        [HttpDelete("{tradeId}")]
        public async Task<IActionResult> DeleteTrade(long tradeId)
        {
            await _tradeService.DeleteTradeAsync(tradeId);
            return NoContent();
        }

        // // Mark trade as completed
        // [HttpPost("{tradeId}/complete")]
        // public async Task<IActionResult> CompleteTrade(long tradeId)
        // {
        //     var result = await _tradeService.MarkTradeCompletedAsync(tradeId);
        //     if (!result) return NotFound();
        //     return Ok(new { message = "Trade marked as completed." });
        // }
    }
}
