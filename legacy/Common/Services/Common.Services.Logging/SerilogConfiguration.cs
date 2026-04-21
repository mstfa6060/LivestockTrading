using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace Common.Services.Logging;

public static class SerilogConfiguration
{
    /// <summary>
    /// Serilog yapılandırmasını oluşturur
    /// </summary>
    /// <param name="serviceName">Servis adı (örn: LivestockTrading.Api)</param>
    /// <param name="environment">Ortam adı (Development, Production)</param>
    /// <param name="logPath">Log dosya yolu (varsayılan: /app/logs/{serviceName})</param>
    public static LoggerConfiguration CreateConfiguration(
        string serviceName,
        string environment,
        string? logPath = null)
    {
        string? basePath;

        // Log path belirleme
        if (!string.IsNullOrEmpty(logPath))
        {
            basePath = logPath;
        }
        else if (OperatingSystem.IsWindows())
        {
            // Windows development
            basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Logs", serviceName);
        }
        else
        {
            // Linux/Docker - /app/logs kullan (container içinde yazılabilir)
            basePath = Path.Combine("/app", "logs", serviceName);
        }

        // Klasör yoksa oluştur - hata durumunda sessizce geç
        try
        {
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
        }
        catch (Exception)
        {
            // Klasör oluşturulamazsa console-only logging kullan
            basePath = null;
        }

        var config = new LoggerConfiguration()
            // Minimum seviyeler
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .MinimumLevel.Override("Hangfire", LogEventLevel.Information)

            // Enrichers
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentName()
            .Enrich.WithMachineName()
            .Enrich.WithThreadId()
            .Enrich.WithProcessId()
            .Enrich.WithProperty("ServiceName", serviceName)
            .Enrich.WithProperty("Environment", environment)

            // Console çıktısı - Her zaman aktif
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{ServiceName}] {Message:lj}{NewLine}{Exception}",
                restrictedToMinimumLevel: LogEventLevel.Information);

        // Dosya logging - sadece path varsa
        if (!string.IsNullOrEmpty(basePath))
        {
            config = config
                // JSON dosya çıktısı
                .WriteTo.File(
                    new CompactJsonFormatter(),
                    path: Path.Combine(basePath, "log-.json"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    fileSizeLimitBytes: 100_000_000,
                    rollOnFileSizeLimit: true,
                    shared: true)

                // Hata logları ayrı dosyada
                .WriteTo.File(
                    path: Path.Combine(basePath, "errors-.log"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    restrictedToMinimumLevel: LogEventLevel.Error,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level:u3}] [{ServiceName}] {Message:lj}{NewLine}{Exception}");
        }

        return config;
    }

    /// <summary>
    /// WebApplicationBuilder için Serilog extension
    /// </summary>
    public static WebApplicationBuilder AddSerilogLogging(this WebApplicationBuilder builder, string serviceName)
    {
        var environment = builder.Environment.EnvironmentName;

        Log.Logger = CreateConfiguration(serviceName, environment)
            .CreateLogger();

        builder.Host.UseSerilog();

        return builder;
    }

    /// <summary>
    /// IHostBuilder için Serilog extension (Worker servisleri için)
    /// </summary>
    public static IHostBuilder AddSerilogLogging(this IHostBuilder hostBuilder, string serviceName, string environment = "Production")
    {
        Log.Logger = CreateConfiguration(serviceName, environment)
            .CreateLogger();

        return hostBuilder.UseSerilog();
    }
}
