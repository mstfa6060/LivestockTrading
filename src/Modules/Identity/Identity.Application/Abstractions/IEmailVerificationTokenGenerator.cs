namespace LivestockTrading.Identity.Application.Abstractions;

/// <summary>
/// Application port for opaque email verification token generation and hashing.
/// Concrete implementation lives in Identity.Infrastructure (Wave 4 W4.3) and
/// is distinct from IRefreshTokenGenerator because the verification hash rides
/// on EmailVerificationTicket.CodeHash (byte[] for CryptographicOperations
/// .FixedTimeEquals) while refresh hashes are stored as hex strings on
/// RefreshToken.TokenHash.
/// </summary>
public interface IEmailVerificationTokenGenerator
{
    /// <summary>
    /// 32-byte rastgele token uretir. Raw = Convert.ToHexString(bytes) (64-char hex).
    /// Hash = SHA256.HashData(Convert.FromHexString(Raw)) (32-byte). Generate ve Hash
    /// AYNI algoritmayi kullanmalidir: raw hex string once Convert.FromHexString ile
    /// 32 byte'a cozulur, sonra SHA-256 alinir. Bu kontrat Identity.Domain
    /// RequestEmailChange (PendingEmailTokenHash) ile birebir ayni olmali; aksi halde
    /// email-change confirm ve email-verify FixedTimeEquals sessiz fail eder.
    /// W4.3 concrete bu algoritmaya UYMAK ZORUNDADIR.
    /// </summary>
    EmailVerificationTokenPair Generate();

    /// <summary>
    /// rawToken (64-char hex) icin Convert.FromHexString(rawToken) -> SHA256.HashData(bytes)
    /// -> byte[32]. Generate'in Hash uretimiyle ve Identity.Domain RequestEmailChange
    /// hash algoritmasiyla BIREBIR ayni. UTF8.GetBytes KULLANILMAZ (hex-decode zorunlu).
    /// </summary>
    byte[] Hash(string rawToken);
}

public sealed record EmailVerificationTokenPair(string Raw, byte[] Hash);
