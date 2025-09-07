using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Solutions.TodoList.Application.Contracts.Identity;
using Solutions.TodoList.Application.Requests.Auth;
using Solutions.TodoList.Domain.Dtos;
using Solutions.TodoList.WebApi.Auth;

namespace Solutions.TodoList.WebApi.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private const string RefreshCookieName = "refreshToken";
    private const string RefreshCookiePath = "/api/v1/auth";

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        try
        {
            return Ok(IssueTokens(await authService.RegisterAsync(request)));
        }
        catch (Exception ex)
        {
            return Failure(ex);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        try
        {
            return Ok(IssueTokens(await authService.LoginAsync(request)));
        }
        catch (Exception ex)
        {
            return Failure(ex);
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies[RefreshCookieName];
        if (string.IsNullOrWhiteSpace(refreshToken))
            return Unauthorized(new { error = "Missing refresh token." });

        try
        {
            return Ok(IssueTokens(await authService.RefreshTokenAsync(refreshToken)));
        }
        catch (Exception ex)
        {
            return Failure(ex);
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies[RefreshCookieName];
        if (!string.IsNullOrWhiteSpace(refreshToken))
            await authService.LogoutAsync(refreshToken);

        Response.Cookies.Delete(RefreshCookieName, RefreshCookieOptions(DateTimeOffset.UtcNow));
        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me() => Ok(new
    {
        Id = User.FindFirstValue(ClaimTypes.NameIdentifier),
        Username = User.Identity?.Name,
        Role = User.FindFirstValue(ClaimTypes.Role)
    });

    private AuthResponse IssueTokens(AuthDto auth)
    {
        Response.Cookies.Append(
            RefreshCookieName,
            auth.RefreshToken,
            RefreshCookieOptions(auth.RefreshTokenExpiresAtUtc));

        return new AuthResponse(auth.Id, auth.Username, auth.Role, auth.AccessToken);
    }

    private static CookieOptions RefreshCookieOptions(DateTimeOffset expires) => new()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        Path = RefreshCookiePath,
        Expires = expires
    };

    private IActionResult Failure(Exception ex) =>
        ex is UnauthorizedAccessException
            ? Unauthorized(new { error = ex.Message })
            : BadRequest(new { error = ex.Message });
}
