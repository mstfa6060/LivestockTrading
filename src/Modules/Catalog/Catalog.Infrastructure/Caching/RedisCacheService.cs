using System.Text.Json;
using LivestockTrading.Catalog.Application.Abstractions;
using LivestockTrading.Catalog.Infrastructure.Caching.JsonConverters;
using StackExchange.Redis;

namespace LivestockTrading.Catalog.Infrastructure.Caching;

/// <summary>
/// ICacheService impl — StackExchange.Redis IDatabase wrapper (network round-trip).
/// JSON serialization System.Text.Json (BCL, ek NuGet yok); RedisValue native string/byte[]
/// ama generic T için serialize zorunlu. ConnectionMultiplexer Singleton DI (StackExchange.Redis
/// resmi pattern, thread-safe internal connection pool). IDatabase her call'da
/// multiplexer.GetDatabase() — lightweight, multiplexer pool yönetir.
/// CancellationToken ignored (StackExchange.Redis async overloads ct accept etmiyor).
/// F-S51 (W3.6.A.1.5): TranslationsJsonConverter Translations sealed class Pure POCO
/// Domain saf STJ-default-deser uyumsuzlugunu kapatir. Static readonly JsonOptions
/// tum Get/Set cagrilarinda paylasilir.
/// </summary>
public sealed class RedisCacheService : ICacheService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        Converters = { new TranslationsJsonConverter() }
    };

    private readonly IDatabase _redis;

    public RedisCacheService(IConnectionMultiplexer connection)
        => _redis = connection.GetDatabase();

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct)
    {
        var value = await _redis.StringGetAsync(key);
        if (value.IsNullOrEmpty)
            return default;
        return JsonSerializer.Deserialize<T>(value.ToString(), JsonOptions);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? ttl, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(value, JsonOptions);
        await _redis.StringSetAsync(key, json);
        if (ttl.HasValue)
            await _redis.KeyExpireAsync(key, ttl.Value);
    }

    public async Task RemoveAsync(string key, CancellationToken ct)
    {
        await _redis.KeyDeleteAsync(key);
    }
}
