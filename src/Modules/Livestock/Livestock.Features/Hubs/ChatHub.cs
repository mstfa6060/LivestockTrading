using System.Security.Claims;
using Livestock.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ZiggyCreatures.Caching.Fusion;

namespace Livestock.Features.Hubs;

[Authorize]
public class ChatHub(LivestockDbContext db, IFusionCache cache) : Hub
{
    private const string OnlineUserKeyPrefix = "chat:online:";
    private const string UserConnectionsKeyPrefix = "chat:connections:";
    private static readonly TimeSpan PresenceTtl = TimeSpan.FromHours(24);

    public async Task JoinConversation(Guid conversationId)
    {
        var userId = GetUserId();
        await EnsureMembershipAsync(conversationId, userId);
        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(conversationId));
    }

    public async Task LeaveConversation(Guid conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(conversationId));
    }

    public async Task SendTypingIndicator(Guid conversationId, bool isTyping)
    {
        var userId = GetUserId();
        await EnsureMembershipAsync(conversationId, userId);

        await Clients.OthersInGroup(GroupName(conversationId)).SendAsync("TypingIndicator", new
        {
            conversationId,
            userId,
            isTyping,
            timestamp = DateTime.UtcNow,
        });
    }

    public async Task MarkMessageAsRead(Guid messageId, Guid conversationId)
    {
        var userId = GetUserId();
        await EnsureMembershipAsync(conversationId, userId);

        await Clients.OthersInGroup(GroupName(conversationId)).SendAsync("MessageRead", new
        {
            messageId,
            conversationId,
            readByUserId = userId,
            readAt = DateTime.UtcNow,
        });
    }

    public async Task<List<OnlineUserInfo>> GetOnlineUsers(List<Guid> userIds)
    {
        var result = new List<OnlineUserInfo>(userIds.Count);
        foreach (var userId in userIds)
        {
            var key = OnlineUserKeyPrefix + userId;
            var isOnline = await cache.TryGetAsync<bool>(key);
            result.Add(new OnlineUserInfo(userId, isOnline.HasValue && isOnline.Value));
        }
        return result;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        if (userId != Guid.Empty)
        {
            await SetUserOnlineAsync(userId);
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId();
        if (userId != Guid.Empty)
        {
            await SetUserOfflineAsync(userId);
        }
        await base.OnDisconnectedAsync(exception);
    }

    private async Task EnsureMembershipAsync(Guid conversationId, Guid userId)
    {
        var isMember = await db.Conversations
            .AsNoTracking()
            .AnyAsync(c => c.Id == conversationId
                        && !c.IsDeleted
                        && (c.InitiatorUserId == userId || c.RecipientUserId == userId));

        if (!isMember)
        {
            throw new HubException("You are not a participant in this conversation.");
        }
    }

    private Guid GetUserId()
    {
        var claim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                 ?? Context.User?.FindFirst("sub")?.Value;
        return Guid.TryParse(claim, out var userId) ? userId : Guid.Empty;
    }

    private static string GroupName(Guid conversationId) => $"conversation_{conversationId}";

    private async Task SetUserOnlineAsync(Guid userId)
    {
        var connectionsKey = UserConnectionsKeyPrefix + userId;
        var onlineKey = OnlineUserKeyPrefix + userId;

        var connections = await cache.GetOrDefaultAsync<HashSet<string>>(connectionsKey, []);
        connections!.Add(Context.ConnectionId);

        await cache.SetAsync(connectionsKey, connections, PresenceTtl);
        await cache.SetAsync(onlineKey, true, PresenceTtl);

        await Clients.All.SendAsync("UserOnline", new { userId, timestamp = DateTime.UtcNow });
    }

    private async Task SetUserOfflineAsync(Guid userId)
    {
        var connectionsKey = UserConnectionsKeyPrefix + userId;
        var onlineKey = OnlineUserKeyPrefix + userId;

        var connections = await cache.GetOrDefaultAsync<HashSet<string>>(connectionsKey, []);
        connections!.Remove(Context.ConnectionId);

        if (connections.Count == 0)
        {
            await cache.RemoveAsync(connectionsKey);
            await cache.RemoveAsync(onlineKey);
            await Clients.All.SendAsync("UserOffline", new { userId, timestamp = DateTime.UtcNow });
        }
        else
        {
            await cache.SetAsync(connectionsKey, connections, PresenceTtl);
        }
    }
}

public sealed record OnlineUserInfo(Guid UserId, bool IsOnline);
