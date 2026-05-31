using LivestockTrading.Identity.Application.Abstractions;
using MassTransit;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.AdminUsers;

public sealed class GrantUserRoleHandler : IConsumer<GrantUserRoleCommand>
{
    private readonly IUserRepository _users;
    private readonly TimeProvider _clock;

    public GrantUserRoleHandler(IUserRepository users, TimeProvider clock)
    {
        _users = users;
        _clock = clock;
    }

    public async Task Consume(ConsumeContext<GrantUserRoleCommand> context)
    {
        var msg = context.Message;
        var ct = context.CancellationToken;
        var now = _clock.GetUtcNow();

        try
        {
            var user = await _users.GetByIdAsync(msg.UserId, ct);
            if (user is null)
            {
                await context.RespondAsync(Result.Failure(
                    new Error("NOT_FOUND_USER", $"User not found: {msg.UserId}")));
                return;
            }

            // Domain: idempotent (zaten-var role early-return) + UserRoleGranted event (User.cs:371).
            user.GrantRole(msg.Role, msg.ActorAdminId, now);

            await context.RespondAsync(Result.Success());
        }
        catch (DomainException ex)
        {
            await context.RespondAsync(Result.Failure(new Error("USER_RULE_VIOLATION", ex.Message)));
        }
    }
}
