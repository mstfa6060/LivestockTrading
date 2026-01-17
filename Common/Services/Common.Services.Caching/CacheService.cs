using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Common.Services.Caching;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;
    private readonly CachingConfiguration _config;
    private readonly IConnectionMultiplexer _redis;

    public CacheService(IConfiguration configuration)
    {
        _config = configuration.GetSection("Caching").Get<CachingConfiguration>() ?? new CachingConfiguration();

        // L1: Memory Cache oluştur - SizeLimit KALDIRILDI
        _memoryCache = new MemoryCache(new MemoryCacheOptions
        {
            CompactionPercentage = _config.Memory.CompactionPercentage
            //  SizeLimit kaldırıldı - basit kullanım için gerek yok
        });

        // L2: Redis Distributed Cache oluştur
        try
        {
            var redisOptions = new RedisCacheOptions
            {
                Configuration = _config.Redis.ConnectionString,
                InstanceName = _config.Redis.InstanceName
            };
            _distributedCache = new RedisCache(redisOptions);

            // Redis ConnectionMultiplexer oluştur
            var configOptions = ConfigurationOptions.Parse(_config.Redis.ConnectionString);
            configOptions.AbortOnConnectFail = false;
            configOptions.ConnectTimeout = 5000;
            configOptions.SyncTimeout = 5000;
            _redis = ConnectionMultiplexer.Connect(configOptions);

            Console.WriteLine(" Redis bağlantısı başarılı");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Redis bağlantısı başarısız: {ex.Message}");
            Console.WriteLine("ℹ️ Uygulama sadece Memory Cache ile devam edecek");

            // Redis olmadan devam et (sadece Memory Cache kullan)
            _distributedCache = null;
            _redis = null;
        }
    }

    public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        // L1: Check memory cache
        if (_memoryCache.TryGetValue(key, out T cachedValue))
        {
            return cachedValue;
        }

        // L2: Check distributed cache (Redis) - eğer varsa
        if (_distributedCache != null)
        {
            try
            {
                var distributedValue = await _distributedCache.GetStringAsync(key, cancellationToken);
                if (distributedValue != null)
                {
                    var value = Deserialize<T>(distributedValue);

                    // Populate L1 cache
                    _memoryCache.Set(key, value, TimeSpan.FromMinutes(5));

                    return value;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Redis okuma hatası: {ex.Message}");
            }
        }

        return default;
    }

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<Task<T>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        // Try to get from cache first
        var cached = await GetAsync<T>(key, cancellationToken);
        if (cached != null && !cached.Equals(default(T)))
        {
            return cached;
        }

        // Not in cache, create it
        var value = await factory();

        if (value != null && !value.Equals(default(T)))
        {
            await SetAsync(key, value, expiration, cancellationToken);
        }

        return value;
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        if (value == null)
            return;

        var absoluteExpiration = expiration ?? TimeSpan.FromMinutes(_config.Redis.DefaultExpirationMinutes);

        // L1: Set in memory cache (shorter TTL)
        var memoryExpiration = TimeSpan.FromMinutes(Math.Min(5, absoluteExpiration.TotalMinutes));
        _memoryCache.Set(key, value, memoryExpiration);

        // L2: Set in distributed cache (Redis) - eğer varsa
        if (_distributedCache != null)
        {
            try
            {
                var serialized = Serialize(value);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = absoluteExpiration
                };
                await _distributedCache.SetStringAsync(key, serialized, options, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Redis yazma hatası: {ex.Message}");
            }
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        // Remove from memory cache
        _memoryCache.Remove(key);

        // Remove from distributed cache - eğer varsa
        if (_distributedCache != null)
        {
            try
            {
                await _distributedCache.RemoveAsync(key, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Redis silme hatası: {ex.Message}");
            }
        }
    }

    public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        if (_redis == null) return;

        try
        {
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var keys = server.Keys(pattern: pattern).ToArray();

            if (keys.Length > 0)
            {
                var db = _redis.GetDatabase();

                // Remove from Redis
                await db.KeyDeleteAsync(keys);

                // Remove from memory cache
                foreach (var key in keys)
                {
                    var keyString = key.ToString().Replace(_config.Redis.InstanceName, "");
                    _memoryCache.Remove(keyString);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Redis pattern silme hatası: {ex.Message}");
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        // Check memory cache first
        if (_memoryCache.TryGetValue(key, out _))
        {
            return true;
        }

        // Check distributed cache - eğer varsa
        if (_distributedCache != null)
        {
            try
            {
                var value = await _distributedCache.GetStringAsync(key, cancellationToken);
                return value != null;
            }
            catch
            {
                return false;
            }
        }

        return false;
    }

    public async Task RefreshAsync(string key, CancellationToken cancellationToken = default)
    {
        if (_distributedCache != null)
        {
            try
            {
                await _distributedCache.RefreshAsync(key, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Redis refresh hatası: {ex.Message}");
            }
        }
    }

    private string Serialize<T>(T value)
    {
        var settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        };
        return JsonConvert.SerializeObject(value, settings);
    }

    private T Deserialize<T>(string value)
    {
        var settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        return JsonConvert.DeserializeObject<T>(value, settings);
    }
}