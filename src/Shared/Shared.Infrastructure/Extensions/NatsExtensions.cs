using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NATS.Client.Core;
using NATS.Extensions.Microsoft.DependencyInjection;
using Shared.Infrastructure.Messaging;

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

        // AddNatsClient registers INatsConnection (which already implements
        // INatsClient as the high-level surface). Aliasing it here lets
        // consumers and publishers depend on INatsClient directly.
        services.AddSingleton<INatsClient>(sp => sp.GetRequiredService<INatsConnection>());

        services.AddSingleton<IEventPublisher, NatsEventPublisher>();

        return services;
    }
}
