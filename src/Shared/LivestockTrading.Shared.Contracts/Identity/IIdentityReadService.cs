namespace Shared.Contracts.Identity;

/// <summary>Cross-modül kimlik okuma (05-identity §5 literal 10 metot). Cache: UserSummary 5dk, Devices 1dk, Role 30sn, Consent 5dk.</summary>
public interface IIdentityReadService
{
    Task<UserSummary?> GetUserSummaryAsync(Guid userId, CancellationToken ct);
    Task<IReadOnlyList<UserSummary>> GetUserSummariesAsync(IReadOnlyList<Guid> userIds, CancellationToken ct);

    Task<bool> UserExistsAsync(Guid userId, CancellationToken ct);
    Task<bool> UserHasRoleAsync(Guid userId, string role, CancellationToken ct);
    Task<bool> UserHasAnyRoleAsync(Guid userId, IReadOnlyList<string> roles, CancellationToken ct);
    Task<AccountType?> GetAccountTypeAsync(Guid userId, CancellationToken ct);

    Task<IReadOnlyList<DeviceInfo>> GetActiveDevicesAsync(Guid userId, CancellationToken ct);
    Task<IReadOnlyList<string>> GetActivePushTokensAsync(Guid userId, CancellationToken ct);

    Task<bool> HasActiveConsentAsync(Guid userId, ConsentType type, CancellationToken ct);
    Task<UserPreferences> GetUserPreferencesAsync(Guid userId, CancellationToken ct);
}
