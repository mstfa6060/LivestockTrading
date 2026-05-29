namespace LivestockTrading.Identity.Application.Abstractions;

/// <summary>
/// Application port for opaque refresh token generation and hashing.
/// Generate: 32-byte RNG produces a hex-encoded raw token plus its SHA-256 hash.
/// Hash: deterministic SHA-256 (no salt) — required for refresh/logout lookup by
/// hash without storing the raw value. Concrete implementation lives in
/// Identity.Infrastructure (Wave 4 W4.3) and uses RandomNumberGenerator +
/// SHA256.HashData from System.Security.Cryptography.
/// </summary>
public interface IRefreshTokenGenerator
{
    RefreshTokenPair Generate();
    string Hash(string rawToken);
}

public sealed record RefreshTokenPair(string Raw, string Hash);
