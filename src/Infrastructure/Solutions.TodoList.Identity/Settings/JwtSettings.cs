using System.ComponentModel.DataAnnotations;

namespace Solutions.TodoList.Identity.Settings;

public class JwtSettings
{
    [Required]
    public string Issuer { get; set; } = "";

    [Required]
    public string Audience { get; set; } = "";

    [Range(1, 1440)]
    public int AccessTokenMinutes { get; set; } = 30;

    /// <summary>
    /// Symmetric (HMAC) secret (base64 or raw). Optional if UseRsa = true.
    /// </summary>
    public string? SymmetricKey { get; set; }

    // RSA PEM strings (optional if UseRsa=false)
    public string? RsaPrivateKeyPem { get; set; }
    public string? RsaPublicKeyPem { get; set; }

    /// <summary>
    /// If true, use RSA signing. If false, use symmetric HMAC.
    /// When running in Production we require UseRsa == true (see validator).
    /// </summary>
    public bool UseRsa { get; set; } = false;

    /// <summary>
    /// Number of days a refresh token stays valid (configurable).
    /// </summary>
    [Range(1, 3650)]
    public int RefreshTokenDays { get; set; } = 30;
}