using Common.Definitions.Infrastructure.RelationalDB;
using Common.Services.Environment;
using Common.Services.Logging;
using LivestockTrading.Infrastructure.RelationalDB;
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

            // Configure DbContext for push token and notification access
            var connectionString = hostContext.Configuration["ProjectConfigurations:RelationalDbConfiguration:ConnectionString"]
                ?? hostContext.Configuration.GetConnectionString("SqlConnectionString");

            var environmentName = hostContext.Configuration["ProjectConfigurations:EnvironmentConfiguration:EnvironmentName"]
                ?? hostContext.Configuration["ASPNETCORE_ENVIRONMENT"]
                ?? "Development";

            var relationalDbConfig = new RelationalDbConfiguration { ConnectionString = connectionString };
            var environmentConfig = new EnvironmentConfiguration { EnvironmentName = environmentName };
            var environmentService = new EnvironmentService(environmentConfig);

            var definitionDbContextOptions = new DefinitionDbContextOptions(relationalDbConfig, environmentService);
            var livestockTradingDbContextOptions = new LivestockTradingDbContextOptions(relationalDbConfig, environmentService, definitionDbContextOptions);

            services.AddSingleton(relationalDbConfig);
            services.AddSingleton(environmentService);
            services.AddSingleton(definitionDbContextOptions);
            services.AddSingleton(livestockTradingDbContextOptions);
            services.AddScoped<LivestockTradingModuleDbContext>(sp =>
            {
                var opts = sp.GetRequiredService<LivestockTradingDbContextOptions>();
                return new LivestockTradingModuleDbContext(opts);
            });

            // Configure push notification service as singleton
            services.AddSingleton<IPushNotificationService>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var logger = sp.GetRequiredService<ILogger<FirebasePushNotificationService>>();
                return new FirebasePushNotificationService(config, logger);
            });

            // Register push token repository
            services.AddScoped<PushTokenRepository>();

            // Register all event handlers as scoped
            // Messaging event handlers
            services.AddScoped<MessageCreatedNotificationHandler>();
            services.AddScoped<MessageReadNotificationHandler>();
            services.AddScoped<ConversationCreatedNotificationHandler>();

            // Product event handlers
            services.AddScoped<ProductCreatedNotificationHandler>();
            services.AddScoped<ProductApprovedNotificationHandler>();

            // Seller/Transporter event handlers
            services.AddScoped<SellerCreatedNotificationHandler>();
            services.AddScoped<TransporterCreatedNotificationHandler>();

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
