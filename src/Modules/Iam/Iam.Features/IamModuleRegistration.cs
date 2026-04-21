using Iam.Features.Services;
using Iam.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Iam.Features;

public static class IamModuleRegistration
{
    public static IServiceCollection AddIamModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddIamPersistence(configuration);

        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<INotificationPublisher, LoggingNotificationPublisher>();

        return services;
    }
}
