// Controllers/AuthController.cs
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TradeHub.Api.Models;
using TradeHub.Api.Repository.Interfaces;

namespace TradeHub.Api.Controllers;

[ApiController]
[Route("")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher<User> _hasher;

    public AuthController(IUserRepository users, IPasswordHasher<User> hasher)
    {
        _users = users;
        _hasher = hasher;
    }

    /// <summary>Log in and receive an auth cookie</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        // Find by username first, then by email
        var user = await _users.GetByUsernameAsync(req.UsernameOrEmail)
                   ?? (await _users.GetAllAsync()).FirstOrDefault(u =>
                        u.Email.Equals(req.UsernameOrEmail, StringComparison.OrdinalIgnoreCase));

        if (user is null)
            return Unauthorized(new { message = "Invalid credentials." });

        var verify = _hasher.VerifyHashedPassword(user, user.PasswordHash, req.Password);
        if (verify == PasswordVerificationResult.Failed)
            return Unauthorized(new { message = "Invalid credentials." });

        // Build claims
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role == 1 ? "Admin" : "User")
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        // Issue cookie
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            });

        return Ok(new { message = "Logged in." });
    }

    /// <summary>Logout (clear the cookie)</summary>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(new { message = "Logged out." });
    }

    /// <summary>Endpoint used by cookie middleware when access is denied</summary>
    [HttpGet("forbidden")]
    public IActionResult ForbiddenEndpoint() => Forbid();
}

// If you DON'T already have a LoginRequest somewhere else, include this:
public sealed class LoginRequest
{
    public string UsernameOrEmail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
