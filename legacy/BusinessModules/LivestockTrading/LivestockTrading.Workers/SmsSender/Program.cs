using Common.Services.Logging;
using LivestockTrading.Workers.SmsSender.EventHandlers;
using LivestockTrading.Workers.SmsSender.Services;
using LivestockTrading.Workers.SmsSender.Workers;
using Serilog;

// Early Serilog initialization
Log.Logger = SerilogConfiguration
    .CreateConfiguration("LivestockTrading.SmsSender", "Production")
    .CreateLogger();

try
{
    Log.Information("Starting LivestockTrading SmsSender Worker...");

    var host = Host.CreateDefaultBuilder(args)
        .AddSerilogLogging("LivestockTrading.SmsSender")
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true);
            config.AddEnvironmentVariables();
        })
        .ConfigureServices((hostContext, services) =>
        {
            // Configure SMS service as singleton
            services.AddSingleton<ISmsService>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var logger = sp.GetRequiredService<ILogger<TwilioSmsService>>();
                return new TwilioSmsService(config, logger);
            });

            // Register all event handlers as scoped
            services.AddScoped<StudentCreatedSmsHandler>();
            services.AddScoped<StudentUpdatedSmsHandler>();
            services.AddScoped<StudentDeletedSmsHandler>();

            // Register hosted service (background worker)
            services.AddHostedService<SmsWorker>();
        })
        .Build();

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "LivestockTrading SmsSender Worker terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
