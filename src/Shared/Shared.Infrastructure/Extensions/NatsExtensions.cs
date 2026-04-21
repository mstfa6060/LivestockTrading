using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NATS.Extensions.Microsoft.DependencyInjection;

namespace Shared.Infrastructure.Extensions;

public static class NatsExtensions
{
    public static IServiceCollection AddSharedNats(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var natsUrl = configuration.GetConnectionString("Nats")
            ?? "nats://localhost:4222";

        services.AddNatsClient(builder =>
            builder.ConfigureOptions(opts => opts with { Url = natsUrl }));

        return services;
    }
}
