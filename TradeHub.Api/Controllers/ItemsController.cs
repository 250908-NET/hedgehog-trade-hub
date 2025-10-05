using Microsoft.AspNetCore.Mvc;
using TradeHub.Api.Models;
using TradeHub.Api.Models.DTOs;
using TradeHub.Api.Repository.Interfaces;

namespace TradeHub.Api.Controllers;

[ApiController]
[Route("[controller]")]
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
    public async Task<ActionResult> CreateItem(CreateItemDTO itemDto)
    {
        var item = new Item(
            itemDto.Name,
            itemDto.Description,
            itemDto.Image,
            itemDto.Value,
            itemDto.OwnerId,
            itemDto.Tags,
            itemDto.Condition,
            itemDto.Availability
        );

        await _itemRepository.AddAsync(item);
        await _itemRepository.SaveChangesAsync();
        return CreatedAtAction(nameof(GetItemById), new { id = item.Id }, item);
    }

    [HttpPut("{id}", Name = "UpdateItem")]
    public async Task<ActionResult> UpdateItem(int id, UpdateItemDTO updatedItemDto)
    {
        var existingItem = await _itemRepository.GetByIdAsync(id);
        if (existingItem == null)
            return NotFound();

        // Only update fields that are provided (not null)
        if (updatedItemDto.Name is not null)
            existingItem.Name = updatedItemDto.Name;
        if (updatedItemDto.Description is not null)
            existingItem.Description = updatedItemDto.Description;
        if (updatedItemDto.Image is not null)
            existingItem.Image = updatedItemDto.Image;
        if (updatedItemDto.Value is not null)
            existingItem.Value = updatedItemDto.Value.Value;
        if (updatedItemDto.OwnerId is not null)
            existingItem.OwnerId = updatedItemDto.OwnerId.Value;
        if (updatedItemDto.Tags is not null)
            existingItem.Tags = updatedItemDto.Tags;
        if (updatedItemDto.Condition is not null)
            existingItem.Condition = updatedItemDto.Condition.Value;
        if (updatedItemDto.Availability is not null)
            existingItem.Availability = updatedItemDto.Availability.Value;

        await _itemRepository.UpdateAsync(id, existingItem);
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
