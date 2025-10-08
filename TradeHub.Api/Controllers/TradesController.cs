using Microsoft.AspNetCore.Mvc;
using TradeHub.Api.Services.Interfaces;
using TradeHub.Api.Models;

namespace TradeHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TradeController : ControllerBase
{
    private readonly ITradeService _trades;
    private readonly ILogger<TradeController> _logger;

    // Inject ITradeService (already exists)
    public TradeController(ITradeService trades, ILogger<TradeController> logger)
    {
        _trades = trades ?? throw new ArgumentNullException(nameof(trades));
        _logger = logger;
    }

    // Helper to get current user id (stub until real auth)
    private static int GetCurrentUserId(HttpContext ctx)
    {
        // TODO: replace with real auth/claims. Temporary: X-User-Id header or similar
        var header = ctx.Request.Headers["X-User-Id"].FirstOrDefault();
        return int.TryParse(header, out var id) ? id : 0;
    }

    [HttpPost("{tradeId:int}/accept")]
    public async Task<IActionResult> AcceptTrade([FromRoute] int tradeId)
    {
        var currentUserId = GetCurrentUserId(HttpContext);

        var trade = await _trades.GetTradeByIdAsync(tradeId);
        if (trade is null) return NotFound();

        // Only the receiver (Galaxy owner) can accept
        if (trade.ReceivedId != currentUserId) return Forbid();

        // Only Pending can be accepted
        if (trade.Status != 0) return Conflict(new { message = "Trade is not pending." });

        trade.Status = 1; // Accepted
        var updated = await _trades.UpdateTradeAsync(trade);

        // Optional: next-step guidance link could be returned here
        return Ok(new { tradeId = updated.Id, status = updated.Status, next = "complete" });
    }

    [HttpPost("{tradeId:int}/reject")]
    public async Task<IActionResult> RejectTrade([FromRoute] int tradeId)
    {
        var currentUserId = GetCurrentUserId(HttpContext);

        var trade = await _trades.GetTradeByIdAsync(tradeId);
        if (trade is null) return NotFound();

        // Only the receiver (Galaxy owner) can reject
        if (trade.ReceivedId != currentUserId) return Forbid();

        // Only Pending can be rejected
        if (trade.Status != 0) return Conflict(new { message = "Trade is not pending." });

        trade.Status = 2; // Rejected
        var updated = await _trades.UpdateTradeAsync(trade);

        return Ok(new { tradeId = updated.Id, status = updated.Status });
    }
}
