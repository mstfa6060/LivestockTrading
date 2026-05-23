using LivestockTrading.Catalog.Application.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace LivestockTrading.Catalog.Infrastructure.Caching;

/// <summary>
/// ICacheService impl — IMemoryCache wrapper (in-process, type-safe direct ref store).
/// JSON serialization YOK — Memory cache `object` slot generic T type-safe cast. Sync
/// IMemoryCache API → Task.FromResult / Task.CompletedTask wrap (port async imza karşılanır).
/// CancellationToken ignored — in-memory ops nanoseconds, ct overhead anlamsız.
/// </summary>
public sealed class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;

    public MemoryCacheService(IMemoryCache cache) => _cache = cache;

    public Task<T?> GetAsync<T>(string key, CancellationToken ct)
    {
        if (_cache.TryGetValue<T>(key, out var value))
            return Task.FromResult<T?>(value);
        return Task.FromResult<T?>(default);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? ttl, CancellationToken ct)
    {
        var opts = new MemoryCacheEntryOptions();
        if (ttl.HasValue)
            opts.AbsoluteExpirationRelativeToNow = ttl.Value;
        _cache.Set(key, value, opts);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken ct)
    {
        _cache.Remove(key);
        return Task.CompletedTask;
    }
}
