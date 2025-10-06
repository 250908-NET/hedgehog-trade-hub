using Microsoft.AspNetCore.Mvc;
using TradeHub.Api.Models;
using TradeHub.Api.Models.DTOs;
using TradeHub.Api.Services.Interfaces;

namespace TradeHub.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ItemsController(IItemService itemService) : ControllerBase
{
    private readonly IItemService _itemService = itemService;

    [HttpGet(Name = "GetAllItems")]
    public async Task<ActionResult<List<ItemDTO>>> GetAllItems(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] decimal? minValue = null,
        [FromQuery] decimal? maxValue = null,
        [FromQuery] Condition? condition = null,
        [FromQuery] Availability? availability = null,
        [FromQuery] string? search = null
    )
    {
        return Ok(
            await _itemService.GetAllItemsAsync(
                page,
                pageSize,
                minValue,
                maxValue,
                condition,
                availability,
                search
            )
        );
    }

    [HttpGet("{id}", Name = "GetItemById")]
    public async Task<ActionResult<ItemDTO>> GetItemById(long id)
    {
        return Ok(await _itemService.GetItemByIdAsync(id));
    }

    [HttpPost(Name = "CreateItem")]
    public async Task<ActionResult> CreateItem(CreateItemDTO dto)
    {
        return Ok(await _itemService.CreateItemAsync(dto));
    }

    [HttpPut("{id}", Name = "UpdateItem")]
    public async Task<ActionResult> UpdateItem(long id, UpdateItemDTO dto)
    {
        return Ok(await _itemService.UpdateItemAsync(id, dto));
    }

    [HttpDelete("{id}", Name = "DeleteItem")]
    public async Task<ActionResult> DeleteItem(long id)
    {
        return Ok(await _itemService.DeleteItemAsync(id));
    }
}
