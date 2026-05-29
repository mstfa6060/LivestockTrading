namespace LivestockTrading.Identity.Domain.Aggregates;

using System.Security.Cryptography;
using LivestockTrading.Identity.Domain.ValueObjects;
using Shared.Domain;

/// <summary>
/// Email dogrulama bileti - User AR'dan bagimsiz standalone AR
/// (PhoneVerificationTicket simetrik). 32-byte hex token + SHA-256 hash;
/// 24 saat TTL + max 5 deneme. Send-verify akisi (W4.2.C G3) tarafindan
/// uretilir, verify endpoint TryConsume cagrir. PhoneVerificationTicket
/// emsali PhonePurpose yok - email-verify tek amac.
/// </summary>
public sealed class EmailVerificationTicket : AggregateRoot
{
    public Guid Id { get; private set; }
    public Guid? UserId { get; private set; }
    public EmailAddress Email { get; private set; }
    public byte[] CodeHash { get; private set; }
    public DateTimeOffset IssuedAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset? ConsumedAt { get; private set; }
    public int AttemptCount { get; private set; }
    public string IpAddress { get; private set; }

    // EF Core
    private EmailVerificationTicket()
    {
        Email = default!;
        CodeHash = null!;
        IpAddress = null!;
    }

    public static EmailVerificationTicket Issue(
        EmailAddress email,
        byte[] codeHash,
        DateTimeOffset now,
        TimeSpan ttl,
        string ipAddress,
        Guid? userId = null)
    {
        if (codeHash is null || codeHash.Length == 0)
            throw new DomainException("CodeHash bos olamaz.");
        if (string.IsNullOrWhiteSpace(ipAddress))
            throw new DomainException("IpAddress bos olamaz.");
        if (ttl <= TimeSpan.Zero)
            throw new DomainException("TTL pozitif olmali.");

        return new EmailVerificationTicket
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            Email = email,
            CodeHash = codeHash,
            IssuedAt = now,
            ExpiresAt = now.Add(ttl),
            ConsumedAt = null,
            AttemptCount = 0,
            IpAddress = ipAddress.Trim(),
        };
    }

    /// <summary>Kullanici verify token'i dener; eslesirse Consume. Eslesmezse AttemptCount++.</summary>
    public bool TryConsume(byte[] providedCodeHash, DateTimeOffset now)
    {
        if (ConsumedAt is not null)
            throw new DomainException("Ticket zaten consumed.");
        if (now >= ExpiresAt)
            throw new DomainException("Ticket expired.");
        if (AttemptCount >= 5)
            throw new DomainException("Max attempt limit (5) asildi.");

        AttemptCount++;

        if (providedCodeHash is null
            || providedCodeHash.Length != CodeHash.Length
            || !CryptographicOperations.FixedTimeEquals(providedCodeHash, CodeHash))
            return false;

        ConsumedAt = now;
        return true;
    }

    public bool IsActive(DateTimeOffset now) =>
        ConsumedAt is null && now < ExpiresAt && AttemptCount < 5;

    protected override object IdentityValue => Id;
    protected override bool IsTransient => Id == Guid.Empty;
}
