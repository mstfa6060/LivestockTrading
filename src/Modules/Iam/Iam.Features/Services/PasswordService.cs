using System.Security.Cryptography;
using System.Text;

namespace Iam.Features.Services;

public sealed class PasswordService : IPasswordService
{
    public (string Hash, string Salt) HashPassword(string password)
    {
        var salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        var hash = ComputeHash(password, salt);
        return (hash, salt);
    }

    public bool VerifyPassword(string password, string hash, string salt)
    {
        var computed = ComputeHash(password, salt);
        return string.Equals(computed, hash, StringComparison.Ordinal);
    }

    private static string ComputeHash(string password, string salt)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password + salt));
        return Convert.ToBase64String(bytes);
    }
}
