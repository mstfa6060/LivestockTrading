namespace Livestock.Features.UserPreferences;

public record UserPreferenceDetail(
    Guid Id, Guid UserId,
    string? PreferredCurrency, string? PreferredLanguage,
    string? CountryCode, string? TimeZone,
    bool EmailNotificationsEnabled, bool SmsNotificationsEnabled, bool PushNotificationsEnabled,
    bool DarkModeEnabled, int ProductsPerPage, DateTime CreatedAt);

public record UpsertUserPreferenceRequest(
    string? PreferredCurrency, string? PreferredLanguage,
    string? CountryCode, string? TimeZone,
    bool EmailNotificationsEnabled, bool SmsNotificationsEnabled, bool PushNotificationsEnabled,
    bool DarkModeEnabled, int ProductsPerPage);
