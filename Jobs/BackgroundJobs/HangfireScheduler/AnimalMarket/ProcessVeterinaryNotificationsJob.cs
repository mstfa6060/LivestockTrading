// Jobs/BackgroundJobs/HangfireScheduler/Jobs/AnimalMarket/ProcessVeterinaryNotificationsJob.cs

using System.Net;
using Microsoft.Extensions.Options;
using Jobs.BackgroundJobs.HangfireScheduler.Models;

namespace Jobs.BackgroundJobs.HangfireScheduler.Jobs.AnimalMarket;

public class ProcessVeterinaryNotificationsJob
{
    private readonly IOptions<ApplicationSettings> _settings;

    public ProcessVeterinaryNotificationsJob(IOptions<ApplicationSettings> settings)
    {
        _settings = settings;
    }

    public async Task Process()
    {
        await SendVeterinaryNotifications();
    }

    private async Task SendVeterinaryNotifications()
    {
        using var client = new HttpClient();

        var requestUrl = $"{_settings.Value.Urls.AnimalMarketApi}{_settings.Value.Urls.AnimalMarket.SendVeterinaryNotifications}";

        Console.WriteLine($"🔔 ProcessVeterinaryNotificationsJob başlatıldı: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"Request URL: {requestUrl}");

        var result = await client.PostAsync(requestUrl, null);

        if (result.StatusCode != HttpStatusCode.OK)
        {
            Console.WriteLine($"❌ ProcessVeterinaryNotificationsJob başarısız: {result.StatusCode}");
            throw new Exception($"ProcessVeterinaryNotificationsJob failed. {DateTime.Now}");
        }

        Console.WriteLine(" ProcessVeterinaryNotificationsJob tamamlandı");
    }
}