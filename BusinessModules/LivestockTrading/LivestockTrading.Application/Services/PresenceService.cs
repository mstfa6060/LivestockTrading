using Common.Services.Caching;

namespace LivestockTrading.Application.Services;

/// <summary>
/// Service for tracking user online/offline status using Redis cache
/// </summary>
public class PresenceService
{
    private readonly ICacheService _cacheService;
    private const string OnlineUsersKey = "chat:online:";
    private const string UserConnectionsKey = "chat:connections:";
    private const string LastSeenKey = "chat:lastseen:";
    private static readonly TimeSpan OnlineTimeout = TimeSpan.FromHours(24);

    public PresenceService(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    /// <summary>
    /// Mark user as online with a specific connection
    /// </summary>
    public async Task SetUserOnlineAsync(Guid userId, string connectionId)
    {
        var connectionsKey = $"{UserConnectionsKey}{userId}";

        // Add connection to user's connection set
        var connections = await _cacheService.GetAsync<HashSet<string>>(connectionsKey) ?? new HashSet<string>();
        connections.Add(connectionId);
        await _cacheService.SetAsync(connectionsKey, connections, OnlineTimeout);

        // Mark user as online
        var onlineKey = $"{OnlineUsersKey}{userId}";
        await _cacheService.SetAsync(onlineKey, true, OnlineTimeout);

        // Update last seen
        var lastSeenKey = $"{LastSeenKey}{userId}";
        await _cacheService.SetAsync(lastSeenKey, DateTime.UtcNow, TimeSpan.FromDays(30));
    }

    /// <summary>
    /// Mark user as offline (when a connection disconnects)
    /// Only marks fully offline if no other connections remain
    /// </summary>
    public async Task SetUserOfflineAsync(Guid userId, string connectionId)
    {
        var connectionsKey = $"{UserConnectionsKey}{userId}";

        // Remove connection from user's connection set
        var connections = await _cacheService.GetAsync<HashSet<string>>(connectionsKey) ?? new HashSet<string>();
        connections.Remove(connectionId);

        if (connections.Count == 0)
        {
            // User has no more connections, mark as offline
            var onlineKey = $"{OnlineUsersKey}{userId}";
            await _cacheService.RemoveAsync(onlineKey);
            await _cacheService.RemoveAsync(connectionsKey);

            // Update last seen
            var lastSeenKey = $"{LastSeenKey}{userId}";
            await _cacheService.SetAsync(lastSeenKey, DateTime.UtcNow, TimeSpan.FromDays(30));
        }
        else
        {
            // User still has other connections
            await _cacheService.SetAsync(connectionsKey, connections, OnlineTimeout);
        }
    }

    /// <summary>
    /// Check if a specific user is online
    /// </summary>
    public async Task<bool> IsUserOnlineAsync(Guid userId)
    {
        var onlineKey = $"{OnlineUsersKey}{userId}";
        var result = await _cacheService.GetAsync<bool?>(onlineKey);
        return result ?? false;
    }

    /// <summary>
    /// Get online status for multiple users
    /// </summary>
    public async Task<Dictionary<Guid, bool>> GetOnlineStatusAsync(IEnumerable<Guid> userIds)
    {
        var result = new Dictionary<Guid, bool>();

        foreach (var userId in userIds)
        {
            result[userId] = await IsUserOnlineAsync(userId);
        }

        return result;
    }

    /// <summary>
    /// Get last seen time for a user
    /// </summary>
    public async Task<DateTime?> GetLastSeenAsync(Guid userId)
    {
        var lastSeenKey = $"{LastSeenKey}{userId}";
        return await _cacheService.GetAsync<DateTime?>(lastSeenKey);
    }

    /// <summary>
    /// Get number of active connections for a user
    /// </summary>
    public async Task<int> GetConnectionCountAsync(Guid userId)
    {
        var connectionsKey = $"{UserConnectionsKey}{userId}";
        var connections = await _cacheService.GetAsync<HashSet<string>>(connectionsKey);
        return connections?.Count ?? 0;
    }

    /// <summary>
    /// Heartbeat to keep user marked as online
    /// </summary>
    public async Task HeartbeatAsync(Guid userId)
    {
        var onlineKey = $"{OnlineUsersKey}{userId}";
        var isOnline = await _cacheService.GetAsync<bool?>(onlineKey);

        if (isOnline == true)
        {
            // Refresh the TTL
            await _cacheService.SetAsync(onlineKey, true, OnlineTimeout);
        }
    }
}

/// <summary>
/// User presence information
/// </summary>
public class UserPresenceInfo
{
    public Guid UserId { get; set; }
    public bool IsOnline { get; set; }
    public DateTime? LastSeen { get; set; }
    public int ConnectionCount { get; set; }
}
