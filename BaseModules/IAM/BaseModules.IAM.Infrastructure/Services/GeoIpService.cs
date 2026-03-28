using MaxMind.GeoIP2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BaseModules.IAM.Infrastructure.Services;

/// <summary>
/// MaxMind GeoLite2 ile IP adresinden ülke tespiti.
/// Singleton olarak register edilir — DatabaseReader thread-safe'tir.
/// </summary>
public class GeoIpService : IDisposable
{
    private DatabaseReader _reader;
    private readonly string _dbPath;
    private readonly ILogger<GeoIpService> _logger;

    public GeoIpService(IConfiguration configuration, ILogger<GeoIpService> logger)
    {
        _logger = logger;
        _dbPath = configuration["GeoIP:DatabasePath"] ?? "GeoData/GeoLite2-Country.mmdb";

        if (File.Exists(_dbPath))
        {
            _reader = new DatabaseReader(_dbPath);
            _logger.LogInformation("GeoIP database loaded from {Path}", _dbPath);
        }
        else
        {
            _logger.LogWarning("GeoIP database not found at {Path}. IP detection disabled.", _dbPath);
        }
    }

    /// <summary>
    /// IP adresinden ISO 3166-1 alpha-2 ülke kodu döner (örn: "TR", "US", "DE").
    /// Bulunamazsa veya hata olursa null döner.
    /// </summary>
    public string GetCountryCodeFromIp(string ipAddress)
    {
        if (_reader == null || string.IsNullOrWhiteSpace(ipAddress))
            return null;

        try
        {
            // Localhost ve private IP'leri atla
            if (ipAddress == "::1" || ipAddress == "127.0.0.1" || ipAddress.StartsWith("192.168.") || ipAddress.StartsWith("10."))
                return null;

            var response = _reader.Country(ipAddress);
            return response?.Country?.IsoCode;
        }
        catch (Exception ex)
        {
            _logger.LogDebug("GeoIP lookup failed for {IP}: {Error}", ipAddress, ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Haftalık güncelleme sonrası database'i yeniden yükler.
    /// Thread-safe: Interlocked.Exchange ile reader swap yapılır.
    /// </summary>
    public void ReloadDatabase()
    {
        if (!File.Exists(_dbPath))
        {
            _logger.LogWarning("GeoIP database file not found at {Path}. Cannot reload.", _dbPath);
            return;
        }

        try
        {
            var newReader = new DatabaseReader(_dbPath);
            var oldReader = Interlocked.Exchange(ref _reader, newReader);
            oldReader?.Dispose();
            _logger.LogInformation("GeoIP database reloaded successfully from {Path}", _dbPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reload GeoIP database from {Path}", _dbPath);
        }
    }

    public bool IsAvailable => _reader != null;

    public void Dispose()
    {
        _reader?.Dispose();
    }
}
