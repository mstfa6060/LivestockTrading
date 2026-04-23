// backend/Jobs/BackgroundJobs/HangfireScheduler/Program.cs

using Hangfire;
using Hangfire.SqlServer;
using Jobs.BackgroundJobs.HangfireScheduler.Models;
using Jobs.BackgroundJobs.HangfireScheduler.Authorization;
using Jobs.BackgroundJobs.HangfireScheduler.Jobs.LivestockTrading;
using Jobs.BackgroundJobs.HangfireScheduler.Jobs.IAM;
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

// HttpClient servisini ekle
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
    DashboardTitle = "LivestockTrading Hangfire Dashboard",
    AppPath = "/Home",
    Authorization = new[] { new HangfireDashboardAuthFilter(hangfireOptions) }
});

var recurringOptions = new RecurringJobOptions
{
    TimeZone = TimeZoneInfo.Local
};

// LIVESTOCKTRADING JOBS

// Öğrenci aktivitelerini logla - Günde 2 kere (09:00 ve 18:00)
RecurringJob.AddOrUpdate<LogStudentActivityJob>(
    nameof(LogStudentActivityJob),
    job => job.Process(),
    "0 9,18 * * *",  // Her gün saat 09:00 ve 18:00'de
    recurringOptions
);

// Döviz kurlarını güncelle - Her 6 saatte bir (00:00, 06:00, 12:00, 18:00)
RecurringJob.AddOrUpdate<UpdateExchangeRatesJob>(
    nameof(UpdateExchangeRatesJob),
    job => job.Process(),
    "0 */6 * * *",  // Her 6 saatte bir
    recurringOptions
);

// Süresi dolmuş boost'ları temizle - Her saat başı
RecurringJob.AddOrUpdate<ExpireProductBoostsJob>(
    nameof(ExpireProductBoostsJob),
    job => job.Process(),
    "0 * * * *",  // Her saat başı
    recurringOptions
);

// IAM JOBS

// MaxMind GeoLite2 veritabanını güncelle - Haftada 1 (Pazartesi 03:00)
RecurringJob.AddOrUpdate<UpdateGeoIpDatabaseJob>(
    nameof(UpdateGeoIpDatabaseJob),
    job => job.Process(),
    "0 3 * * 1",  // Her Pazartesi saat 03:00
    recurringOptions
);

Log.Information("Hangfire Scheduler started!");
Log.Information("Active Jobs:");
Log.Information("  - LogStudentActivityJob (Daily at 09:00 and 18:00)");
Log.Information("  - UpdateExchangeRatesJob (Every 6 hours)");
Log.Information("  - ExpireProductBoostsJob (Every hour)");
Log.Information("  - UpdateGeoIpDatabaseJob (Weekly, Monday 03:00)");

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
