namespace LivestockTrading.Identity.Domain.Aggregates;

using System.Security.Cryptography;
using LivestockTrading.Identity.Domain.Enums;
using LivestockTrading.Identity.Domain.ValueObjects;
using Shared.Domain;

/// <summary>
/// Telefon dogrulama bileti - User AR'dan bagimsiz standalone AR (plan-doc 05-identity §3 satir 279).
/// 6-digit OTP hash (SHA-256) + 5dk TTL + max 5 deneme. Register flow icin UserId null olabilir.
/// Catalog Brand AR pattern emsali: public static factory + object initializer + Guid v7.
/// Plan-doc event YOK (§3 listelememis), Raise cagrisi yok.
/// </summary>
public sealed class PhoneVerificationTicket : AggregateRoot
{
    public Guid Id { get; private set; }
    public Guid? UserId { get; private set; }
    public PhoneNumber Phone { get; private set; }
    public PhonePurpose Purpose { get; private set; }
    public byte[] CodeHash { get; private set; }
    public DateTimeOffset IssuedAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset? ConsumedAt { get; private set; }
    public int AttemptCount { get; private set; }
    public string IpAddress { get; private set; }

    // EF Core
    private PhoneVerificationTicket()
    {
        Phone = default!;
        CodeHash = null!;
        IpAddress = null!;
    }

    public static PhoneVerificationTicket Issue(
        PhoneNumber phone,
        PhonePurpose purpose,
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

        return new PhoneVerificationTicket
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            Phone = phone,
            Purpose = purpose,
            CodeHash = codeHash,
            IssuedAt = now,
            ExpiresAt = now.Add(ttl),
            ConsumedAt = null,
            AttemptCount = 0,
            IpAddress = ipAddress.Trim(),
        };
    }

    /// <summary>Kullanici OTP girisini dener; eslesirse Consume. Eslesmezse AttemptCount++.</summary>
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
