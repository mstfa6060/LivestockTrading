using Microsoft.Extensions.Options;
using Jobs.BackgroundJobs.HangfireScheduler.Models;
using System.Text;
using System.Text.Json;

namespace Jobs.BackgroundJobs.HangfireScheduler.Jobs.AnimalMarket;

/// <summary>
/// İtiraz süresi dolmuş escrow'ları otomatik release eden Hangfire job.
/// Her 30 dakikada bir çalışır.
/// </summary>
public class AutoReleaseEscrowAfterDisputePeriodJob
{
    private readonly IOptions<ApplicationSettings> _appSettings;
    private readonly IOptions<HangfireSettings> _hangfireSettings;
    private readonly ILogger<AutoReleaseEscrowAfterDisputePeriodJob> _logger;
    private readonly HttpClient _httpClient;

    public AutoReleaseEscrowAfterDisputePeriodJob(
        IOptions<ApplicationSettings> appSettings,
        IOptions<HangfireSettings> hangfireSettings,
        ILogger<AutoReleaseEscrowAfterDisputePeriodJob> logger,
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
            _logger.LogInformation("[AutoReleaseEscrowAfterDisputePeriodJob] Is basladi - {Time}", DateTime.UtcNow);

            var baseUrl = _appSettings.Value.Urls.AnimalMarketApi;
            var endpoint = _appSettings.Value.Urls.AnimalMarket.AutoReleaseEscrowAfterDisputePeriod;
            var url = $"{baseUrl}{endpoint}";

            _logger.LogInformation("[AutoReleaseEscrowAfterDisputePeriodJob] URL: {Url}", url);

            var request = new HttpRequestMessage(HttpMethod.Post, url);

            // SystemSecret'ı önce settings'ten, yoksa environment'tan al
            var systemSecret = _hangfireSettings.Value.SystemSecret
                ?? Environment.GetEnvironmentVariable("SystemSettings__HangfireSystemSecret")
                ?? "HnGf1r3-Sy5t3m-S3cr3t-D3v-2024-V3ry-L0ng-R4nd0m-K3y-Ch4ng3-1n-Pr0d";

            if (string.IsNullOrEmpty(systemSecret))
            {
                _logger.LogError("[AutoReleaseEscrowAfterDisputePeriodJob] SystemSecret bulunamadi!");
                return;
            }

            request.Headers.Add("X-System-Secret", systemSecret);

            // Boş body gönder
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
                _logger.LogInformation("[AutoReleaseEscrowAfterDisputePeriodJob] Basarili - Response: {Response}", responseBody);
            }
            else
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    "[AutoReleaseEscrowAfterDisputePeriodJob] Basarisiz - StatusCode: {StatusCode}, Error: {Error}",
                    response.StatusCode,
                    errorBody);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[AutoReleaseEscrowAfterDisputePeriodJob] Exception olustu");
        }
    }
}