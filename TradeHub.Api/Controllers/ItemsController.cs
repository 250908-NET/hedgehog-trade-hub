using Microsoft.AspNetCore.Authorization.Infrastructure;
using TradeHub.Api.Models;
using TradeHub.Api.Models.DTOs;
using TradeHub.Api.Repository.Interfaces
using Microsoft.AspNetCore.Mvc;
using TradeHub.Api.Models;
using TradeHub.Api.Repository.Interfaces;

namespace TradeHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly IItemRepository _itemRepository;

    public ItemsController(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    [HttpGet(Name = "GetAllItems")]
    public async Task<ActionResult<List<Item>>> GetAllItems()
    {
        var items = await _itemRepository.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id}", Name = "GetItemById")]
    public async Task<ActionResult<Item?>> GetItemById(int id)
    {
        var item = await _itemRepository.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpPost(Name = "CreateItem")]
    public async Task<ActionResult> CreateItem(ItemDTO itemDto)
    {
         var item = new Item(
            itemDto.Description,
            itemDto.Image,
            itemDto.Value,
            itemDto.Owner,
            itemDto.Tags,
            itemDto.Condition,
            itemDto.Availability
        );


        await _itemRepository.AddAsync(item);
        await _itemRepository.SaveChangesAsync();
        return CreatedAtAction(nameof(GetItemById), new { id = item.Id }, item);
    }

    [HttpPut("{id}", Name = "UpdateItem")]
    public async Task<ActionResult> UpdateItem(int id, ItemDTO updatedItemDto)

    {
        var existingItem = await_itemRepository.GetByIdAsync(id);
        if (existingItem == null) return NotFound();

        existingItem.Description = updatedItemDto.Description;
        existingItem.Image = updatedItemDto.Image;
        existingItem.Value = updatedItemDto.Value;
        existingItem.Tags = updatedItemDto.Tags;
        existingItem.Condition = updatedItemDto.Condition;
        existingItem.Availability = updatedItemDto.Availability;


        await _itemRepository.UpdateAsync(id, updatedItem);
        await _itemRepository.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}", Name = "DeleteItem")]
    public async Task<ActionResult> DeleteItem(int id)
    {
        var deleted = await _itemRepository.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }
        await _itemRepository.SaveChangesAsync();
        return NoContent();
    }
}
