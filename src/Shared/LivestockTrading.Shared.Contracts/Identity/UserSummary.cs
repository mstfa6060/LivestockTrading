namespace Shared.Contracts.Identity;

/// <summary>Cross-modül kullanıcı özeti (Listings/Marketplace/Messaging/Notifications consumer). Cache TTL 5dk.</summary>
public sealed record UserSummary(
    Guid UserId,
    string DisplayName,
    string? AvatarUrl,
    string Locale,
    string TimeZone,
    AccountType AccountType);
