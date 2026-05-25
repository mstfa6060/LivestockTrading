namespace Shared.Contracts.Identity.Admin;

/// <summary>Admin kullanıcı detay projeksiyonu (05-identity §12 GET /admin/users/{id}). ~12 property — audit alanları dahil.</summary>
public sealed record UserDetail(
    Guid UserId,
    string DisplayName,
    string Email,
    bool EmailVerified,
    string? Phone,
    bool PhoneVerified,
    AccountType AccountType,
    UserStatus Status,
    IReadOnlyList<string> Roles,
    string? SuspendedReason,
    DateTimeOffset? SuspendedAt,
    DateTimeOffset? LastLoginAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
