namespace LivestockTrading.Identity.Domain.Entities;

using Shared.Domain;

/// <summary>
/// User device child entity (User AR collection). Push token + last seen tracking.
/// Plan-doc 05-identity §3 satir 221-250 birebir.
/// </summary>
public sealed class UserDevice : Entity
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Platform { get; private set; }
    public string? UserAgent { get; private set; }
    public string DeviceFingerprint { get; private set; }
    public string? PushToken { get; private set; }
    public DateTimeOffset RegisteredAt { get; private set; }
    public DateTimeOffset LastSeenAt { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset? PushTokenInvalidatedAt { get; private set; }

    // EF Core
    private UserDevice()
    {
        Platform = null!;
        DeviceFingerprint = null!;
    }

    // Sadece User AR cagirir - internal.
    internal UserDevice(
        Guid userId,
        string platform,
        string? userAgent,
        string deviceFingerprint,
        string? pushToken,
        DateTimeOffset registeredAt)
    {
        Id = Guid.CreateVersion7();
        UserId = userId;
        Platform = platform;
        UserAgent = userAgent;
        DeviceFingerprint = deviceFingerprint;
        PushToken = pushToken;
        RegisteredAt = registeredAt;
        LastSeenAt = registeredAt;
        IsActive = true;
    }

    internal void UpdatePushToken(string? newToken)
    {
        PushToken = newToken;
        IsActive = true;
        PushTokenInvalidatedAt = null;
    }

    internal void InvalidatePushToken(DateTimeOffset now)
    {
        PushTokenInvalidatedAt = now;
        IsActive = false;
    }

    internal void Touch(DateTimeOffset now)
    {
        LastSeenAt = now;
    }

    protected override object IdentityValue => Id;
    protected override bool IsTransient => Id == Guid.Empty;
}
