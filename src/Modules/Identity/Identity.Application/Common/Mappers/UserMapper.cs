using LivestockTrading.Identity.Domain.Aggregates;
using Shared.Contracts.Identity;

namespace LivestockTrading.Identity.Application.Common.Mappers;

/// <summary>
/// Extension mapper from User aggregate to cross-module UserSummary projection.
/// Centralized here because Login and Refresh handlers both return LoginResponse
/// carrying a UserSummary — extracting the projection avoids duplication and
/// keeps the field mapping in a single auditable location (Catalog BorderRuleKindMapper
/// emsali, repeated-twice rule). DisplayName resolves to PersonName.FullName
/// (handles optional Middle); Locale and TimeZone come from UserPreferences.
/// </summary>
public static class UserMapper
{
    public static UserSummary ToSummary(this User user)
        => new(
            user.Id,
            user.Name.FullName,
            user.AvatarUrl,
            user.Preferences.Locale,
            user.Preferences.TimeZone,
            user.AccountType);
}
