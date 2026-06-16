using FastEndpoints;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;

namespace Livestock.Features.Conversations;

// Legacy alias for the frontend's generated client which was coded against
// an older backend path. Accepts a userId in the body for parity with the
// old contract but defers to the JWT claim (IUserContext) to scope the
// query — the client-supplied value is ignored to keep this authoritative.
public sealed record MessagesUnreadCountRequest(Guid UserId);

public sealed record MessagesUnreadCountResponse(
    int TotalUnreadCount,
    List<ConversationUnreadItem> Conversations
);

public sealed record ConversationUnreadItem(
    Guid ConversationId,
    int UnreadCount,
    DateTime? LastMessageAt
);

public sealed class MessagesUnreadCountEndpoint(LivestockDbContext db, IUserContext user)
    : Endpoint<MessagesUnreadCountRequest, MessagesUnreadCountResponse>
{
    public override void Configure()
    {
        Post("/livestocktrading/Messages/UnreadCount");
        Tags("Messages");
    }

    public override async Task HandleAsync(MessagesUnreadCountRequest req, CancellationToken ct)
    {
        var items = await db.Conversations
            .AsNoTracking()
            .Where(c => (c.InitiatorUserId == user.UserId && c.UnreadCountInitiator > 0)
                     || (c.RecipientUserId == user.UserId && c.UnreadCountRecipient > 0))
            .Select(c => new ConversationUnreadItem(
                c.Id,
                c.InitiatorUserId == user.UserId ? c.UnreadCountInitiator : c.UnreadCountRecipient,
                c.LastMessageAt))
            .ToListAsync(ct);

        var total = items.Sum(i => i.UnreadCount);
        await SendAsync(new MessagesUnreadCountResponse(total, items), 200, ct);
    }
}
