using Livestock.Domain.Entities;
using Livestock.Domain.Enums;
using Livestock.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NATS.Client.Core;
using Shared.Contracts.Events.Livestock;
using Shared.Infrastructure.Messaging;

namespace Livestock.Features.Consumers;

/// <summary>
/// Creates in-app notifications for admin users when a new product is submitted for approval.
/// Admin user IDs are read from configuration: AdminNotification:UserIds (list of Guids).
/// </summary>
public sealed class ProductPendingAdminNotificationConsumer(
    INatsClient nats,
    IServiceScopeFactory scopeFactory,
    IConfiguration configuration,
    ILogger<ProductPendingAdminNotificationConsumer> logger) : NatsConsumerBase<ProductCreatedEvent>(nats)
{
    protected override string Subject => ProductCreatedEvent.Subject;

    protected override async Task HandleAsync(ProductCreatedEvent message, CancellationToken ct)
    {
        var adminUserIds = configuration.GetSection("AdminNotification:UserIds").Get<List<Guid>>() ?? [];

        if (adminUserIds.Count == 0)
        {
            logger.LogDebug("AdminNotification:UserIds not configured; skipping admin notification for product {ProductId}.", message.ProductId);
            return;
        }

        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LivestockDbContext>();

        var notifications = adminUserIds.Select(adminId => new Notification
        {
            RecipientUserId = adminId,
            Type = NotificationType.ProductPendingApproval,
            Title = "New Product Pending Approval",
            Body = $"\"{message.Title}\" is awaiting review.",
            RelatedEntityId = message.ProductId,
            RelatedEntityType = "Product"
        }).ToList();

        db.Notifications.AddRange(notifications);
        await db.SaveChangesAsync(ct);

        logger.LogInformation("Admin notification sent for product {ProductId} to {Count} admin(s).", message.ProductId, notifications.Count);
    }
}

/// <summary>
/// Creates in-app notifications for admin users when a new seller registers and awaits verification.
/// </summary>
public sealed class SellerPendingAdminNotificationConsumer(
    INatsClient nats,
    IServiceScopeFactory scopeFactory,
    IConfiguration configuration,
    ILogger<SellerPendingAdminNotificationConsumer> logger) : NatsConsumerBase<SellerRegisteredEvent>(nats)
{
    protected override string Subject => SellerRegisteredEvent.Subject;

    protected override async Task HandleAsync(SellerRegisteredEvent message, CancellationToken ct)
    {
        var adminUserIds = configuration.GetSection("AdminNotification:UserIds").Get<List<Guid>>() ?? [];

        if (adminUserIds.Count == 0)
        {
            logger.LogDebug("AdminNotification:UserIds not configured; skipping admin notification for seller {SellerId}.", message.SellerId);
            return;
        }

        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LivestockDbContext>();

        var notifications = adminUserIds.Select(adminId => new Notification
        {
            RecipientUserId = adminId,
            Type = NotificationType.SellerPendingVerification,
            Title = "New Seller Awaiting Verification",
            Body = $"\"{message.BusinessName}\" has registered and is awaiting verification.",
            RelatedEntityId = message.SellerId,
            RelatedEntityType = "Seller"
        }).ToList();

        db.Notifications.AddRange(notifications);
        await db.SaveChangesAsync(ct);

        logger.LogInformation("Admin notification sent for seller {SellerId} to {Count} admin(s).", message.SellerId, notifications.Count);
    }
}
