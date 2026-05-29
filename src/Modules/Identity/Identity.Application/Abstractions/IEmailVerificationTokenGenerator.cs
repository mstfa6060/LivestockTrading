namespace LivestockTrading.Identity.Application.Abstractions;

/// <summary>
/// Application port for opaque email verification token generation and hashing.
/// Generate: 32-byte RNG produces a hex-encoded raw token plus its SHA-256
/// byte[] hash (FixedTimeEquals comparison surface). Hash: deterministic SHA-256
/// byte[] for verify-time lookup. Concrete implementation lives in
/// Identity.Infrastructure (Wave 4 W4.3). Distinct from IRefreshTokenGenerator
/// because the verification hash rides on EmailVerificationTicket.CodeHash
/// (byte[] for CryptographicOperations.FixedTimeEquals) while refresh hashes
/// are stored as hex strings on RefreshToken.TokenHash.
/// </summary>
public interface IEmailVerificationTokenGenerator
{
    EmailVerificationTokenPair Generate();
    byte[] Hash(string rawToken);
}

public sealed record EmailVerificationTokenPair(string Raw, byte[] Hash);
