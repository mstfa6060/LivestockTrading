using System;

namespace LivestockTrading.Domain.Entities;

/// <summary>
/// Kullanıcı tercihleri ve ayarları
/// </summary>
public class UserPreferences : BaseEntity
{
    public Guid UserId { get; set; }
    public string PreferredCurrency { get; set; }
    public string PreferredLanguage { get; set; }
    public string CountryCode { get; set; }
    public string TimeZone { get; set; }
    public MeasurementSystem WeightSystem { get; set; }
    public MeasurementSystem DistanceSystem { get; set; }
    public MeasurementSystem AreaSystem { get; set; }
    public bool EmailNotificationsEnabled { get; set; }
    public bool SmsNotificationsEnabled { get; set; }
    public bool PushNotificationsEnabled { get; set; }
    public bool DarkModeEnabled { get; set; }
    public int ProductsPerPage { get; set; }
    public ViewMode DefaultViewMode { get; set; }

    public UserPreferences()
    {
        PreferredCurrency = "USD";
        PreferredLanguage = "en";
        TimeZone = "UTC";
        WeightSystem = MeasurementSystem.Metric;
        DistanceSystem = MeasurementSystem.Metric;
        AreaSystem = MeasurementSystem.Metric;
        EmailNotificationsEnabled = true;
        SmsNotificationsEnabled = false;
        PushNotificationsEnabled = true;
        DarkModeEnabled = false;
        ProductsPerPage = 24;
        DefaultViewMode = ViewMode.Grid;
    }
}

public enum MeasurementSystem
{
    Metric = 0,
    Imperial = 1
}

public enum ViewMode
{
    Grid = 0,
    List = 1
}
