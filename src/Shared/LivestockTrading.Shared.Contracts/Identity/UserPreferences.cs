namespace Shared.Contracts.Identity;

/// <summary>Kullanıcı tercih seti — cross-modül okuma (Messaging read-receipts, Notifications locale). Cache TTL 5dk.</summary>
public sealed record UserPreferences(
    string Locale,
    string CurrencyCode,
    string CountryCode,
    string TimeZone,
    bool ReadReceiptsEnabled,
    bool TypingIndicatorEnabled);
