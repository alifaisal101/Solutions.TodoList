using Microsoft.Extensions.Options;
using Solutions.TodoList.Application.Contracts.Identity;
using Solutions.TodoList.Application.Contracts.Repositories;
using Solutions.TodoList.Application.Contracts.Security;
using Solutions.TodoList.Application.Requests.Auth;
using Solutions.TodoList.Domain.Dtos;
using Solutions.TodoList.Domain.Entities;
using Solutions.TodoList.Domain.Enums;
using Solutions.TodoList.Identity.Settings;

namespace Solutions.TodoList.Identity.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _usersRepo;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokens;
    private readonly IRefreshTokenRepository _refreshRepo;
    private readonly TimeSpan _refreshTokenLifetime;

    public AuthService(
        IUserRepository usersRepo,
        IPasswordHasher hasher,
        ITokenService tokens,
        IRefreshTokenRepository refreshRepo,
        IOptions<JwtSettings> jwtOptions)
    {
        _usersRepo = usersRepo;
        _hasher = hasher;
        _tokens = tokens;
        _refreshRepo = refreshRepo;

        var opts = jwtOptions.Value;
        _refreshTokenLifetime = TimeSpan.FromDays(opts.RefreshTokenDays);
    }

    public async Task<AuthDto> RegisterAsync(RegisterRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (await _usersRepo.GetByUsernameAsync(request.Username) != null)
            throw new InvalidOperationException("Username already registered.");

        var hashed = _hasher.Hash(request.Password);
        var user = new User(request.Username, hashed, UserRole.User);
        await _usersRepo.AddAsync(user);

        var (accessToken, refreshToken) = _tokens.CreateTokens(user);
        var hashedRefresh = _tokens.HashRefreshToken(refreshToken);

        var rt = new RefreshToken(user.Id, hashedRefresh, DateTime.UtcNow.Add(_refreshTokenLifetime));
        await _refreshRepo.AddAsync(rt);

        return new AuthDto
        {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role.ToString(),
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<AuthDto> LoginAsync(LoginRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await _usersRepo.GetByUsernameAsync(request.Username)
                   ?? throw new UnauthorizedAccessException("Invalid credentials.");

        if (!_hasher.Verify(request.Password, user.EncryptedPassword))
            throw new UnauthorizedAccessException("Invalid credentials.");

        var (accessToken, refreshToken) = _tokens.CreateTokens(user);
        var hashedRefresh = _tokens.HashRefreshToken(refreshToken);

        var rt = new RefreshToken(user.Id, hashedRefresh, DateTime.UtcNow.Add(_refreshTokenLifetime));
        await _refreshRepo.AddAsync(rt);

        return new AuthDto
        {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role.ToString(),
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<AuthDto> RefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new ArgumentException("Refresh token is required.", nameof(refreshToken));

        var hashed = _tokens.HashRefreshToken(refreshToken);
        var stored = await _refreshRepo.GetByTokenAsync(hashed)
                      ?? throw new UnauthorizedAccessException("Invalid refresh token.");

        if (stored.ExpiresAtUtc < DateTime.UtcNow || stored.RevokedAtUtc != null)
            throw new UnauthorizedAccessException("Invalid refresh token.");

        // Revoke the old token
        await _refreshRepo.RevokeAsync(stored);

        var user = await _usersRepo.GetByIdAsync(stored.UserId) ?? throw new InvalidOperationException("User not found.");

        // Issue new tokens and persist hashed refresh
        var (accessToken, newRefresh) = _tokens.CreateTokens(user);
        var newHashed = _tokens.HashRefreshToken(newRefresh);
        var newRt = new RefreshToken(user.Id, newHashed, DateTime.UtcNow.Add(_refreshTokenLifetime));
        await _refreshRepo.AddAsync(newRt);

        return new AuthDto
        {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role.ToString(),
            AccessToken = accessToken,
            RefreshToken = newRefresh
        };
    }
}