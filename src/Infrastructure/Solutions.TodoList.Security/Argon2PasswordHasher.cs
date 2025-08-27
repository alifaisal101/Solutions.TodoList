using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;
using Microsoft.Extensions.Options;
using Solutions.TodoList.Application.Contracts.Security;

namespace Solutions.TodoList.Security;

public class Argon2PasswordHasher(IOptions<Argon2Settings> settings) : IPasswordHasher
{
    private readonly Argon2Settings _conf = settings.Value;

    public string Hash(string password)
    {
        ArgumentNullException.ThrowIfNull(password);

        var salt = RandomNumberGenerator.GetBytes(_conf.SaltSize);
        var argon = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            Iterations = _conf.Iterations,
            MemorySize = _conf.MemoryKb,
            DegreeOfParallelism = _conf.DegreeOfParallelism
        };
        var hash = argon.GetBytes(_conf.HashSize);
        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public bool Verify(string password, string hashed)
    {
        ArgumentNullException.ThrowIfNull(password);
        if (string.IsNullOrWhiteSpace(hashed)) return false;

        var parts = hashed.Split('.', 2);
        if (parts.Length != 2) return false;

        var salt = Convert.FromBase64String(parts[0]);
        var expected = Convert.FromBase64String(parts[1]);

        var argon = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            Iterations = _conf.Iterations,
            MemorySize = _conf.MemoryKb,
            DegreeOfParallelism = _conf.DegreeOfParallelism
        };
        var actual = argon.GetBytes(expected.Length);

        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }
}