using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Solutions.TodoList.Application.Contracts.Identity;
using Solutions.TodoList.Domain.Entities;
using Solutions.TodoList.Identity.Settings;

namespace Solutions.TodoList.Identity.Services;

public class JwtTokenService(IOptions<JwtSettings> settings) : ITokenService
{
    private readonly JwtSettings _settings = settings.Value;

    public (string accessToken, string refreshToken) CreateTokens(User user)
    {
        if (string.IsNullOrWhiteSpace(_settings.SymmetricKey))
            throw new InvalidOperationException("SymmetricKey is not configured in JwtSettings.");

        var keyBytes = Encoding.UTF8.GetBytes(_settings.SymmetricKey);
        var key = new SymmetricSecurityKey(keyBytes);
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.AccessTokenMinutes),
            signingCredentials: creds
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        var randomBytes = RandomNumberGenerator.GetBytes(64);
        var refreshToken = Base64UrlEncode(randomBytes);

        return (accessToken, refreshToken);
    }

    public string HashRefreshToken(string refreshToken)
    {
        var bytes = Encoding.UTF8.GetBytes(refreshToken);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }

    private static string Base64UrlEncode(byte[] bytes)
    {
        var s = Convert.ToBase64String(bytes);
        return s.TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }
}