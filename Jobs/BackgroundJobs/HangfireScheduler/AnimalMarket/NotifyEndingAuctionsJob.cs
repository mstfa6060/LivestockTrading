// backend/Jobs/BackgroundJobs/HangfireScheduler/Jobs/AnimalMarket/NotifyEndingAuctionsJob.cs

using Microsoft.Extensions.Options;
using Jobs.BackgroundJobs.HangfireScheduler.Models;
using System.Text;
using System.Text.Json;

namespace Jobs.BackgroundJobs.HangfireScheduler.Jobs.AnimalMarket;

/// <summary>
/// Yakında bitecek açık artırmalar için bildirim gönderen HangFire job
/// Her 5 dakikada bir çalışır ve 1 saat içinde bitecek açık artırmaları bildirir
/// </summary>
public class NotifyEndingAuctionsJob
{
    private readonly IOptions<ApplicationSettings> _appSettings;
    private readonly IOptions<HangfireSettings> _hangfireSettings;
    private readonly ILogger<NotifyEndingAuctionsJob> _logger;
    private readonly HttpClient _httpClient;

    public NotifyEndingAuctionsJob(
        IOptions<ApplicationSettings> appSettings,
        IOptions<HangfireSettings> hangfireSettings,
        ILogger<NotifyEndingAuctionsJob> logger,
        HttpClient httpClient)
    {
        _appSettings = appSettings;
        _hangfireSettings = hangfireSettings;
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task Process()
    {
        try
        {
            _logger.LogInformation("⏰ [NotifyEndingAuctionsJob] İş başladı - {Time}", DateTime.UtcNow);

            var baseUrl = _appSettings.Value.Urls.AnimalMarketApi;
            var endpoint = "/AnimalBids/NotifyEndingAuctions";
            var url = $"{baseUrl}{endpoint}";

            _logger.LogInformation("📡 [NotifyEndingAuctionsJob] URL: {Url}", url);

            var request = new HttpRequestMessage(HttpMethod.Post, url);

            //  DÜZELTME: SystemSecret'ı önce settings'ten, yoksa environment'tan al
            var systemSecret = _hangfireSettings.Value.SystemSecret
                ?? Environment.GetEnvironmentVariable("SystemSettings__HangfireSystemSecret")
                ?? "HnGf1r3-Sy5t3m-S3cr3t-D3v-2024-V3ry-L0ng-R4nd0m-K3y-Ch4ng3-1n-Pr0d";

            if (string.IsNullOrEmpty(systemSecret))
            {
                _logger.LogError("❌ [NotifyEndingAuctionsJob] SystemSecret bulunamadı!");
                return;
            }

            request.Headers.Add("X-System-Secret", systemSecret);

            // Boş body gönder (Request model boş)
            var emptyBody = new { };
            request.Content = new StringContent(
                JsonSerializer.Serialize(emptyBody),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogInformation(" [NotifyEndingAuctionsJob] Başarılı - Response: {Response}", responseBody);
            }
            else
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    "❌ [NotifyEndingAuctionsJob] Başarısız - StatusCode: {StatusCode}, Error: {Error}",
                    response.StatusCode,
                    errorBody);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [NotifyEndingAuctionsJob] Exception oluştu");
        }
    }
}