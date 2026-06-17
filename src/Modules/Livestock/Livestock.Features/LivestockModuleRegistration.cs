using Livestock.Features.Consumers;
using Livestock.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Abstractions.Identity;
using Shared.Infrastructure.Identity;

namespace Livestock.Features;

public static class LivestockModuleRegistration
{
    public static IServiceCollection AddLivestockModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddLivestockPersistence(configuration);
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, HttpUserContext>();

        services.AddHostedService<ProductPendingAdminNotificationConsumer>();
        services.AddHostedService<SellerPendingAdminNotificationConsumer>();

        return services;
    }
}
