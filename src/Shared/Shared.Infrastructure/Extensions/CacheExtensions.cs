using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Backplane.StackExchangeRedis;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

namespace Shared.Infrastructure.Extensions;

public static class CacheExtensions
{
    public static IServiceCollection AddSharedCache(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var redisConnection = configuration.GetConnectionString("Redis")
            ?? "localhost:6379";

        services.AddFusionCache()
            .WithDefaultEntryOptions(new FusionCacheEntryOptions
            {
                Duration = TimeSpan.FromMinutes(5),
                FailSafeMaxDuration = TimeSpan.FromHours(2),
                FailSafeThrottleDuration = TimeSpan.FromSeconds(30),
            })
            .WithSerializer(new FusionCacheSystemTextJsonSerializer())
            .WithStackExchangeRedisBackplane(opts => opts.Configuration = redisConnection)
            .WithRegisteredMemoryCache();

        return services;
    }
}
