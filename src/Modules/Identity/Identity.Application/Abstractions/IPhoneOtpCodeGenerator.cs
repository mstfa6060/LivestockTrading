namespace LivestockTrading.Identity.Application.Abstractions;

/// <summary>
/// Application port for phone OTP code generation and hashing. Concrete
/// implementation lives in Identity.Infrastructure (Wave 4 W4.3) and is
/// distinct from IEmailVerificationTokenGenerator because the OTP is a 6-digit
/// numeric string sent over SMS (human-typeable), whereas email verification
/// uses a 32-byte opaque hex token (link-clickable). Hash discipline:
/// SHA256.HashData(Encoding.UTF8.GetBytes(rawCode)) -> byte[32]; Generate and
/// Hash MUST use the same algorithm. The hash is stored on
/// PhoneVerificationTicket.CodeHash (byte[] for CryptographicOperations
/// .FixedTimeEquals); plan-doc 05-identity.md §3 line 288 ("byte[] CodeHash;
/// // SHA-256 of 6-digit OTP") is the canonical contract.
/// </summary>
public interface IPhoneOtpCodeGenerator
{
    /// <summary>
    /// 6-digit numeric raw kod uretir (zero-padded "000000".."999999"). Hash =
    /// SHA256.HashData(Encoding.UTF8.GetBytes(Raw)) (byte[32]). Generate ve
    /// Hash AYNI algoritmayi kullanmalidir; W4.3 concrete bu kontrata UYMAK
    /// ZORUNDADIR (round-trip test: Hash(Generate().Raw) == Generate().Hash).
    /// </summary>
    PhoneOtpCodePair Generate();

    /// <summary>
    /// rawCode (6-digit numeric) icin SHA256.HashData(Encoding.UTF8.GetBytes(rawCode))
    /// -> byte[32]. Generate'in Hash uretimiyle BIREBIR ayni algoritma.
    /// </summary>
    byte[] Hash(string rawCode);
}

public sealed record PhoneOtpCodePair(string Raw, byte[] Hash);
