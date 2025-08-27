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

public class RsaJwtTokenService : ITokenService
{
    private readonly JwtSettings _settings;
    private readonly RsaSecurityKey _privateKey;
    private readonly RsaSecurityKey? _publicKey;

    public RsaJwtTokenService(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;

        if (string.IsNullOrWhiteSpace(_settings.RsaPrivateKeyPem))
            throw new InvalidOperationException("RsaPrivateKeyPem must be provided for RSA JWT signing.");

        var rsa = RSA.Create();
        rsa.ImportFromPem(_settings.RsaPrivateKeyPem.ToCharArray());
        _privateKey = new RsaSecurityKey(rsa);

        if (string.IsNullOrWhiteSpace(_settings.RsaPublicKeyPem)) return;
        
        var rsaPub = RSA.Create();
        rsaPub.ImportFromPem(_settings.RsaPublicKeyPem.ToCharArray());
        _publicKey = new RsaSecurityKey(rsaPub);
    }

    public (string accessToken, string refreshToken) CreateTokens(User user)
    {
        var creds = new SigningCredentials(_privateKey, SecurityAlgorithms.RsaSha256);

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
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        return (accessToken, refreshToken);
    }

    public string HashRefreshToken(string refreshToken)
    {
        var bytes = Encoding.UTF8.GetBytes(refreshToken);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}