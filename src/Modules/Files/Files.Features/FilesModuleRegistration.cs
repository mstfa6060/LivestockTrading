using Files.Features.Services;
using Files.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace Files.Features;

public static class FilesModuleRegistration
{
    public static IServiceCollection AddFilesModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddFilesPersistence(configuration);

        var settings = configuration.GetSection("Minio").Get<MinioSettings>() ?? new MinioSettings();
        services.Configure<MinioSettings>(configuration.GetSection("Minio"));

        services.AddMinio(client => client
            .WithEndpoint(settings.Endpoint)
            .WithCredentials(settings.AccessKey, settings.SecretKey)
            .WithSSL(settings.UseSSL));

        services.AddScoped<IStorageService, MinioStorageService>();

        return services;
    }
}
