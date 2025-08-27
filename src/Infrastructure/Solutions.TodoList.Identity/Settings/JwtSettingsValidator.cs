using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Solutions.TodoList.Identity.Settings;

public class JwtSettingsValidator(IHostEnvironment env) : IValidateOptions<JwtSettings>
{
    public ValidateOptionsResult Validate(string? name, JwtSettings options)
    {
        if (string.IsNullOrWhiteSpace(options.Issuer) || string.IsNullOrWhiteSpace(options.Audience))
            return ValidateOptionsResult.Fail("JwtSettings: Issuer and Audience are required.");

        if (options.UseRsa)
        {
            if (string.IsNullOrWhiteSpace(options.RsaPrivateKeyPem))
                return ValidateOptionsResult.Fail("JwtSettings: UseRsa=true but RsaPrivateKeyPem is not provided.");
        }
        else
        {
            if (string.IsNullOrWhiteSpace(options.SymmetricKey))
                return ValidateOptionsResult.Fail("JwtSettings: SymmetricKey must be provided when UseRsa is false.");
        }

        if (env.IsProduction() && !options.UseRsa)
            return ValidateOptionsResult.Fail("In Production environment, JWT signing must use RSA. Set Jwt:UseRsa = true and provide RSA PEM keys.");

        return ValidateOptionsResult.Success;
    }
}