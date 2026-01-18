// backend/Jobs/BackgroundJobs/HangfireScheduler/Program.cs

using Hangfire;
using Hangfire.SqlServer;
using Jobs.BackgroundJobs.HangfireScheduler.Models;
using Jobs.BackgroundJobs.HangfireScheduler.Authorization;
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

// TODO: Add LivestockTrading background jobs here
// Example:
// RecurringJob.AddOrUpdate<YourJob>(
//     nameof(YourJob),
//     job => job.Process(),
//     "*/5 * * * *",  // Every 5 minutes
//     recurringOptions
// );

Log.Information("Hangfire Scheduler started!");
Log.Information("No recurring jobs configured yet. Add jobs as needed.");

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
