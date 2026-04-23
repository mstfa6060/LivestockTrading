using System.IO.Compression;
using Microsoft.Extensions.Options;
using Jobs.BackgroundJobs.HangfireScheduler.Models;

namespace Jobs.BackgroundJobs.HangfireScheduler.Jobs.IAM;

/// <summary>
/// MaxMind GeoLite2-Country veritabanını haftalık olarak günceller.
/// Pazartesi 03:00'te çalışır.
/// İndirir → Extract → .mmdb dosyasını shared path'e yazar
/// </summary>
public class UpdateGeoIpDatabaseJob
{
    private readonly IOptions<ApplicationSettings> _appSettings;
    private readonly ILogger<UpdateGeoIpDatabaseJob> _logger;
    private readonly HttpClient _httpClient;

    private const string EditionId = "GeoLite2-Country";

    public UpdateGeoIpDatabaseJob(
        IOptions<ApplicationSettings> appSettings,
        ILogger<UpdateGeoIpDatabaseJob> logger,
        HttpClient httpClient)
    {
        _appSettings = appSettings;
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task Process()
    {
        try
        {
            _logger.LogInformation("[UpdateGeoIpDatabaseJob] İş başladı - {Time}", DateTime.UtcNow);

            var licenseKey = Environment.GetEnvironmentVariable("MAXMIND_LICENSE_KEY");
            if (string.IsNullOrWhiteSpace(licenseKey))
            {
                _logger.LogWarning("[UpdateGeoIpDatabaseJob] MAXMIND_LICENSE_KEY env variable bulunamadı. Atlanıyor.");
                return;
            }

            // 1. MaxMind'dan .tar.gz indir
            var downloadUrl = $"https://download.maxmind.com/app/geoip_download?edition_id={EditionId}&license_key={licenseKey}&suffix=tar.gz";

            _logger.LogInformation("[UpdateGeoIpDatabaseJob] MaxMind'dan indiriliyor...");
            var response = await _httpClient.GetAsync(downloadUrl);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("[UpdateGeoIpDatabaseJob] İndirme başarısız: {StatusCode}", response.StatusCode);
                return;
            }

            var tarGzBytes = await response.Content.ReadAsByteArrayAsync();
            _logger.LogInformation("[UpdateGeoIpDatabaseJob] {Size}KB indirildi", tarGzBytes.Length / 1024);

            // 2. .tar.gz'den .mmdb dosyasını çıkar
            var mmdbBytes = ExtractMmdbFromTarGz(tarGzBytes);
            if (mmdbBytes == null)
            {
                _logger.LogError("[UpdateGeoIpDatabaseJob] .mmdb dosyası tar.gz içinde bulunamadı");
                return;
            }

            // 3. Dosyayı hedef path'e yaz
            // IAM API container'ın erişebileceği shared volume path
            var targetPath = _appSettings.Value.GeoIpDatabasePath
                ?? Environment.GetEnvironmentVariable("GEOIP_DATABASE_PATH")
                ?? "/app/GeoData/GeoLite2-Country.mmdb";

            var directory = Path.GetDirectoryName(targetPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            await File.WriteAllBytesAsync(targetPath, mmdbBytes);
            _logger.LogInformation("[UpdateGeoIpDatabaseJob] .mmdb dosyası yazıldı: {Path} ({Size}KB)",
                targetPath, mmdbBytes.Length / 1024);

            // 4. IAM API'ye reload sinyali gönder (opsiyonel)
            await NotifyIamApiToReload();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[UpdateGeoIpDatabaseJob] Exception oluştu");
        }
    }

    private byte[] ExtractMmdbFromTarGz(byte[] tarGzBytes)
    {
        try
        {
            using var gzStream = new GZipStream(new MemoryStream(tarGzBytes), CompressionMode.Decompress);
            using var tarStream = new MemoryStream();
            gzStream.CopyTo(tarStream);
            tarStream.Position = 0;

            // Basit TAR parser — .mmdb dosyasını bul
            var buffer = new byte[512];
            while (tarStream.Read(buffer, 0, 512) == 512)
            {
                // TAR header: dosya adı ilk 100 byte
                var fileName = System.Text.Encoding.ASCII.GetString(buffer, 0, 100).Trim('\0');
                if (string.IsNullOrEmpty(fileName)) break;

                // Dosya boyutu: byte 124-135 (octal)
                var sizeStr = System.Text.Encoding.ASCII.GetString(buffer, 124, 12).Trim('\0', ' ');
                var fileSize = Convert.ToInt64(sizeStr, 8);

                if (fileName.EndsWith(".mmdb"))
                {
                    var mmdbData = new byte[fileSize];
                    tarStream.Read(mmdbData, 0, (int)fileSize);
                    _logger.LogInformation("[UpdateGeoIpDatabaseJob] .mmdb bulundu: {Name} ({Size}KB)",
                        fileName, fileSize / 1024);
                    return mmdbData;
                }

                // Dosya verisini atla (512-byte bloklar halinde)
                var blocks = (fileSize + 511) / 512;
                tarStream.Seek(blocks * 512, SeekOrigin.Current);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[UpdateGeoIpDatabaseJob] tar.gz extract hatası");
        }

        return null;
    }

    private async Task NotifyIamApiToReload()
    {
        try
        {
            var iamApiUrl = _appSettings.Value.Urls?.IamApi;
            if (string.IsNullOrWhiteSpace(iamApiUrl))
            {
                _logger.LogDebug("[UpdateGeoIpDatabaseJob] IAM API URL tanımlı değil, reload sinyali atlanıyor");
                return;
            }

            // Basit bir health check — GeoIpService file watcher ile de yenilenebilir
            _logger.LogInformation("[UpdateGeoIpDatabaseJob] GeoIP database güncellendi. IAM API restart ile yeni versiyon yüklenir.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[UpdateGeoIpDatabaseJob] IAM API notify başarısız (kritik değil)");
        }
    }
}
