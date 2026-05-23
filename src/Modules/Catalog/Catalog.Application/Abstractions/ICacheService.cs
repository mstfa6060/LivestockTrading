namespace LivestockTrading.Catalog.Application.Abstractions;

/// <summary>
/// Cache abstraction port — Plan-1 Karar 4 grounded. 3-metot minimum (Get/Set/Remove);
/// GetOrSetAsync HARİÇ — race-condition handling impl-leak (Memory lock vs Redis SETNX
/// abstract interface'de gizlenemez; W3.5B fiili usage sonrası W3.6+ revisit).
/// Implementation in Catalog.Infrastructure/Caching/ (Memory veya Redis, config-driven
/// `Catalog:CacheProvider`). Cache-miss = default(T); TTL null = no expiration.
/// </summary>
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct);

    Task SetAsync<T>(string key, T value, TimeSpan? ttl, CancellationToken ct);

    Task RemoveAsync(string key, CancellationToken ct);
}
