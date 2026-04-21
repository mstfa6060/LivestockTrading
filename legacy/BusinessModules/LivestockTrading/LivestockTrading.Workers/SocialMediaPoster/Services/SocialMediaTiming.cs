namespace LivestockTrading.Workers.SocialMediaPoster.Services;

/// <summary>
/// Decides the ideal posting time for a given country by mapping its ISO 3166-1
/// alpha-2 code to an IANA timezone + a local-time window when regional social
/// media engagement typically peaks. The handler delays posts until the next
/// occurrence of that window to maximize reach.
/// </summary>
public static class SocialMediaTiming
{
    public readonly record struct PostingWindow(string TimeZoneId, int LocalStartHour, int LocalEndHour);

    // Country → (IANA tz, localStart, localEnd). Windows reflect typical peak
    // engagement slots (lunch break for EMEA, evening for Americas / APAC).
    private static readonly IReadOnlyDictionary<string, PostingWindow> CountryConfig
        = new Dictionary<string, PostingWindow>(StringComparer.OrdinalIgnoreCase)
    {
        // Europe — lunch break peak
        ["TR"] = new("Europe/Istanbul", 12, 14),
        ["GB"] = new("Europe/London", 12, 14),
        ["DE"] = new("Europe/Berlin", 12, 14),
        ["FR"] = new("Europe/Paris", 12, 14),
        ["IT"] = new("Europe/Rome", 12, 14),
        ["ES"] = new("Europe/Madrid", 13, 15),
        ["NL"] = new("Europe/Amsterdam", 12, 14),
        ["PL"] = new("Europe/Warsaw", 12, 14),
        ["RU"] = new("Europe/Moscow", 12, 14),
        ["UA"] = new("Europe/Kiev", 12, 14),

        // Americas — after-work evening peak
        ["US"] = new("America/New_York", 18, 20),
        ["CA"] = new("America/Toronto", 18, 20),
        ["BR"] = new("America/Sao_Paulo", 19, 21),
        ["MX"] = new("America/Mexico_City", 19, 21),
        ["AR"] = new("America/Argentina/Buenos_Aires", 19, 21),
        ["CL"] = new("America/Santiago", 19, 21),
        ["CO"] = new("America/Bogota", 19, 21),

        // Asia — evening peak
        ["JP"] = new("Asia/Tokyo", 20, 22),
        ["CN"] = new("Asia/Shanghai", 20, 22),
        ["KR"] = new("Asia/Seoul", 20, 22),
        ["IN"] = new("Asia/Kolkata", 19, 21),
        ["ID"] = new("Asia/Jakarta", 19, 21),
        ["PH"] = new("Asia/Manila", 19, 21),
        ["TH"] = new("Asia/Bangkok", 19, 21),
        ["VN"] = new("Asia/Ho_Chi_Minh", 19, 21),

        // Middle East
        ["AE"] = new("Asia/Dubai", 19, 21),
        ["SA"] = new("Asia/Riyadh", 20, 22),
        ["IL"] = new("Asia/Jerusalem", 19, 21),
        ["IR"] = new("Asia/Tehran", 20, 22),

        // Oceania
        ["AU"] = new("Australia/Sydney", 18, 20),
        ["NZ"] = new("Pacific/Auckland", 18, 20),

        // Africa
        ["ZA"] = new("Africa/Johannesburg", 18, 20),
        ["EG"] = new("Africa/Cairo", 19, 21),
        ["NG"] = new("Africa/Lagos", 19, 21),
        ["KE"] = new("Africa/Nairobi", 19, 21),
        ["MA"] = new("Africa/Casablanca", 19, 21),
    };

    /// <summary>
    /// Returns the configured posting window for a country. Falls back to a
    /// mid-day UTC window when the country code is unknown.
    /// </summary>
    public static PostingWindow GetWindow(string countryCode)
    {
        if (!string.IsNullOrWhiteSpace(countryCode)
            && CountryConfig.TryGetValue(countryCode.Trim(), out var cfg))
        {
            return cfg;
        }
        return new PostingWindow("UTC", 12, 14);
    }

    /// <summary>
    /// Computes the next UTC instant to publish a post for the given country.
    /// If "now" already falls inside the local posting window, returns nowUtc.
    /// Otherwise returns the next window start (today or tomorrow) converted to UTC.
    /// Falls back to nowUtc on invalid timezone ids so posting still happens.
    /// </summary>
    public static DateTime ComputeNextPostTimeUtc(string countryCode, DateTime nowUtc)
    {
        var window = GetWindow(countryCode);

        TimeZoneInfo tz;
        try
        {
            tz = TimeZoneInfo.FindSystemTimeZoneById(window.TimeZoneId);
        }
        catch (TimeZoneNotFoundException)
        {
            return nowUtc;
        }
        catch (InvalidTimeZoneException)
        {
            return nowUtc;
        }

        var nowLocal = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, tz);

        // Inside the window → post immediately.
        if (nowLocal.Hour >= window.LocalStartHour && nowLocal.Hour < window.LocalEndHour)
        {
            return nowUtc;
        }

        // Next window start in local time (today or tomorrow).
        var targetLocal = nowLocal.Date.AddHours(window.LocalStartHour);
        if (nowLocal >= targetLocal)
        {
            targetLocal = targetLocal.AddDays(1);
        }

        // ConvertTimeToUtc expects Unspecified/Local kind; AddHours preserves Unspecified.
        return TimeZoneInfo.ConvertTimeToUtc(targetLocal, tz);
    }
}
