using Iam.Domain.Entities;
using Iam.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NATS.Client.Core;
using Shared.Contracts.Events.Livestock;
using Shared.Infrastructure.Messaging;

namespace Iam.Features.Consumers;

public class TransporterRoleConsumer(INatsClient nats, IServiceScopeFactory scopeFactory)
    : NatsConsumerBase<TransporterRegisteredEvent>(nats)
{
    private static readonly Guid TransporterRoleId = new("a1000000-0000-0000-0000-000000000004");
    private static readonly Guid LivestockModuleId = new("DFD018C9-FC32-42C4-AEFD-70A5942A295E");

    protected override string Subject => TransporterRegisteredEvent.Subject;

    protected override async Task HandleAsync(TransporterRegisteredEvent message, CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IamDbContext>();

        var exists = await db.UserRoles.AnyAsync(r =>
            r.UserId == message.UserId &&
            r.RoleId == TransporterRoleId &&
            r.ModuleId == LivestockModuleId, ct);

        if (!exists)
        {
            db.UserRoles.Add(new UserRole
            {
                UserId = message.UserId,
                RoleId = TransporterRoleId,
                ModuleId = LivestockModuleId
            });
            await db.SaveChangesAsync(ct);
        }
    }
}
