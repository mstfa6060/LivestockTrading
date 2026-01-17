using System.Net;
using Microsoft.Extensions.Options;
using Jobs.BackgroundJobs.HangfireScheduler.Models;

namespace Jobs.BackgroundJobs.HangfireScheduler.Jobs.Hirovo;

public class SeedAndTranslateSkillsJob
{
    private readonly IOptions<ApplicationSettings> _appSettings;
    private readonly IOptions<SystemSettings> _systemSettings;

    public SeedAndTranslateSkillsJob(
        IOptions<ApplicationSettings> appSettings,
        IOptions<SystemSettings> systemSettings)
    {
        _appSettings = appSettings;
        _systemSettings = systemSettings;
    }

    public async Task Process()
    {
        using var client = new HttpClient();
        client.Timeout = TimeSpan.FromMinutes(30); // Uzun sürebilir

        // System secret header ekle
        client.DefaultRequestHeaders.Add(
            "X-System-Secret",
            _systemSettings.Value.HangfireSystemSecret
        );

        var requestUrl = $"{_appSettings.Value.Urls.HirovoApi}{_appSettings.Value.Urls.Hirovo.SeedAndTranslateSkills}";

        Console.WriteLine($"SeedAndTranslateSkillsJob baslatildi: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"Request URL: {requestUrl}");

        var result = await client.PostAsync(requestUrl, new StringContent("{}", System.Text.Encoding.UTF8, "application/json"));

        if (result.StatusCode != HttpStatusCode.OK)
        {
            Console.WriteLine($"SeedAndTranslateSkillsJob basarisiz: {result.StatusCode}");
            var errorContent = await result.Content.ReadAsStringAsync();
            Console.WriteLine($"Error Content: {errorContent}");
            throw new Exception($"SeedAndTranslateSkillsJob failed. Status: {result.StatusCode}");
        }

        var responseContent = await result.Content.ReadAsStringAsync();
        Console.WriteLine($"SeedAndTranslateSkillsJob tamamlandi. Response: {responseContent}");
    }
}
