using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TradeHub.Api.Models;
using TradeHub.Api.Models.DTOs;

namespace TradeHub.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(
    UserManager<User> userManager,
    RoleManager<IdentityRole<long>> roleManager,
    ITokenService tokenService,
    ILogger<AuthController> logger
) : ControllerBase
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly RoleManager<IdentityRole<long>> _roleManager = roleManager;
    private readonly ITokenService _tokenService = tokenService;
    private readonly ILogger<AuthController> _logger = logger;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateToken(user, roles);

        _logger.LogInformation("User logged in: {Email}", dto.Email);

        return Ok(
            new AuthResponseDto
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email ?? string.Empty,
                Username = user.UserName ?? string.Empty,
                Roles = [.. roles],
            }
        );
    }

    [HttpPost("register/user")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDTO dto)
    {
        var user = new User
        {
            UserName = dto.Username,
            Email = dto.Email,
            Description = dto.Description,
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        // Ensure "User" role exists
        if (!await _roleManager.RoleExistsAsync("User"))
        {
            await _roleManager.CreateAsync(new IdentityRole<long>("User"));
        }

        await _userManager.AddToRoleAsync(user, "User");

        _logger.LogInformation("User registered: {Email}", dto.Email);

        return Ok(new { message = "User registered successfully" });
    }

    [HttpPost("register/Admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterUserDTO dto)
    {
        var user = new User
        {
            UserName = dto.Username,
            Email = dto.Email,
            Description = dto.Description,
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        // Ensure "Admin" role exists
        if (!await _roleManager.RoleExistsAsync("Admin"))
        {
            await _roleManager.CreateAsync(new IdentityRole<long>("Admin"));
        }

        await _userManager.AddToRoleAsync(user, "Admin");

        _logger.LogInformation("Admin registered: {Email}", dto.Email);

        return Ok(new { message = "Admin registered successfully" });
    }
}
