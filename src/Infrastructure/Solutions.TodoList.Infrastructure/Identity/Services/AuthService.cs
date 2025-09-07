using Microsoft.Extensions.Options;
using Solutions.TodoList.Application.Contracts.Identity;
using Solutions.TodoList.Application.Contracts.Repositories;
using Solutions.TodoList.Application.Contracts.Security;
using Solutions.TodoList.Application.Requests.Auth;
using Solutions.TodoList.Domain.Dtos;
using Solutions.TodoList.Domain.Entities;
using Solutions.TodoList.Domain.Enums;
using Solutions.TodoList.Infrastructure.Identity.Settings;

namespace Solutions.TodoList.Infrastructure.Identity.Services;

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

        return await IssueAuthAsync(user);
    }

    public async Task<AuthDto> LoginAsync(LoginRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await _usersRepo.GetByUsernameAsync(request.Username)
                   ?? throw new UnauthorizedAccessException("Invalid credentials.");

        if (!_hasher.Verify(request.Password, user.EncryptedPassword))
            throw new UnauthorizedAccessException("Invalid credentials.");

        return await IssueAuthAsync(user);
    }

    public async Task<AuthDto> RefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new ArgumentException("Refresh token is required.", nameof(refreshToken));

        var stored = await _refreshRepo.GetByTokenAsync(_tokens.HashRefreshToken(refreshToken))
                     ?? throw new UnauthorizedAccessException("Invalid refresh token.");

        if (stored.ExpiresAtUtc < DateTime.UtcNow || stored.RevokedAtUtc != null)
            throw new UnauthorizedAccessException("Invalid refresh token.");

        await _refreshRepo.RevokeAsync(stored);

        var user = await _usersRepo.GetByIdAsync(stored.UserId)
                   ?? throw new InvalidOperationException("User not found.");

        return await IssueAuthAsync(user);
    }

    public async Task LogoutAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return;

        var stored = await _refreshRepo.GetByTokenAsync(_tokens.HashRefreshToken(refreshToken));
        if (stored is { RevokedAtUtc: null })
            await _refreshRepo.RevokeAsync(stored);
    }

    private async Task<AuthDto> IssueAuthAsync(User user)
    {
        var (accessToken, refreshToken) = _tokens.CreateTokens(user);
        var expiresAtUtc = DateTime.UtcNow.Add(_refreshTokenLifetime);
        var hashedRefresh = _tokens.HashRefreshToken(refreshToken);
        await _refreshRepo.AddAsync(new RefreshToken(user.Id, hashedRefresh, expiresAtUtc));

        return new AuthDto
        {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role.ToString(),
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAtUtc = expiresAtUtc
        };
    }
}