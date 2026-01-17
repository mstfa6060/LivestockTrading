// backend/Jobs/BackgroundJobs/HangfireScheduler/Jobs/AnimalMarket/CheckExpiredPaymentsJob.cs

using Jobs.BackgroundJobs.HangfireScheduler.Models;
using Microsoft.Extensions.Options;
using System.Net;

namespace Jobs.BackgroundJobs.HangfireScheduler.Jobs.AnimalMarket;

public class CheckExpiredPaymentsJob
{
    private readonly IOptions<ApplicationSettings> _appSettings;
    private readonly IOptions<SystemSettings> _systemSettings;

    public CheckExpiredPaymentsJob(
        IOptions<ApplicationSettings> appSettings,
        IOptions<SystemSettings> systemSettings)
    {
        _appSettings = appSettings;
        _systemSettings = systemSettings;
    }

    public async Task Process()
    {
        try
        {
            await CheckExpiredPayments();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ CheckExpiredPaymentsJob Exception: {ex.Message}");
            throw;
        }
    }

    private async Task CheckExpiredPayments()
    {
        using var client = new HttpClient();

        //  System secret header ekle
        client.DefaultRequestHeaders.Add(
            "X-System-Secret",
            _systemSettings.Value.HangfireSystemSecret
        );

        var requestUrl = $"{_appSettings.Value.Urls.AnimalMarketApi}{_appSettings.Value.Urls.AnimalMarket.CheckExpiredPayments}";

        Console.WriteLine($"⏰ [CheckExpiredPaymentsJob] Başlatıldı: {DateTime.UtcNow}");
        Console.WriteLine($"🔗 Request URL: {requestUrl}");

        var result = await client.PostAsync(requestUrl, null);

        if (result.StatusCode != HttpStatusCode.OK)
        {
            var errorContent = await result.Content.ReadAsStringAsync();
            Console.WriteLine($"❌ [CheckExpiredPaymentsJob] Başarısız! Status: {result.StatusCode}");
            Console.WriteLine($"❌ Error: {errorContent}");
            throw new Exception($"CheckExpiredPaymentsJob failed with status: {result.StatusCode}");
        }

        var responseContent = await result.Content.ReadAsStringAsync();
        Console.WriteLine($" [CheckExpiredPaymentsJob] Tamamlandı. Response: {responseContent}");
    }
}