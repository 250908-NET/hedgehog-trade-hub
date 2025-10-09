using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TradeHub.Api.Models;
using TradeHub.Api.Models.DTOs;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;

    public UsersController(
        ILogger<UsersController> logger,
        IMapper mapper,
        UserManager<User> userManager)
    {
        _logger = logger;
        _mapper = mapper;
        _userManager = userManager;
    }

    // ---------------------------
    // GET: /users/{id}
    // ---------------------------
    [Authorize]
    [HttpGet("{id}", Name = "GetUser")]
    public async Task<ActionResult<UserDto>> GetAsync(long id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found.", id);
            return NotFound($"User with ID {id} not found.");
        }

        var currentUserEmail = User.FindFirstValue(ClaimTypes.Email);
        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin && user.Email != currentUserEmail)
            return Unauthorized("You are not authorized to view other users.");

        var userDto = _mapper.Map<UserDto>(user);
        _logger.LogInformation("Retrieved user with ID {UserId}", id);
        return Ok(userDto);
    }

    // ---------------------------
    // POST: /users
    // ---------------------------
    [HttpPost(Name = "CreateUser")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateAsync([FromBody] RegisterUserDTO dto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid registration data.");
            return BadRequest(ModelState);
        }

        var user = _mapper.Map<User>(dto);
        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        // Optionally assign default role
        await _userManager.AddToRoleAsync(user, "User");

        var userDto = _mapper.Map<UserDto>(user);
        _logger.LogInformation("Admin created new user with ID {UserId}", user.Id);

        return CreatedAtRoute("GetUser", new { id = user.Id }, userDto);
    }

    // ---------------------------
    // PUT: /users/{id}
    // ---------------------------
    [Authorize]
    [HttpPut("{id}", Name = "UpdateUser")]
    public async Task<IActionResult> UpdateAsync(long id, [FromBody] UserDto dto)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found for update.", id);
            return NotFound($"User with ID {id} not found.");
        }

        var currentUserEmail = User.FindFirstValue(ClaimTypes.Email);
        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin && user.Email != currentUserEmail)
            return Unauthorized("You can only update your own account.");

        // Map allowed fields from DTO to user
        _mapper.Map(dto, user);

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        _logger.LogInformation("Updated user with ID {UserId}", id);
        return Ok(_mapper.Map<UserDto>(user));
    }

    // ---------------------------
    // DELETE: /users/{id}
    // ---------------------------
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}", Name = "DeleteUser")]
    public async Task<IActionResult> DeleteAsync(long id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found for deletion.", id);
            return NotFound($"User with ID {id} not found.");
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        _logger.LogInformation("Deleted user with ID {UserId}", id);
        return NoContent();
    }
}
