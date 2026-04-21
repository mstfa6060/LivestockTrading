using FastEndpoints;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;

namespace Livestock.Features.Notifications;

public class GetMyNotificationsEndpoint(LivestockDbContext db, IUserContext user) : EndpointWithoutRequest<List<NotificationItem>>
{
    public override void Configure()
    {
        Get("/Notifications");
        Tags("Notifications");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var notifications = await db.Notifications
            .AsNoTracking()
            .Where(n => n.RecipientUserId == user.UserId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(100)
            .Select(n => new NotificationItem(n.Id, n.Type, n.Title, n.Body, n.ImageUrl, n.DeepLink, n.IsRead, n.ReadAt, n.RelatedEntityId, n.RelatedEntityType, n.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(notifications, 200, ct);
    }
}

public class MarkNotificationReadEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<MarkNotificationReadRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/Notifications/{Id}/Read");
        Tags("Notifications");
    }

    public override async Task HandleAsync(MarkNotificationReadRequest req, CancellationToken ct)
    {
        var notification = await db.Notifications.FirstOrDefaultAsync(n => n.Id == req.Id, ct);
        if (notification is null)
        {
            AddError(LivestockErrors.NotificationErrors.NotificationNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        if (notification.RecipientUserId != user.UserId)
        {
            AddError(LivestockErrors.NotificationErrors.NotificationNotOwned);
            await SendErrorsAsync(403, ct);
            return;
        }

        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}

public class MarkAllNotificationsReadEndpoint(LivestockDbContext db, IUserContext user) : EndpointWithoutRequest<EmptyResponse>
{
    public override void Configure()
    {
        Post("/Notifications/ReadAll");
        Tags("Notifications");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await db.Notifications
            .Where(n => n.RecipientUserId == user.UserId && !n.IsRead)
            .ExecuteUpdateAsync(s => s
                .SetProperty(n => n.IsRead, true)
                .SetProperty(n => n.ReadAt, DateTime.UtcNow), ct);

        await SendNoContentAsync(ct);
    }
}
