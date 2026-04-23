// backend/Jobs/BackgroundJobs/HangfireScheduler/Models/SystemSettings.cs

namespace Jobs.BackgroundJobs.HangfireScheduler.Models;


/// <summary>
/// Hangfire'dan API'ye yapılan çağrılar için sistem secret
/// Bu secret hem Hangfire hem de API tarafında aynı olmalı
/// </summary>
public class SystemSettings
{
    public string HangfireSystemSecret { get; set; }
}