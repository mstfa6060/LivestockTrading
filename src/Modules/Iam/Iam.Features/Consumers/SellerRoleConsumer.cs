using Iam.Domain.Entities;
using Iam.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NATS.Client.Core;
using Shared.Contracts.Events.Livestock;
using Shared.Infrastructure.Messaging;

namespace Iam.Features.Consumers;

public class SellerRoleConsumer(INatsClient nats, IServiceScopeFactory scopeFactory)
    : NatsConsumerBase<SellerRegisteredEvent>(nats)
{
    private static readonly Guid SellerRoleId = new("a1000000-0000-0000-0000-000000000003");
    private static readonly Guid LivestockModuleId = new("DFD018C9-FC32-42C4-AEFD-70A5942A295E");

    protected override string Subject => SellerRegisteredEvent.Subject;

    protected override async Task HandleAsync(SellerRegisteredEvent message, CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IamDbContext>();

        var exists = await db.UserRoles.AnyAsync(r =>
            r.UserId == message.UserId &&
            r.RoleId == SellerRoleId &&
            r.ModuleId == LivestockModuleId, ct);

        if (!exists)
        {
            db.UserRoles.Add(new UserRole
            {
                UserId = message.UserId,
                RoleId = SellerRoleId,
                ModuleId = LivestockModuleId
            });
            await db.SaveChangesAsync(ct);
        }
    }
}
