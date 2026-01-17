// backend/Jobs/BackgroundJobs/HangfireScheduler/Program.cs

using Hangfire;
using Hangfire.SqlServer;
using Jobs.BackgroundJobs.HangfireScheduler.Models;
using Jobs.BackgroundJobs.HangfireScheduler.Authorization;
using Jobs.BackgroundJobs.HangfireScheduler.Jobs.AnimalMarket;
using Jobs.BackgroundJobs.HangfireScheduler.Jobs.Hirovo;
using Microsoft.Extensions.Options;
using Common.Services.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog Logging
builder.AddSerilogLogging("HangfireScheduler");

// Controller ve configuration
builder.Services.AddControllers();
builder.Services.Configure<HangfireSettings>(builder.Configuration.GetSection("HangfireSettings"));
builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("ApplicationSettings"));
builder.Services.Configure<SystemSettings>(builder.Configuration.GetSection("SystemSettings"));

// HttpClient servisini ekle - NotifyEndingAuctionsJob için gerekli
builder.Services.AddHttpClient();

var hangfireConnectionString = builder.Configuration["ConnectionStrings:Hangfire"];
Log.Information("Hangfire connection: {Connection}", hangfireConnectionString?.Substring(0, Math.Min(50, hangfireConnectionString?.Length ?? 0)) + "...");

// Hangfire konfigürasyonu
builder.Services.AddHangfire(config =>
{
    var option = new SqlServerStorageOptions
    {
        PrepareSchemaIfNecessary = true,
        QueuePollInterval = TimeSpan.FromMinutes(5),
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    };

    config.UseSqlServerStorage(hangfireConnectionString, option)
          .WithJobExpirationTimeout(TimeSpan.FromDays(3));
});

builder.Services.AddHangfireServer(options =>
{
    options.SchedulePollingInterval = TimeSpan.FromSeconds(30);
    options.WorkerCount = Environment.ProcessorCount * 5;
});

var app = builder.Build();

app.UseDeveloperExceptionPage();

// Serilog Request Logging
app.UseSerilogRequestLogging();

app.MapControllers();

var hangfireOptions = app.Services.GetService<IOptions<HangfireSettings>>();

app.UseHangfireDashboard("", new DashboardOptions
{
    DashboardTitle = "AnimalMarket Hangfire Dashboard",
    AppPath = "/Home",
    Authorization = new[] { new HangfireDashboardAuthFilter(hangfireOptions) }
});

var recurringOptions = new RecurringJobOptions
{
    TimeZone = TimeZoneInfo.Local
};

// JOBS
RecurringJob.AddOrUpdate<CheckExpiredBidsJob>(
    nameof(CheckExpiredBidsJob),
    job => job.Process(),
    "*/5 * * * *",
    recurringOptions
);

RecurringJob.AddOrUpdate<ProcessVeterinaryNotificationsJob>(
    nameof(ProcessVeterinaryNotificationsJob),
    job => job.Process(),
    "*/5 * * * *",
    recurringOptions
);

// Süresi dolmuş ödemeleri kontrol et (Her 10 dakika)
RecurringJob.AddOrUpdate<CheckExpiredPaymentsJob>(
    nameof(CheckExpiredPaymentsJob),
    job => job.Process(),
    "*/10 * * * *",
    recurringOptions
);

// Yakında bitecek açık artırmalar için bildirim gönder (Her 5 dakika)
RecurringJob.AddOrUpdate<NotifyEndingAuctionsJob>(
    nameof(NotifyEndingAuctionsJob),
    job => job.Process(),
    "*/5 * * * *",
    recurringOptions
);

// Yaklaşan itiraz deadline'ları için bildirim gönder (Her 2 saatte bir)
RecurringJob.AddOrUpdate<NotifyApproachingDisputeDeadlinesJob>(
    nameof(NotifyApproachingDisputeDeadlinesJob),
    job => job.Process(),
    "0 */2 * * *",
    recurringOptions
);

// İtiraz süresi dolmuş escrow'ları otomatik release et (Her 30 dakika)
RecurringJob.AddOrUpdate<AutoReleaseEscrowAfterDisputePeriodJob>(
    nameof(AutoReleaseEscrowAfterDisputePeriodJob),
    job => job.Process(),
    "*/30 * * * *",
    recurringOptions
);

// HIROVO JOBS
// Skill seed ve çeviri - Manuel tetikleme için (Recurring değil, Dashboard'dan tetiklenir)
RecurringJob.AddOrUpdate<SeedAndTranslateSkillsJob>(
    nameof(SeedAndTranslateSkillsJob),
    job => job.Process(),
    Cron.Never,  // Manuel tetikleme - otomatik çalışmaz
    recurringOptions
);

Log.Information("Hangfire Scheduler started!");
Log.Information("Active Jobs:");
Log.Information("  - CheckExpiredBidsJob (Every 5 min)");
Log.Information("  - ProcessVeterinaryNotificationsJob (Every 5 min)");
Log.Information("  - CheckExpiredPaymentsJob (Every 10 min)");
Log.Information("  - NotifyEndingAuctionsJob (Every 5 min)");
Log.Information("  - NotifyApproachingDisputeDeadlinesJob (Every 2 hours)");
Log.Information("  - AutoReleaseEscrowAfterDisputePeriodJob (Every 30 min)");
Log.Information("  - SeedAndTranslateSkillsJob (Manual trigger)");

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Hangfire Scheduler terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
