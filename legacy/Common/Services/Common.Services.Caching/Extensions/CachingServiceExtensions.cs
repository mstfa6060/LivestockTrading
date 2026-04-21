using StackExchange.Redis;

namespace Common.Services.Caching.Extensions;

public static class CachingServiceExtensions
{
    public static IServiceCollection AddMadenCaching(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var cachingConfig = configuration.GetSection("Caching").Get<CachingConfiguration>() ?? new CachingConfiguration();

        // Add Memory Cache
        services.AddMemoryCache(options =>
        {
            options.SizeLimit = cachingConfig.Memory.SizeLimitMB * 1024 * 1024; // Convert MB to bytes
            options.CompactionPercentage = cachingConfig.Memory.CompactionPercentage;
        });

        // Add Redis Distributed Cache
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = cachingConfig.Redis.ConnectionString;
            options.InstanceName = cachingConfig.Redis.InstanceName;
        });

        // Add Redis ConnectionMultiplexer for advanced operations (pattern-based deletion)
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var configurationOptions = ConfigurationOptions.Parse(cachingConfig.Redis.ConnectionString);
            configurationOptions.AbortOnConnectFail = false; // Don't fail if Redis is unavailable
            configurationOptions.ConnectTimeout = 5000;
            configurationOptions.SyncTimeout = 5000;
            return ConnectionMultiplexer.Connect(configurationOptions);
        });

        // Add Cache Service
        services.AddSingleton<ICacheService, CacheService>();

        return services;
    }
}
