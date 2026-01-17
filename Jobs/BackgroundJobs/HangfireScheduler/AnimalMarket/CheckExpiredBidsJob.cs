// backend/Jobs/BackgroundJobs/HangfireScheduler/Jobs/AnimalMarket/CheckExpiredBidsJob.cs

using System.Net;
using Microsoft.Extensions.Options;
using Jobs.BackgroundJobs.HangfireScheduler.Models;

namespace Jobs.BackgroundJobs.HangfireScheduler.Jobs.AnimalMarket;

public class CheckExpiredBidsJob
{
    private readonly IOptions<ApplicationSettings> _appSettings;
    private readonly IOptions<SystemSettings> _systemSettings;

    public CheckExpiredBidsJob(
        IOptions<ApplicationSettings> appSettings,
        IOptions<SystemSettings> systemSettings)
    {
        _appSettings = appSettings;
        _systemSettings = systemSettings;
    }

    public async Task Process()
    {
        await CheckExpiredBids();
    }

    private async Task CheckExpiredBids()
    {
        using var client = new HttpClient();

        //  System secret header ekle - GÜVENLİK
        client.DefaultRequestHeaders.Add(
            "X-System-Secret",
            _systemSettings.Value.HangfireSystemSecret
        );

        var requestUrl = $"{_appSettings.Value.Urls.AnimalMarketApi}{_appSettings.Value.Urls.AnimalMarket.CheckExpiredBids}";

        Console.WriteLine($"⏰ CheckExpiredBidsJob başlatıldı: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"Request URL: {requestUrl}");

        var result = await client.PostAsync(requestUrl, null);

        if (result.StatusCode != HttpStatusCode.OK)
        {
            Console.WriteLine($"❌ CheckExpiredBidsJob başarısız: {result.StatusCode}");
            var errorContent = await result.Content.ReadAsStringAsync();
            Console.WriteLine($"Error Content: {errorContent}");
            throw new Exception($"CheckExpiredBidsJob failed. Status: {result.StatusCode}");
        }

        var responseContent = await result.Content.ReadAsStringAsync();
        Console.WriteLine($" CheckExpiredBidsJob tamamlandı. Response: {responseContent}");
    }
}