using Common.Services.Logging;
using LivestockTrading.Workers.SocialMediaPoster.EventHandlers;
using LivestockTrading.Workers.SocialMediaPoster.Services;
using LivestockTrading.Workers.SocialMediaPoster.Workers;
using Serilog;

// Early Serilog initialization
Log.Logger = SerilogConfiguration
    .CreateConfiguration("LivestockTrading.SocialMediaPoster", "Production")
    .CreateLogger();

try
{
    Log.Information("Starting LivestockTrading SocialMediaPoster Worker...");

    var host = Host.CreateDefaultBuilder(args)
        .AddSerilogLogging("LivestockTrading.SocialMediaPoster")
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true);
            config.AddEnvironmentVariables();
        })
        .ConfigureServices((hostContext, services) =>
        {
            // Configure HTTP client for Instagram API
            services.AddHttpClient("Instagram", client =>
            {
                client.BaseAddress = new Uri(
                    hostContext.Configuration["Instagram:BaseUrl"] ?? "https://graph.instagram.com/v21.0");
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            // Configure HTTP client for FileProvider API (cover image resolution)
            services.AddHttpClient("FileProvider", client =>
            {
                client.BaseAddress = new Uri(
                    hostContext.Configuration["FileProvider:BaseUrl"] ?? "http://fileprovider-api-container:8080");
                client.Timeout = TimeSpan.FromSeconds(10);
            });

            // Instagram service as singleton
            services.AddSingleton<IInstagramService, InstagramService>();

            // Event handler as scoped (per message)
            services.AddScoped<ProductApprovedSocialMediaHandler>();

            // Background worker
            services.AddHostedService<SocialMediaWorker>();
        })
        .Build();

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "LivestockTrading SocialMediaPoster Worker terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
