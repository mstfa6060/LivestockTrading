namespace Shared.Contracts.Identity;

/// <summary>Kullanıcı cihaz özeti (push token dağıtımı için). Cache TTL 1dk.</summary>
public sealed record DeviceInfo(
    Guid DeviceId,
    string Platform,
    string? PushToken,
    DateTimeOffset LastSeenAt);
