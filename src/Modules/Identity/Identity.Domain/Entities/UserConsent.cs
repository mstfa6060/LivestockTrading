namespace LivestockTrading.Identity.Domain.Entities;

using Shared.Contracts.Identity;
using Shared.Domain;

/// <summary>
/// Consent record (KVKK audit trail). B-W4.1-6: immutable her grant ayri entity.
/// Revoke ayni entity uzerinde RevokedAt set; sonraki grant yeni entity olarak yaratilir.
/// Plan-doc 05-identity §3 satir 286-302 birebir.
/// </summary>
public sealed class UserConsent : Entity
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public ConsentType Type { get; private set; }
    public string Version { get; private set; }
    public bool Granted { get; private set; }
    public DateTimeOffset GrantedAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public string IpAddress { get; private set; }
    public string UserAgent { get; private set; }

    // EF Core
    private UserConsent()
    {
        Version = null!;
        IpAddress = null!;
        UserAgent = null!;
    }

    // Sadece User AR cagirir - internal.
    internal UserConsent(
        Guid userId,
        ConsentType type,
        string version,
        bool granted,
        DateTimeOffset grantedAt,
        string ipAddress,
        string userAgent)
    {
        Id = Guid.CreateVersion7();
        UserId = userId;
        Type = type;
        Version = version;
        Granted = granted;
        GrantedAt = grantedAt;
        IpAddress = ipAddress;
        UserAgent = userAgent;
    }

    public bool IsActive => Granted && RevokedAt is null;

    internal void Revoke(DateTimeOffset now)
    {
        if (RevokedAt is not null) return; // idempotent
        RevokedAt = now;
    }

    protected override object IdentityValue => Id;
    protected override bool IsTransient => Id == Guid.Empty;
}
