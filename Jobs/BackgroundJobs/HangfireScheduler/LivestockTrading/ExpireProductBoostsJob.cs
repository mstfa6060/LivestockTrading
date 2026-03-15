using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Jobs.BackgroundJobs.HangfireScheduler.Models;

namespace Jobs.BackgroundJobs.HangfireScheduler.Jobs.LivestockTrading;

/// <summary>
/// Süresi dolmuş ProductBoost kayıtlarını devre dışı bırakır ve
/// Product.IsFeatured / BoostScore alanlarını sıfırlar.
/// Her saat başı çalışır.
/// Akış: Hangfire → LivestockTrading API (ProductBoosts/Expire) → DB
/// </summary>
public class ExpireProductBoostsJob
{
    private readonly IOptions<ApplicationSettings> _appSettings;
    private readonly ILogger<ExpireProductBoostsJob> _logger;
    private readonly HttpClient _httpClient;

    public ExpireProductBoostsJob(
        IOptions<ApplicationSettings> appSettings,
        ILogger<ExpireProductBoostsJob> logger,
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
            _logger.LogInformation("[ExpireProductBoostsJob] İş başladı - {Time}", DateTime.UtcNow);

            var baseUrl = _appSettings.Value.Urls.LivestockTradingApi;
            var url = $"{baseUrl}/ProductBoosts/Expire";

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new StringContent("{}", Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse>(body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                _logger.LogInformation("[ExpireProductBoostsJob] {Count} adet boost süresi doldu", result?.Payload?.ExpiredCount ?? 0);
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("[ExpireProductBoostsJob] API hatası - {StatusCode}: {Error}", response.StatusCode, error);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ExpireProductBoostsJob] Exception oluştu");
        }
    }

    private class ApiResponse
    {
        public ApiPayload Payload { get; set; }
    }

    private class ApiPayload
    {
        public int ExpiredCount { get; set; }
    }
}
