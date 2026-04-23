# Maden Backend Caching Implementation

## 📋 Overview

This caching system provides a two-tier caching strategy using Redis (distributed cache) and in-memory cache for optimal performance.

### Features
-  Two-tier caching (L1: Memory, L2: Redis)
-  Automatic failover (continues if Redis is down)
-  Pattern-based cache invalidation
-  Centralized cache key management
-  Configurable expiration times
-  Thread-safe operations

## 🚀 Quick Start

### 1. Configuration

Add to your `appsettings.json`:

```json
{
  "Caching": {
    "Redis": {
      "ConnectionString": "localhost:6379",
      "InstanceName": "maden:",
      "DefaultExpirationMinutes": 60
    },
    "Memory": {
      "SizeLimitMB": 1024,
      "CompactionPercentage": 0.25
    }
  }
}
```

### 2. Service Registration

In your `Program.cs`:

```csharp
using Common.Services.Caching.Extensions;

// Add caching services
builder.Services.AddMadenCaching(builder.Configuration);
```

### 3. Usage

#### Basic Usage

```csharp
public class YourService
{
    private readonly ICacheService _cacheService;

    public YourService(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task<User> GetUserAsync(Guid userId)
    {
        var cacheKey = CacheKeys.User.Profile(userId);

        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async () => await _dbContext.Users.FindAsync(userId),
            TimeSpan.FromMinutes(15)
        );
    }
}
```

#### Manual Cache Operations

```csharp
// Get from cache
var user = await _cacheService.GetAsync<User>(cacheKey);

// Set in cache
await _cacheService.SetAsync(cacheKey, user, TimeSpan.FromMinutes(30));

// Remove from cache
await _cacheService.RemoveAsync(cacheKey);

// Remove by pattern (e.g., all user caches)
await _cacheService.RemoveByPatternAsync("user:profile:*");

// Check if exists
var exists = await _cacheService.ExistsAsync(cacheKey);
```

## 📊 Cache Keys

Use centralized cache keys from `CacheKeys` class:

```csharp
// Authorization
CacheKeys.Authorization.UserRoles(userId)
CacheKeys.Authorization.UserPermissions(userId)
CacheKeys.Authorization.SystemAdmin(userId)

// Reference Data
CacheKeys.Reference.AnimalBreeds
CacheKeys.Reference.Skills(language)
CacheKeys.Reference.ServiceAreas

// User Data
CacheKeys.User.Profile(userId)
CacheKeys.User.Company(userId)

// Location
CacheKeys.Location.NearbyJobs(lat, lng, radius)
```

## 🎯 Implemented Examples

### 1. Authorization Service

Cache user roles and permissions:

```csharp
public async Task<bool> IsSystemAdmin(ModuleTypes moduleType)
{
    var currentUserId = _currentUserService.GetCurrentUserId();
    var cacheKey = CacheKeys.Authorization.SystemAdmin(currentUserId);

    return await _cacheService.GetOrCreateAsync(
        cacheKey,
        async () =>
        {
            return await _dbContext.AppSystemAdmins
                .Include(a => a.RelSystemUserModules)
                .AnyAsync(s => s.UserId == currentUserId && s.IsActive);
        },
        TimeSpan.FromMinutes(30)
    );
}
```

### 2. Reference Data

Cache rarely-changing data:

```csharp
public async Task<List<AnimalBreed>> GetAllBreedsAsync()
{
    var cacheKey = CacheKeys.Reference.AnimalBreeds;

    return await _cacheService.GetOrCreateAsync(
        cacheKey,
        async () =>
        {
            return await _dbContext.AnimalBreeds
                .Where(b => !b.IsDeleted)
                .OrderBy(b => b.Name)
                .ToListAsync();
        },
        TimeSpan.FromHours(24)
    );
}
```

### 3. Cache Invalidation

When data changes, invalidate related caches:

```csharp
public async Task UpdateUserRoleAsync(Guid userId, Guid roleId)
{
    // Update database
    await _dbContext.SaveChangesAsync();

    // Invalidate cache
    await _cacheService.RemoveAsync(CacheKeys.Authorization.UserRoles(userId));
    await _cacheService.RemoveAsync(CacheKeys.Authorization.UserPermissions(userId));
}
```

## ⚙️ Configuration Options

### Redis Configuration

| Property | Default | Description |
|----------|---------|-------------|
| ConnectionString | localhost:6379 | Redis server connection |
| InstanceName | maden: | Key prefix for all cache entries |
| DefaultExpirationMinutes | 60 | Default TTL for cached items |

### Memory Configuration

| Property | Default | Description |
|----------|---------|-------------|
| SizeLimitMB | 1024 | Maximum memory cache size |
| CompactionPercentage | 0.25 | Percentage to compact when full |

## 🔄 Cache Strategy

1. **L1 Cache (Memory)**: Fast, in-process cache with short TTL (5 min)
2. **L2 Cache (Redis)**: Distributed cache with longer TTL (configurable)

Flow:
1. Check L1 (memory) → if found, return
2. Check L2 (Redis) → if found, populate L1 and return
3. Execute factory function → store in both L1 and L2

## 📈 Recommended TTL Values

| Data Type | TTL | Rationale |
|-----------|-----|-----------|
| Authorization | 15-30 min | Balance security & performance |
| Reference Data | 24 hours | Rarely changes |
| User Profile | 5-15 min | May change occasionally |
| Location Data | 2-10 min | Real-time sensitivity |
| Static Config | 1 hour+ | Very stable |

## 🛠️ Docker Configuration

Redis is configured in `docker-compose`:

```yaml
redis:
  image: redis:7-alpine
  ports:
    - "6379:6379"
  command: redis-server --appendonly yes --maxmemory 2gb --maxmemory-policy allkeys-lru
  healthcheck:
    test: ["CMD", "redis-cli", "ping"]
    interval: 10s
```

## 🧪 Testing

```bash
# Start Redis
docker-compose up redis

# Test connection
docker exec -it <redis-container> redis-cli ping
# Expected: PONG

# View cache keys
docker exec -it <redis-container> redis-cli KEYS "maden:*"
```

## 📊 Performance Impact

Expected improvements:
- **Authorization**: 80-90% faster (20ms → 2ms)
- **Reference Data**: 95% faster (20ms → 1ms)
- **Overall API**: 40-60% response time reduction
- **Database Load**: 60-70% reduction

## 🔒 Security Considerations

- Sensitive data is cached temporarily (short TTL for auth)
- Cache keys are namespaced to prevent collisions
- Redis persistence is optional (configured with AOF)
- Connection timeout prevents hanging requests

## 🐛 Troubleshooting

**Redis not connecting:**
- Check if Redis container is running
- Verify connection string in appsettings
- Application continues without Redis (degraded performance)

**High memory usage:**
- Adjust `SizeLimitMB` in Memory configuration
- Reduce default expiration times
- Implement cache eviction patterns

**Cache stampede:**
- Use `GetOrCreateAsync` for automatic locking
- Consider adding jitter to expiration times

## 📝 Best Practices

1.  Always use `CacheKeys` class for key management
2.  Set appropriate TTL based on data volatility
3.  Invalidate cache when data changes
4.  Use pattern-based removal for related data
5.  Monitor cache hit rates in production
6. ❌ Don't cache user-specific sensitive data for long periods
7. ❌ Don't store large objects without size limits
8. ❌ Don't rely on cache for critical data consistency

## 🔗 Related Files

- Cache Service: `/Common/Services/Common.Services.Caching/`
- Authorization: `/Common/Services/Common.Services.Auth/Authorization/`
- Docker Config: `/_devops/docker/compose/common.yml`
- App Settings: `**/appsettings.json`

---

**Version:** 1.0
**Last Updated:** 2025-01-17
**Maintainer:** Maden Backend Team
