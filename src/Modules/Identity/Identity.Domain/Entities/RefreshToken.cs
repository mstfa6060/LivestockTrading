namespace LivestockTrading.Identity.Domain.Entities;

using LivestockTrading.Identity.Domain.Enums;
using Shared.Domain;

/// <summary>
/// Refresh token child entity (User AR icindeki collection). Rotation chain + reuse detection.
/// Plan-doc 05-identity §3 satir 187-220 birebir. Davranis User AR'da, burada state mutation only.
/// </summary>
public sealed class RefreshToken : Entity
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid DeviceId { get; private set; }
    public string TokenHash { get; private set; }
    public Guid? ParentTokenId { get; private set; }
    public Guid FamilyId { get; private set; }
    public DateTimeOffset IssuedAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public RevocationReason? RevocationReason { get; private set; }
    public string? ReplacedByTokenId { get; private set; }

    // EF Core
    private RefreshToken()
    {
        TokenHash = null!;
    }

    // Sadece User AR cagirir (rotation/issue) - internal.
    internal RefreshToken(
        Guid userId,
        Guid deviceId,
        string tokenHash,
        Guid? parentTokenId,
        Guid familyId,
        DateTimeOffset issuedAt,
        DateTimeOffset expiresAt)
    {
        Id = Guid.CreateVersion7();
        UserId = userId;
        DeviceId = deviceId;
        TokenHash = tokenHash;
        ParentTokenId = parentTokenId;
        FamilyId = familyId;
        IssuedAt = issuedAt;
        ExpiresAt = expiresAt;
    }

    public bool IsActive(DateTimeOffset now) => RevokedAt is null && ExpiresAt > now;

    internal void Revoke(RevocationReason reason, DateTimeOffset now, string? replacedByTokenId = null)
    {
        if (RevokedAt is not null) return; // idempotent
        RevokedAt = now;
        RevocationReason = reason;
        ReplacedByTokenId = replacedByTokenId;
    }

    protected override object IdentityValue => Id;
    protected override bool IsTransient => Id == Guid.Empty;
}
