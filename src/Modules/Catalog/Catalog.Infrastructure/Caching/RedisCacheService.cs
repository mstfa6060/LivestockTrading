using System.Text.Json;
using LivestockTrading.Catalog.Application.Abstractions;
using StackExchange.Redis;

namespace LivestockTrading.Catalog.Infrastructure.Caching;

/// <summary>
/// ICacheService impl — StackExchange.Redis IDatabase wrapper (network round-trip).
/// JSON serialization System.Text.Json (BCL, ek NuGet yok); RedisValue native string/byte[]
/// ama generic T için serialize zorunlu. ConnectionMultiplexer Singleton DI (StackExchange.Redis
/// resmi pattern, thread-safe internal connection pool). IDatabase her call'da
/// multiplexer.GetDatabase() — lightweight, multiplexer pool yönetir.
/// CancellationToken ignored (StackExchange.Redis async overloads ct accept etmiyor).
/// </summary>
public sealed class RedisCacheService : ICacheService
{
    private readonly IDatabase _redis;

    public RedisCacheService(IConnectionMultiplexer connection)
        => _redis = connection.GetDatabase();

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct)
    {
        var value = await _redis.StringGetAsync(key);
        if (value.IsNullOrEmpty)
            return default;
        return JsonSerializer.Deserialize<T>(value.ToString());
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? ttl, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(value);
        await _redis.StringSetAsync(key, json);
        if (ttl.HasValue)
            await _redis.KeyExpireAsync(key, ttl.Value);
    }

    public async Task RemoveAsync(string key, CancellationToken ct)
    {
        await _redis.KeyDeleteAsync(key);
    }
}
