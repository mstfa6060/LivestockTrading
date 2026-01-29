using Common.Services.Logging;
using LivestockTrading.Workers.NotificationSender.EventHandlers;
using LivestockTrading.Workers.NotificationSender.Services;
using LivestockTrading.Workers.NotificationSender.Workers;
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
