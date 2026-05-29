namespace LivestockTrading.Identity.Domain.ValueObjects;

using Microsoft.AspNetCore.Identity;
using Shared.Domain;

/// <summary>
/// Hashed password VO. Pure Domain - IPasswordHasher&lt;TUser&gt; dependency parametre olarak gelir, VO icinde tutulmaz.
/// PBKDF2-SHA256 (Microsoft.Extensions.Identity.Core 10.0.8 PasswordHasher&lt;TUser&gt; impl).
/// W4.0 B-W4.1-1: Frontend onayli pure VO + parameter injection pattern.
/// </summary>
public readonly record struct HashedPassword
{
    private readonly string? _value;

    public string Value => _value ?? throw new DomainException(
        "HashedPassword default(struct) instance - kullanim hatasi.");

    private HashedPassword(string hashedValue)
    {
        _value = hashedValue;
    }

    /// <summary>
    /// Plaintext password'u hash'leyerek HashedPassword olusturur.
    /// Caller IPasswordHasher&lt;TUser&gt; impl gecmeli (DI'dan).
    /// </summary>
    public static HashedPassword Create<TUser>(string plaintext, IPasswordHasher<TUser> hasher)
        where TUser : class
    {
        if (string.IsNullOrEmpty(plaintext))
            throw new DomainException("Password bos olamaz.");
        if (plaintext.Length < 8)
            throw new DomainException("Password en az 8 karakter olmali.");
        if (plaintext.Length > 256)
            throw new DomainException("Password 256 karakteri asamaz.");

        // hasher.HashPassword TUser parametresi gerektirir ama null gecmek
        // PasswordHasher<TUser> impl'inde sorun yaratmaz (impl user'i kullanmaz).
        var hash = hasher.HashPassword(null!, plaintext);
        return new HashedPassword(hash);
    }

    /// <summary>
    /// Plaintext password'un bu hash ile eslesip eslesmedigini dogrular.
    /// Rehash gereken durum (success-rehash-needed) basari sayilir.
    /// </summary>
    public bool Verify<TUser>(string plaintext, IPasswordHasher<TUser> hasher)
        where TUser : class
    {
        if (string.IsNullOrEmpty(plaintext))
            return false;

        var result = hasher.VerifyHashedPassword(null!, Value, plaintext);
        return result == PasswordVerificationResult.Success
            || result == PasswordVerificationResult.SuccessRehashNeeded;
    }

    public override string ToString() => "[HashedPassword]"; // Hash gosterme - log/serialize guvenligi
}
