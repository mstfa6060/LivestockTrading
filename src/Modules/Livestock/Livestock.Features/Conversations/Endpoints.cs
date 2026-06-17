using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Enums;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;
using Shared.Contracts.Events.Livestock;
using Shared.Infrastructure.Messaging;

namespace Livestock.Features.Conversations;

public class GetMyConversationsEndpoint(LivestockDbContext db, IUserContext user) : EndpointWithoutRequest<List<ConversationListItem>>
{
    public override void Configure()
    {
        Post("/livestocktrading/Conversations/My");
        Tags("Conversations");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var conversations = await db.Conversations
            .AsNoTracking()
            .Where(c => c.InitiatorUserId == user.UserId || c.RecipientUserId == user.UserId)
            .OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt)
            .Select(c => new ConversationListItem(
                c.Id, c.InitiatorUserId, c.RecipientUserId, c.ProductId, c.Status, c.LastMessageAt,
                c.InitiatorUserId == user.UserId ? c.UnreadCountInitiator : c.UnreadCountRecipient,
                c.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(conversations, 200, ct);
    }
}

public class GetConversationEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<GetConversationRequest, ConversationDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/Conversations/Detail");
        Tags("Conversations");
    }

    public override async Task HandleAsync(GetConversationRequest req, CancellationToken ct)
    {
        var c = await db.Conversations.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.Id, ct);
        if (c is null)
        {
            AddError(LivestockErrors.ConversationErrors.ConversationNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        if (c.InitiatorUserId != user.UserId && c.RecipientUserId != user.UserId)
        {
            AddError(LivestockErrors.ConversationErrors.ConversationNotParticipant);
            await SendErrorsAsync(403, ct);
            return;
        }

        await SendAsync(new ConversationDetail(c.Id, c.InitiatorUserId, c.RecipientUserId, c.ProductId, c.Status, c.LastMessageAt, c.CreatedAt), 200, ct);
    }
}

public class CreateConversationEndpoint(LivestockDbContext db, IUserContext user, IEventPublisher publisher) : Endpoint<CreateConversationRequest, ConversationDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/Conversations/Create");
        Tags("Conversations");
    }

    public override async Task HandleAsync(CreateConversationRequest req, CancellationToken ct)
    {
        var existing = await db.Conversations.FirstOrDefaultAsync(c =>
            c.ProductId == req.ProductId &&
            ((c.InitiatorUserId == user.UserId && c.RecipientUserId == req.RecipientUserId) ||
             (c.InitiatorUserId == req.RecipientUserId && c.RecipientUserId == user.UserId)), ct);

        Conversation conversation;
        var isNew = false;
        if (existing is not null)
        {
            conversation = existing;
        }
        else
        {
            conversation = new Conversation
            {
                InitiatorUserId = user.UserId,
                RecipientUserId = req.RecipientUserId,
                ProductId = req.ProductId,
                Status = ConversationStatus.Active,
                LastMessageAt = DateTime.UtcNow
            };
            db.Conversations.Add(conversation);
            await db.SaveChangesAsync(ct);
            isNew = true;
        }

        var message = new Message
        {
            ConversationId = conversation.Id,
            SenderUserId = user.UserId,
            RecipientUserId = req.RecipientUserId,
            Content = req.FirstMessage
        };
        db.Messages.Add(message);
        conversation.LastMessageAt = DateTime.UtcNow;
        conversation.UnreadCountRecipient++;
        await db.SaveChangesAsync(ct);

        if (isNew)
        {
            await publisher.PublishAsync(ConversationCreatedEvent.Subject, new ConversationCreatedEvent
            {
                ConversationId = conversation.Id,
                InitiatorUserId = conversation.InitiatorUserId,
                RecipientUserId = conversation.RecipientUserId,
                ProductId = conversation.ProductId
            }, ct);
        }

        await publisher.PublishAsync(MessageSentEvent.Subject, new MessageSentEvent
        {
            MessageId = message.Id,
            ConversationId = conversation.Id,
            SenderUserId = message.SenderUserId,
            RecipientUserId = message.RecipientUserId,
            Content = message.Content
        }, ct);

        await SendAsync(new ConversationDetail(conversation.Id, conversation.InitiatorUserId, conversation.RecipientUserId, conversation.ProductId, conversation.Status, conversation.LastMessageAt, conversation.CreatedAt), 201, ct);
    }
}

public class GetConversationMessagesEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<GetConversationMessagesRequest, List<MessageItem>>
{
    public override void Configure()
    {
        Post("/livestocktrading/Conversations/Messages");
        Tags("Conversations");
    }

    public override async Task HandleAsync(GetConversationMessagesRequest req, CancellationToken ct)
    {
        var conversation = await db.Conversations.AsNoTracking().FirstOrDefaultAsync(c => c.Id == req.ConversationId, ct);
        if (conversation is null || (conversation.InitiatorUserId != user.UserId && conversation.RecipientUserId != user.UserId))
        {
            AddError(LivestockErrors.ConversationErrors.ConversationNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        var pageSize = Math.Min(req.PageSize, 100);
        var skip = (req.Page - 1) * pageSize;

        var messages = await db.Messages
            .AsNoTracking()
            .Where(m => m.ConversationId == req.ConversationId)
            .OrderByDescending(m => m.CreatedAt)
            .Skip(skip).Take(pageSize)
            .Select(m => new MessageItem(m.Id, m.ConversationId, m.SenderUserId, m.RecipientUserId, m.Content, m.IsRead, m.ReadAt, m.AttachmentUrl, m.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(messages, 200, ct);
    }
}

public class SendMessageEndpoint(LivestockDbContext db, IUserContext user, IEventPublisher publisher) : Endpoint<SendMessageRequest, MessageItem>
{
    public override void Configure()
    {
        Post("/livestocktrading/Conversations/SendMessage");
        Tags("Conversations");
    }

    public override async Task HandleAsync(SendMessageRequest req, CancellationToken ct)
    {
        var conversation = await db.Conversations.FirstOrDefaultAsync(c => c.Id == req.ConversationId, ct);
        if (conversation is null || (conversation.InitiatorUserId != user.UserId && conversation.RecipientUserId != user.UserId))
        {
            AddError(LivestockErrors.ConversationErrors.ConversationNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        var recipientId = conversation.InitiatorUserId == user.UserId
            ? conversation.RecipientUserId
            : conversation.InitiatorUserId;

        var message = new Message
        {
            ConversationId = req.ConversationId,
            SenderUserId = user.UserId,
            RecipientUserId = recipientId,
            Content = req.Content,
            AttachmentUrl = req.AttachmentUrl
        };

        db.Messages.Add(message);
        conversation.LastMessageAt = DateTime.UtcNow;
        if (conversation.InitiatorUserId == user.UserId)
        {
            conversation.UnreadCountRecipient++;
        }
        else
        {
            conversation.UnreadCountInitiator++;
        }

        await db.SaveChangesAsync(ct);

        await publisher.PublishAsync(MessageSentEvent.Subject, new MessageSentEvent
        {
            MessageId = message.Id,
            ConversationId = message.ConversationId,
            SenderUserId = message.SenderUserId,
            RecipientUserId = message.RecipientUserId,
            Content = message.Content,
            AttachmentUrl = message.AttachmentUrl
        }, ct);

        await SendAsync(new MessageItem(message.Id, message.ConversationId, message.SenderUserId, message.RecipientUserId, message.Content, false, null, message.AttachmentUrl, message.CreatedAt), 201, ct);
    }
}

public class GetUnreadCountEndpoint(LivestockDbContext db, IUserContext user) : EndpointWithoutRequest<UnreadCountResponse>
{
    public override void Configure()
    {
        Post("/livestocktrading/Conversations/UnreadCount");
        Tags("Conversations");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var items = await db.Conversations
            .AsNoTracking()
            .Where(c => (c.InitiatorUserId == user.UserId && c.UnreadCountInitiator > 0)
                     || (c.RecipientUserId == user.UserId && c.UnreadCountRecipient > 0))
            .Select(c => new UnreadCountItem(
                c.Id,
                c.InitiatorUserId == user.UserId ? c.UnreadCountInitiator : c.UnreadCountRecipient))
            .ToListAsync(ct);

        var total = items.Sum(i => i.UnreadCount);
        await SendAsync(new UnreadCountResponse(total, items), 200, ct);
    }
}

public class MarkMessagesReadEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<MarkMessagesReadRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/livestocktrading/Conversations/MarkRead");
        Tags("Conversations");
    }

    public override async Task HandleAsync(MarkMessagesReadRequest req, CancellationToken ct)
    {
        var conversation = await db.Conversations.FirstOrDefaultAsync(c => c.Id == req.ConversationId, ct);
        if (conversation is null || (conversation.InitiatorUserId != user.UserId && conversation.RecipientUserId != user.UserId))
        {
            AddError(LivestockErrors.ConversationErrors.ConversationNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        await db.Messages
            .Where(m => m.ConversationId == req.ConversationId && m.RecipientUserId == user.UserId && !m.IsRead)
            .ExecuteUpdateAsync(s => s
                .SetProperty(m => m.IsRead, true)
                .SetProperty(m => m.ReadAt, DateTime.UtcNow), ct);

        if (conversation.InitiatorUserId == user.UserId)
        {
            conversation.UnreadCountInitiator = 0;
        }
        else
        {
            conversation.UnreadCountRecipient = 0;
        }

        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}
