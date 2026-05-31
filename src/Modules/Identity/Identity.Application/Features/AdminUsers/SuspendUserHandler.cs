using LivestockTrading.Identity.Application.Abstractions;
using MassTransit;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.AdminUsers;

public sealed class SuspendUserHandler : IConsumer<SuspendUserCommand>
{
    private readonly IUserRepository _users;
    private readonly TimeProvider _clock;

    public SuspendUserHandler(IUserRepository users, TimeProvider clock)
    {
        _users = users;
        _clock = clock;
    }

    public async Task Consume(ConsumeContext<SuspendUserCommand> context)
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

            // Domain cascade: refresh-token revoke + UserSuspended event raise (User.cs:254).
            user.Suspend(msg.Reason, msg.ActorAdminId, now);

            await context.RespondAsync(Result.Success());
        }
        catch (DomainException ex)
        {
            await context.RespondAsync(Result.Failure(new Error("USER_RULE_VIOLATION", ex.Message)));
        }
    }
}
