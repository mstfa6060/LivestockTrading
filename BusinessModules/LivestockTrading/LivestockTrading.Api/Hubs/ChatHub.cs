using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Common.Services.Caching;
using LivestockTrading.Infrastructure.RelationalDB;
using Serilog;

namespace LivestockTrading.Api.Hubs;

/// <summary>
/// Real-time chat hub for messaging functionality
/// Handles: message delivery, typing indicators, presence tracking
/// </summary>
[Authorize]
public class ChatHub : Hub
{
    private readonly ICacheService _cacheService;
    private readonly LivestockTradingModuleDbContext _dbContext;
    private const string OnlineUsersKey = "chat:online:";
    private const string UserConnectionsKey = "chat:connections:";

    public ChatHub(ICacheService cacheService, LivestockTradingModuleDbContext dbContext)
    {
        _cacheService = cacheService;
        _dbContext = dbContext;
    }

    /// <summary>
    /// Join a conversation room to receive real-time updates
    /// </summary>
    public async Task JoinConversation(Guid conversationId)
    {
        var userId = GetUserId();
        await VerifyConversationMembership(conversationId, userId);

        var groupName = GetConversationGroupName(conversationId);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        Log.Information("User {UserId} joined conversation {ConversationId}", userId, conversationId);
    }

    /// <summary>
    /// Leave a conversation room
    /// </summary>
    public async Task LeaveConversation(Guid conversationId)
    {
        var groupName = GetConversationGroupName(conversationId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        Log.Information("User {UserId} left conversation {ConversationId}", GetUserId(), conversationId);
    }

    /// <summary>
    /// Send typing indicator to conversation participants
    /// </summary>
    public async Task SendTypingIndicator(Guid conversationId, bool isTyping)
    {
        var userId = GetUserId();
        await VerifyConversationMembership(conversationId, userId);

        var groupName = GetConversationGroupName(conversationId);

        await Clients.OthersInGroup(groupName).SendAsync("TypingIndicator", new
        {
            conversationId,
            userId,
            isTyping,
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Notify that a message has been read
    /// </summary>
    public async Task MarkMessageAsRead(Guid messageId, Guid conversationId)
    {
        var userId = GetUserId();
        await VerifyConversationMembership(conversationId, userId);

        var groupName = GetConversationGroupName(conversationId);

        await Clients.OthersInGroup(groupName).SendAsync("MessageRead", new
        {
            messageId,
            conversationId,
            readByUserId = userId,
            readAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Get online status of specific users
    /// </summary>
    public async Task<List<OnlineUserInfo>> GetOnlineUsers(List<Guid> userIds)
    {
        var result = new List<OnlineUserInfo>();

        foreach (var userId in userIds)
        {
            var isOnline = await IsUserOnline(userId);
            result.Add(new OnlineUserInfo
            {
                UserId = userId,
                IsOnline = isOnline
            });
        }

        return result;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        if (userId != Guid.Empty)
        {
            await SetUserOnline(userId);
            Log.Information("User {UserId} connected. ConnectionId: {ConnectionId}", userId, Context.ConnectionId);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var userId = GetUserId();
        if (userId != Guid.Empty)
        {
            await SetUserOffline(userId);
            Log.Information("User {UserId} disconnected. ConnectionId: {ConnectionId}", userId, Context.ConnectionId);
        }

        if (exception != null)
        {
            Log.Error(exception, "User {UserId} disconnected with error", userId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    #region Private Methods

    /// <summary>
    /// Verifies that the authenticated user is a participant in the conversation.
    /// Throws HubException if user is not a member.
    /// </summary>
    private async Task VerifyConversationMembership(Guid conversationId, Guid userId)
    {
        var isMember = await _dbContext.Conversations
            .AsNoTracking()
            .AnyAsync(c => c.Id == conversationId
                && !c.IsDeleted
                && (c.ParticipantUserId1 == userId || c.ParticipantUserId2 == userId));

        if (!isMember)
        {
            Log.Warning("User {UserId} attempted to access conversation {ConversationId} without membership", userId, conversationId);
            throw new HubException("You are not a participant in this conversation.");
        }
    }

    private Guid GetUserId()
    {
        var userIdClaim = Context.User?.FindFirst("userId")?.Value
            ?? Context.User?.FindFirst("sub")?.Value;

        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }

    private static string GetConversationGroupName(Guid conversationId)
    {
        return $"conversation_{conversationId}";
    }

    private async Task SetUserOnline(Guid userId)
    {
        var key = $"{OnlineUsersKey}{userId}";
        var connectionsKey = $"{UserConnectionsKey}{userId}";

        // Add connection to user's connection set
        var connections = await _cacheService.GetAsync<HashSet<string>>(connectionsKey) ?? new HashSet<string>();
        connections.Add(Context.ConnectionId);
        await _cacheService.SetAsync(connectionsKey, connections, TimeSpan.FromHours(24));

        // Mark user as online
        await _cacheService.SetAsync(key, true, TimeSpan.FromHours(24));

        // Broadcast online status to all clients
        await Clients.All.SendAsync("UserOnline", new { userId, timestamp = DateTime.UtcNow });
    }

    private async Task SetUserOffline(Guid userId)
    {
        var key = $"{OnlineUsersKey}{userId}";
        var connectionsKey = $"{UserConnectionsKey}{userId}";

        // Remove connection from user's connection set
        var connections = await _cacheService.GetAsync<HashSet<string>>(connectionsKey) ?? new HashSet<string>();
        connections.Remove(Context.ConnectionId);

        if (connections.Count == 0)
        {
            // User has no more connections, mark as offline
            await _cacheService.RemoveAsync(key);
            await _cacheService.RemoveAsync(connectionsKey);

            // Broadcast offline status to all clients
            await Clients.All.SendAsync("UserOffline", new { userId, timestamp = DateTime.UtcNow });
        }
        else
        {
            // User still has other connections
            await _cacheService.SetAsync(connectionsKey, connections, TimeSpan.FromHours(24));
        }
    }

    private async Task<bool> IsUserOnline(Guid userId)
    {
        var key = $"{OnlineUsersKey}{userId}";
        var result = await _cacheService.GetAsync<bool?>(key);
        return result ?? false;
    }

    #endregion
}

public class OnlineUserInfo
{
    public Guid UserId { get; set; }
    public bool IsOnline { get; set; }
}
