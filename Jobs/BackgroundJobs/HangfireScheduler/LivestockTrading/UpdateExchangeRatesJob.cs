using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Jobs.BackgroundJobs.HangfireScheduler.Models;

namespace Jobs.BackgroundJobs.HangfireScheduler.Jobs.LivestockTrading;

/// <summary>
/// Döviz kurlarını harici API'den çekip LivestockTrading API üzerinden güncelleyen Hangfire job.
/// Her 6 saatte bir çalışır.
/// Akış: open.er-api.com → Hangfire → LivestockTrading API (Currencies/UpdateRates) → DB
/// </summary>
public class UpdateExchangeRatesJob
{
    private readonly IOptions<ApplicationSettings> _appSettings;
    private readonly IOptions<HangfireSettings> _hangfireSettings;
    private readonly ILogger<UpdateExchangeRatesJob> _logger;
    private readonly HttpClient _httpClient;

    private const string ExchangeRateApiUrl = "https://open.er-api.com/v6/latest/USD";

    public UpdateExchangeRatesJob(
        IOptions<ApplicationSettings> appSettings,
        IOptions<HangfireSettings> hangfireSettings,
        ILogger<UpdateExchangeRatesJob> logger,
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
            _logger.LogInformation("[UpdateExchangeRatesJob] İş başladı - {Time}", DateTime.UtcNow);

            // 1. Döviz kurlarını harici API'den çek
            var rates = await FetchExchangeRates();
            if (rates == null || rates.Count == 0)
            {
                _logger.LogWarning("[UpdateExchangeRatesJob] API'den kur bilgisi alınamadı");
                return;
            }

            _logger.LogInformation("[UpdateExchangeRatesJob] {Count} adet kur bilgisi alındı", rates.Count);

            // 2. LivestockTrading API'ye gönder (Currencies/UpdateRates endpoint'i)
            var updatedCount = await SendRatesToApi(rates);
            _logger.LogInformation("[UpdateExchangeRatesJob] {UpdatedCount} adet para birimi güncellendi", updatedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[UpdateExchangeRatesJob] Exception oluştu");
        }
    }

    private async Task<Dictionary<string, decimal>> FetchExchangeRates()
    {
        try
        {
            var response = await _httpClient.GetAsync(ExchangeRateApiUrl);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ExchangeRateApiResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (apiResponse?.Result != "success" || apiResponse.Rates == null)
            {
                _logger.LogWarning("[UpdateExchangeRatesJob] API yanıtı başarısız: {Result}", apiResponse?.Result);
                return null;
            }

            _logger.LogInformation("[UpdateExchangeRatesJob] API base: {Base}, güncelleme: {Time}",
                apiResponse.BaseCode, apiResponse.TimeLastUpdateUtc);

            return apiResponse.Rates;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[UpdateExchangeRatesJob] Harici API çağrısı başarısız");
            return null;
        }
    }

    private async Task<int> SendRatesToApi(Dictionary<string, decimal> rates)
    {
        var baseUrl = _appSettings.Value.Urls.LivestockTradingApi;
        var url = $"{baseUrl}/Currencies/UpdateRates";

        _logger.LogInformation("[UpdateExchangeRatesJob] URL: {Url}", url);

        // Request body oluştur
        var rateItems = rates.Select(r => new { code = r.Key, exchangeRateToUSD = r.Value }).ToList();
        var requestBody = new { rates = rateItems };

        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Content = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json"
        );

        // SystemSecret header ekle
        var systemSecret = !string.IsNullOrEmpty(_hangfireSettings.Value.SystemSecret)
            ? _hangfireSettings.Value.SystemSecret
            : Environment.GetEnvironmentVariable("SystemSettings__HangfireSystemSecret")
              ?? Environment.GetEnvironmentVariable("HangfireSettings__SystemSecret")
              ?? "";

        if (!string.IsNullOrEmpty(systemSecret))
            request.Headers.Add("X-System-Secret", systemSecret);

        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("[UpdateExchangeRatesJob] API yanıtı başarılı: {Response}", responseBody);

            // Response'dan updatedCount'u parse et
            try
            {
                var result = JsonSerializer.Deserialize<ApiResponse>(responseBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return result?.Payload?.UpdatedCount ?? 0;
            }
            catch
            {
                return 0;
            }
        }
        else
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            _logger.LogError(
                "[UpdateExchangeRatesJob] API çağrısı başarısız - StatusCode: {StatusCode}, Error: {Error}",
                response.StatusCode, errorBody);
            return 0;
        }
    }

    private class ExchangeRateApiResponse
    {
        public string Result { get; set; }
        public string BaseCode { get; set; }
        public string TimeLastUpdateUtc { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
    }

    private class ApiResponse
    {
        public ApiPayload Payload { get; set; }
    }

    private class ApiPayload
    {
        public int UpdatedCount { get; set; }
    }
}
