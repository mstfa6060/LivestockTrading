namespace Livestock.Domain.Entities;

public class UserPreference : BaseEntity
{
    public Guid UserId { get; set; }
    public string? PreferredCurrency { get; set; }
    public string? PreferredLanguage { get; set; }
    public string? CountryCode { get; set; }
    public string? TimeZone { get; set; }
    public bool EmailNotificationsEnabled { get; set; } = true;
    public bool SmsNotificationsEnabled { get; set; } = true;
    public bool PushNotificationsEnabled { get; set; } = true;
    public bool DarkModeEnabled { get; set; }
    public int ProductsPerPage { get; set; } = 20;
}
