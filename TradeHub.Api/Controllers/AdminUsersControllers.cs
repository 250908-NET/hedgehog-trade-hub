using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TradeHub.Api.Services;              // IAdminUserService
using TradeHub.Api.Models.DTOs;          // UserDTO, CreateUserDTO, UpdateUserDTO

namespace TradeHub.Api.Controllers;

[ApiController]
[Route("admin/users")]
[Authorize(Policy = "AdminOnly")]
public class AdminUsersController : ControllerBase
{
    private readonly IAdminUserService _svc;

    public AdminUsersController(IAdminUserService svc) => _svc = svc;

    /// <summary>Get all users.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetAll()
        => Ok(await _svc.GetAllAsync(User));

    /// <summary>Get a specific user.</summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDTO>> GetById(long id)
    {
        var dto = await _svc.GetByIdAsync(id, User);
        return dto is null ? NotFound(new { message = $"User {id} not found." }) : Ok(dto);
    }

    /// <summary>Create a new user.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(UserDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserDTO>> Create([FromBody] CreateUserDTO req)
    {
        try
        {
            var created = await _svc.CreateAsync(req, User);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return ValidationProblem(title: "Invalid input", detail: ex.Message, statusCode: 400);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>Update a user (partial).</summary>
    [HttpPatch("{id:long}")]
    [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserDTO>> Update(long id, [FromBody] UpdateUserDTO req)
    {
        try
        {
            var updated = await _svc.UpdateAsync(id, req, User);
            return updated is null
                ? NotFound(new { message = $"User {id} not found." })
                : Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            // e.g., duplicate username/email, or self-delete protection etc.
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return ValidationProblem(title: "Invalid input", detail: ex.Message, statusCode: 400);
        }
    }

    /// <summary>Delete a user.</summary>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            var ok = await _svc.DeleteAsync(id, User);
            return ok ? NoContent() : NotFound(new { message = $"User {id} not found." });
        }
        catch (InvalidOperationException ex)
        {
            // e.g., prevent admin deleting themselves
            return Conflict(new { message = ex.Message });
        }
    }
}
