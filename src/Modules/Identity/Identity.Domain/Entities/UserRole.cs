namespace LivestockTrading.Identity.Domain.Entities;

using Shared.Domain;

/// <summary>
/// User role grant entity. Revoke RevokedAt+RevokedByUserId set ile yapilir.
/// Plan-doc 05-identity §3 satir 266-285 birebir.
/// </summary>
public sealed class UserRole : Entity
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Role { get; private set; }
    public DateTimeOffset GrantedAt { get; private set; }
    public Guid? GrantedByUserId { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public Guid? RevokedByUserId { get; private set; }

    // EF Core
    private UserRole()
    {
        Role = null!;
    }

    // Sadece User AR cagirir - internal.
    internal UserRole(
        Guid userId,
        string role,
        DateTimeOffset grantedAt,
        Guid? grantedByUserId)
    {
        Id = Guid.CreateVersion7();
        UserId = userId;
        Role = role;
        GrantedAt = grantedAt;
        GrantedByUserId = grantedByUserId;
    }

    public bool IsActive => RevokedAt is null;

    internal void Revoke(Guid revokedBy, DateTimeOffset now)
    {
        if (RevokedAt is not null) return; // idempotent
        RevokedAt = now;
        RevokedByUserId = revokedBy;
    }

    protected override object IdentityValue => Id;
    protected override bool IsTransient => Id == Guid.Empty;
}
