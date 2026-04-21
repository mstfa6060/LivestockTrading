using Iam.Domain.Entities;
using Iam.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NATS.Client.Core;
using Shared.Contracts.Events.Livestock;
using Shared.Infrastructure.Messaging;

namespace Iam.Features.Consumers;

/// <summary>
/// Assigns the Seller role to a user when they successfully register as a seller.
/// Reacts to SellerRegisteredEvent published by the Livestock module.
/// </summary>
public sealed class SellerRoleConsumer(
    INatsClient nats,
    IServiceScopeFactory scopeFactory,
    ILogger<SellerRoleConsumer> logger) : NatsConsumerBase<SellerRegisteredEvent>(nats)
{
    private static readonly Guid SellerRoleId = new("a1000000-0000-0000-0000-000000000003");
    private static readonly Guid LivestockModuleId = new("DFD018C9-FC32-42C4-AEFD-70A5942A295E");

    protected override string Subject => SellerRegisteredEvent.Subject;

    protected override async Task HandleAsync(SellerRegisteredEvent message, CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IamDbContext>();

        var alreadyHasRole = await db.UserRoles.AnyAsync(ur =>
            ur.UserId == message.UserId &&
            ur.RoleId == SellerRoleId &&
            ur.ModuleId == LivestockModuleId, ct);

        if (alreadyHasRole)
        {
            logger.LogDebug("User {UserId} already has Seller role, skipping assignment.", message.UserId);
            return;
        }

        db.UserRoles.Add(new UserRole
        {
            UserId = message.UserId,
            RoleId = SellerRoleId,
            ModuleId = LivestockModuleId,
        });

        await db.SaveChangesAsync(ct);

        logger.LogInformation("Seller role assigned to user {UserId} (seller {SellerId})", message.UserId, message.SellerId);
    }
}
