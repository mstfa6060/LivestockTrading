namespace BaseModules.IAM.Infrastructure.Services;

public class GeoLocationResult
{
    public string CountryCode { get; set; }
    public string CountryName { get; set; }
    public string Currency { get; set; }
    public string Language { get; set; }
    public string Timezone { get; set; }
}

public interface IGeoLocationService
{
    GeoLocationResult DetectCountry(string ipAddress);
}

public class GeoLocationService : IGeoLocationService
{
    private static readonly Dictionary<string, GeoLocationResult> _countryMetadata = new()
    {
        ["TR"] = new GeoLocationResult { CountryCode = "TR", CountryName = "Turkey", Currency = "TRY", Language = "tr", Timezone = "Europe/Istanbul" },
        ["US"] = new GeoLocationResult { CountryCode = "US", CountryName = "United States", Currency = "USD", Language = "en", Timezone = "America/New_York" },
        ["GB"] = new GeoLocationResult { CountryCode = "GB", CountryName = "United Kingdom", Currency = "GBP", Language = "en", Timezone = "Europe/London" },
        ["DE"] = new GeoLocationResult { CountryCode = "DE", CountryName = "Germany", Currency = "EUR", Language = "de", Timezone = "Europe/Berlin" },
        ["FR"] = new GeoLocationResult { CountryCode = "FR", CountryName = "France", Currency = "EUR", Language = "fr", Timezone = "Europe/Paris" },
        ["ES"] = new GeoLocationResult { CountryCode = "ES", CountryName = "Spain", Currency = "EUR", Language = "es", Timezone = "Europe/Madrid" },
        ["IT"] = new GeoLocationResult { CountryCode = "IT", CountryName = "Italy", Currency = "EUR", Language = "it", Timezone = "Europe/Rome" },
        ["NL"] = new GeoLocationResult { CountryCode = "NL", CountryName = "Netherlands", Currency = "EUR", Language = "nl", Timezone = "Europe/Amsterdam" },
        ["RU"] = new GeoLocationResult { CountryCode = "RU", CountryName = "Russia", Currency = "RUB", Language = "ru", Timezone = "Europe/Moscow" },
        ["CN"] = new GeoLocationResult { CountryCode = "CN", CountryName = "China", Currency = "CNY", Language = "zh", Timezone = "Asia/Shanghai" },
        ["JP"] = new GeoLocationResult { CountryCode = "JP", CountryName = "Japan", Currency = "JPY", Language = "ja", Timezone = "Asia/Tokyo" },
        ["KR"] = new GeoLocationResult { CountryCode = "KR", CountryName = "South Korea", Currency = "KRW", Language = "ko", Timezone = "Asia/Seoul" },
        ["AU"] = new GeoLocationResult { CountryCode = "AU", CountryName = "Australia", Currency = "AUD", Language = "en", Timezone = "Australia/Sydney" },
        ["CA"] = new GeoLocationResult { CountryCode = "CA", CountryName = "Canada", Currency = "CAD", Language = "en", Timezone = "America/Toronto" },
        ["BR"] = new GeoLocationResult { CountryCode = "BR", CountryName = "Brazil", Currency = "BRL", Language = "pt", Timezone = "America/Sao_Paulo" },
        ["IN"] = new GeoLocationResult { CountryCode = "IN", CountryName = "India", Currency = "INR", Language = "hi", Timezone = "Asia/Kolkata" },
        ["MX"] = new GeoLocationResult { CountryCode = "MX", CountryName = "Mexico", Currency = "MXN", Language = "es", Timezone = "America/Mexico_City" },
        ["AR"] = new GeoLocationResult { CountryCode = "AR", CountryName = "Argentina", Currency = "ARS", Language = "es", Timezone = "America/Buenos_Aires" },
        ["ZA"] = new GeoLocationResult { CountryCode = "ZA", CountryName = "South Africa", Currency = "ZAR", Language = "en", Timezone = "Africa/Johannesburg" },
        ["EG"] = new GeoLocationResult { CountryCode = "EG", CountryName = "Egypt", Currency = "EGP", Language = "ar", Timezone = "Africa/Cairo" },
        ["SA"] = new GeoLocationResult { CountryCode = "SA", CountryName = "Saudi Arabia", Currency = "SAR", Language = "ar", Timezone = "Asia/Riyadh" },
        ["AE"] = new GeoLocationResult { CountryCode = "AE", CountryName = "United Arab Emirates", Currency = "AED", Language = "ar", Timezone = "Asia/Dubai" },
        ["PL"] = new GeoLocationResult { CountryCode = "PL", CountryName = "Poland", Currency = "PLN", Language = "pl", Timezone = "Europe/Warsaw" },
        ["CH"] = new GeoLocationResult { CountryCode = "CH", CountryName = "Switzerland", Currency = "CHF", Language = "de", Timezone = "Europe/Zurich" },
        ["NZ"] = new GeoLocationResult { CountryCode = "NZ", CountryName = "New Zealand", Currency = "NZD", Language = "en", Timezone = "Pacific/Auckland" },
        ["SE"] = new GeoLocationResult { CountryCode = "SE", CountryName = "Sweden", Currency = "SEK", Language = "sv", Timezone = "Europe/Stockholm" },
        ["NO"] = new GeoLocationResult { CountryCode = "NO", CountryName = "Norway", Currency = "NOK", Language = "no", Timezone = "Europe/Oslo" },
        ["DK"] = new GeoLocationResult { CountryCode = "DK", CountryName = "Denmark", Currency = "DKK", Language = "da", Timezone = "Europe/Copenhagen" },
        ["AT"] = new GeoLocationResult { CountryCode = "AT", CountryName = "Austria", Currency = "EUR", Language = "de", Timezone = "Europe/Vienna" },
        ["BE"] = new GeoLocationResult { CountryCode = "BE", CountryName = "Belgium", Currency = "EUR", Language = "nl", Timezone = "Europe/Brussels" },
    };

    private readonly string _geoDbPath;

    public GeoLocationService()
    {
        _geoDbPath = "/app/data/GeoIP2-Country.mmdb";
    }

    public GeoLocationResult DetectCountry(string ipAddress)
    {
        // Default fallback
        var defaultResult = _countryMetadata["TR"];

        if (string.IsNullOrWhiteSpace(ipAddress))
            return defaultResult;

        try
        {
            // Try MaxMind GeoIP2 database if available
            if (System.IO.File.Exists(_geoDbPath))
            {
                using var reader = new MaxMind.GeoIP2.DatabaseReader(_geoDbPath);
                if (reader.TryCountry(ipAddress, out var response))
                {
                    var countryCode = response.Country.IsoCode;
                    if (!string.IsNullOrEmpty(countryCode) && _countryMetadata.TryGetValue(countryCode, out var result))
                        return result;

                    // Known country code but not in our metadata - return basic info
                    if (!string.IsNullOrEmpty(countryCode))
                    {
                        return new GeoLocationResult
                        {
                            CountryCode = countryCode,
                            CountryName = response.Country.Name ?? countryCode,
                            Currency = "USD",
                            Language = "en",
                            Timezone = "UTC"
                        };
                    }
                }
            }
        }
        catch
        {
            // If MaxMind fails, fall through to default
        }

        return defaultResult;
    }

    public static GeoLocationResult GetCountryMetadata(string countryCode)
    {
        if (string.IsNullOrEmpty(countryCode))
            return _countryMetadata["TR"];

        return _countryMetadata.TryGetValue(countryCode.ToUpperInvariant(), out var result)
            ? result
            : _countryMetadata["TR"];
    }
}
