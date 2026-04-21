using Common.Services.Logging;
using LivestockTrading.Workers.MailSender.EventHandlers;
using LivestockTrading.Workers.MailSender.Services;
using LivestockTrading.Workers.MailSender.Workers;
using Serilog;

// Early Serilog initialization
Log.Logger = SerilogConfiguration
    .CreateConfiguration("LivestockTrading.MailSender", "Production")
    .CreateLogger();

try
{
    Log.Information("Starting LivestockTrading MailSender Worker...");

    var host = Host.CreateDefaultBuilder(args)
        .AddSerilogLogging("LivestockTrading.MailSender")
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true);
            config.AddEnvironmentVariables();
        })
        .ConfigureServices((hostContext, services) =>
        {
            // Configure email service as singleton
            services.AddSingleton<IEmailService>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var logger = sp.GetRequiredService<ILogger<SendGridEmailService>>();
                return new SendGridEmailService(
                    config["SendGrid:ApiKey"] ?? "",
                    config["SendGrid:FromEmail"] ?? "noreply@globallivestock.com",
                    config["SendGrid:FromName"] ?? "GlobalLivestock",
                    logger
                );
            });

            // Register all event handlers as scoped
            services.AddScoped<StudentCreatedEmailHandler>();
            services.AddScoped<StudentUpdatedEmailHandler>();
            services.AddScoped<StudentDeletedEmailHandler>();

            // Register hosted service (background worker)
            services.AddHostedService<EmailWorker>();
        })
        .Build();

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "LivestockTrading MailSender Worker terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
