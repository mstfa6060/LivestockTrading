using Common.Services.Logging;
using Common.Definitions.Infrastructure.RelationalDB;
using LivestockTrading.Infrastructure.RelationalDB;
using LivestockTrading.Workers.NotificationSender.EventHandlers;
using LivestockTrading.Workers.NotificationSender.Services;
using LivestockTrading.Workers.NotificationSender.Workers;
using Microsoft.EntityFrameworkCore;
using Serilog;

// Early Serilog initialization
Log.Logger = SerilogConfiguration
    .CreateConfiguration("LivestockTrading.NotificationSender", "Production")
    .CreateLogger();

try
{
    Log.Information("Starting LivestockTrading NotificationSender Worker...");

    var host = Host.CreateDefaultBuilder(args)
        .AddSerilogLogging("LivestockTrading.NotificationSender")
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true);
            config.AddEnvironmentVariables();
        })
        .ConfigureServices((hostContext, services) =>
        {
            // Configure HTTP client factory
            services.AddHttpClient();

            // Configure LivestockTradingModuleDbContext for ExchangeRateUpdater
            var connectionString = hostContext.Configuration["ProjectConfigurations:RelationalDbConfiguration:ConnectionString"]
                ?? hostContext.Configuration["ConnectionStrings:DefaultConnection"]
                ?? Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING");

            if (!string.IsNullOrEmpty(connectionString))
            {
                services.AddScoped<LivestockTradingModuleDbContext>(sp =>
                {
                    var options = new DbContextOptionsBuilder<DefinitionDbContext>()
                        .UseSqlServer(connectionString, sql =>
                        {
                            sql.EnableRetryOnFailure(
                                maxRetryCount: 5,
                                maxRetryDelay: TimeSpan.FromSeconds(3),
                                errorNumbersToAdd: null
                            );
                        })
                        .Options;

                    return new LivestockTradingModuleDbContext(options);
                });

                // Register ExchangeRateUpdater scheduled background service
                services.AddHostedService<ExchangeRateUpdaterService>();

                Log.Information("ExchangeRateUpdaterService registered with database connection");
            }
            else
            {
                Log.Warning("No database connection string found. ExchangeRateUpdaterService will not be registered");
            }

            // Configure push notification service as singleton
            services.AddSingleton<IPushNotificationService>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var logger = sp.GetRequiredService<ILogger<FirebasePushNotificationService>>();
                return new FirebasePushNotificationService(config, logger);
            });

            // Register all event handlers as scoped
            // Messaging event handlers
            services.AddScoped<MessageCreatedNotificationHandler>();
            services.AddScoped<MessageReadNotificationHandler>();
            services.AddScoped<ConversationCreatedNotificationHandler>();

            // Student event handlers (legacy)
            services.AddScoped<StudentCreatedNotificationHandler>();
            services.AddScoped<StudentUpdatedNotificationHandler>();
            services.AddScoped<StudentDeletedNotificationHandler>();

            // Register hosted service (background worker)
            services.AddHostedService<NotificationWorker>();
        })
        .Build();

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "LivestockTrading NotificationSender Worker terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
