using Microsoft.Extensions.Options;
using Jobs.BackgroundJobs.HangfireScheduler.Models;
using System.Text;
using System.Text.Json;

namespace Jobs.BackgroundJobs.HangfireScheduler.Jobs.LivestockTrading;

/// <summary>
/// Öğrenci aktivitelerini loglayan Hangfire job
/// Günde 2 kere çalışır (09:00 ve 18:00)
/// </summary>
public class LogStudentActivityJob
{
    private readonly IOptions<ApplicationSettings> _appSettings;
    private readonly IOptions<HangfireSettings> _hangfireSettings;
    private readonly ILogger<LogStudentActivityJob> _logger;
    private readonly HttpClient _httpClient;

    public LogStudentActivityJob(
        IOptions<ApplicationSettings> appSettings,
        IOptions<HangfireSettings> hangfireSettings,
        ILogger<LogStudentActivityJob> logger,
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
            _logger.LogInformation("[LogStudentActivityJob] İş başladı - {Time}", DateTime.UtcNow);

            var baseUrl = _appSettings.Value.Urls.LivestockTradingApi;
            var endpoint = "/Students/LogStudentActivity";
            var url = $"{baseUrl}{endpoint}";

            _logger.LogInformation("[LogStudentActivityJob] URL: {Url}", url);

            var request = new HttpRequestMessage(HttpMethod.Post, url);

            // SystemSecret header ekle
            var systemSecret = !string.IsNullOrEmpty(_hangfireSettings.Value.SystemSecret)
                ? _hangfireSettings.Value.SystemSecret
                : !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SystemSettings__HangfireSystemSecret"))
                    ? Environment.GetEnvironmentVariable("SystemSettings__HangfireSystemSecret")
                    : !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("HangfireSettings__SystemSecret"))
                        ? Environment.GetEnvironmentVariable("HangfireSettings__SystemSecret")
                        : "HnGf1r3-Sy5t3m-S3cr3t-D3v-2024-V3ry-L0ng-R4nd0m-K3y-Ch4ng3-1n-Pr0d";

            request.Headers.Add("X-System-Secret", systemSecret);

            // Request body - son 1 günün aktivitelerini logla
            var requestBody = new { DaysToKeep = 1 };
            request.Content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("[LogStudentActivityJob] Başarılı - Response: {Response}", responseBody);
            }
            else
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    "[LogStudentActivityJob] Başarısız - StatusCode: {StatusCode}, Error: {Error}",
                    response.StatusCode,
                    errorBody);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[LogStudentActivityJob] Exception oluştu");
        }
    }
}
