using FastEndpoints;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;

namespace Livestock.Features.Messages;

public class GetMessageEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<GetMessageRequest, MessageDetail>
{
    public override void Configure()
    {
        Get("/Messages/{Id}");
        Tags("Messages");
    }

    public override async Task HandleAsync(GetMessageRequest req, CancellationToken ct)
    {
        var m = await db.Messages.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (m is null)
        {
            AddError(LivestockErrors.MessageErrors.MessageNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        if (m.SenderUserId != user.UserId && m.RecipientUserId != user.UserId)
        {
            AddError(LivestockErrors.Common.Unauthorized);
            await SendErrorsAsync(403, ct);
            return;
        }

        await SendAsync(new MessageDetail(m.Id, m.ConversationId, m.SenderUserId, m.RecipientUserId, m.Content, m.IsRead, m.ReadAt, m.AttachmentUrl, m.AttachmentType, m.CreatedAt, m.UpdatedAt), 200, ct);
    }
}

public class GetMessagesByConversationEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<GetMessagesByConversationRequest, List<MessageListItem>>
{
    public override void Configure()
    {
        Get("/Messages/ByConversation/{ConversationId}");
        Tags("Messages");
    }

    public override async Task HandleAsync(GetMessagesByConversationRequest req, CancellationToken ct)
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
            .Where(m => m.ConversationId == req.ConversationId && !m.IsDeleted)
            .OrderByDescending(m => m.CreatedAt)
            .Skip(skip).Take(pageSize)
            .Select(m => new MessageListItem(m.Id, m.ConversationId, m.SenderUserId, m.RecipientUserId, m.Content, m.IsRead, m.ReadAt, m.AttachmentUrl, m.AttachmentType, m.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(messages, 200, ct);
    }
}

public class UpdateMessageEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<UpdateMessageRequest, MessageDetail>
{
    public override void Configure()
    {
        Put("/Messages/{Id}");
        Tags("Messages");
    }

    public override async Task HandleAsync(UpdateMessageRequest req, CancellationToken ct)
    {
        var m = await db.Messages.FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (m is null)
        {
            AddError(LivestockErrors.MessageErrors.MessageNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        if (m.SenderUserId != user.UserId)
        {
            AddError(LivestockErrors.Common.Unauthorized);
            await SendErrorsAsync(403, ct);
            return;
        }

        m.Content = req.Content;
        m.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        await SendAsync(new MessageDetail(m.Id, m.ConversationId, m.SenderUserId, m.RecipientUserId, m.Content, m.IsRead, m.ReadAt, m.AttachmentUrl, m.AttachmentType, m.CreatedAt, m.UpdatedAt), 200, ct);
    }
}

public class DeleteMessageEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<DeleteMessageRequest, EmptyResponse>
{
    public override void Configure()
    {
        Delete("/Messages/{Id}");
        Tags("Messages");
    }

    public override async Task HandleAsync(DeleteMessageRequest req, CancellationToken ct)
    {
        var m = await db.Messages.FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (m is null)
        {
            AddError(LivestockErrors.MessageErrors.MessageNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        if (m.SenderUserId != user.UserId)
        {
            AddError(LivestockErrors.Common.Unauthorized);
            await SendErrorsAsync(403, ct);
            return;
        }

        m.IsDeleted = true;
        m.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}

public class MarkMessageReadEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<MarkMessageReadRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/Messages/{Id}/Read");
        Tags("Messages");
    }

    public override async Task HandleAsync(MarkMessageReadRequest req, CancellationToken ct)
    {
        var m = await db.Messages.FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (m is null)
        {
            AddError(LivestockErrors.MessageErrors.MessageNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        if (m.RecipientUserId != user.UserId)
        {
            AddError(LivestockErrors.Common.Unauthorized);
            await SendErrorsAsync(403, ct);
            return;
        }

        m.IsRead = true;
        m.ReadAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}

public class GetAllMessagesEndpoint(LivestockDbContext db) : EndpointWithoutRequest<List<MessageListItem>>
{
    public override void Configure()
    {
        Get("/Messages");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Messages");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var messages = await db.Messages
            .AsNoTracking()
            .Where(m => !m.IsDeleted)
            .OrderByDescending(m => m.CreatedAt)
            .Take(100)
            .Select(m => new MessageListItem(m.Id, m.ConversationId, m.SenderUserId, m.RecipientUserId, m.Content, m.IsRead, m.ReadAt, m.AttachmentUrl, m.AttachmentType, m.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(messages, 200, ct);
    }
}

public class PickMessagesEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<MessagePickRequest, List<MessagePickItem>>
{
    public override void Configure()
    {
        Post("/Messages/Pick");
        Tags("Messages");
    }

    public override async Task HandleAsync(MessagePickRequest req, CancellationToken ct)
    {
        var query = db.Messages
            .AsNoTracking()
            .Where(m => !m.IsDeleted && (m.SenderUserId == user.UserId || m.RecipientUserId == user.UserId));

        if (req.ConversationId.HasValue)
        {
            query = query.Where(m => m.ConversationId == req.ConversationId.Value);
        }

        if (!string.IsNullOrWhiteSpace(req.Keyword))
        {
            query = query.Where(m => m.Content.Contains(req.Keyword));
        }

        var messages = await query
            .OrderByDescending(m => m.CreatedAt)
            .Take(req.Limit > 0 ? req.Limit : 10)
            .Select(m => new MessagePickItem(m.Id, m.Content, m.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(messages, 200, ct);
    }
}
