using Shared.Contracts.Identity;

namespace LivestockTrading.Identity.Application.Features.Me;

/// <summary>
/// Self-service profile projection returned by GET /identity/users/me. Domain
/// value objects are flattened to primitives (EmailAddress.Value, PhoneNumber.E164,
/// NationalId.Value, PersonName split) so clients never see Domain types.
/// Consents carry a per-grant summary projection; the full UserConsent entity
/// (with audit IP/UA) stays inside the aggregate.
/// </summary>
public sealed record MeProfile(
    Guid UserId,
    string Email,
    bool EmailVerified,
    string? Phone,
    bool PhoneVerified,
    string FirstName,
    string LastName,
    string? NationalId,
    AccountType AccountType,
    UserStatus Status,
    UserPreferences Preferences,
    string? AvatarUrl,
    DateTimeOffset CreatedAt,
    DateTimeOffset? LastLoginAt,
    IReadOnlyList<string> Roles,
    IReadOnlyList<UserConsentSummary> Consents);

public sealed record UserConsentSummary(
    ConsentType Type,
    string Version,
    bool Granted,
    DateTimeOffset GrantedAt);

/// <summary>
/// Session list item returned by GET /identity/users/me/sessions. Combines
/// the refresh token (issued/expires) with the device it lives on. IsCurrent
/// stays false for Faz 1: the read service has no way to know which refresh
/// token belongs to the calling request — the access JWT in flight carries
/// no refresh hash and host-auth (W4.4) is what surfaces the active session
/// identity. Backlog item: enrich IsCurrent once host-auth wiring lands.
/// </summary>
public sealed record SessionInfo(
    Guid SessionId,
    Guid DeviceId,
    string Platform,
    string? UserAgent,
    string IpAddress,
    DateTimeOffset IssuedAt,
    DateTimeOffset ExpiresAt,
    bool IsCurrent);
