using LivestockTrading.Identity.Application.Abstractions;
using MassTransit;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Me;

public sealed class CancelEmailChangeHandler : IConsumer<CancelEmailChangeCommand>
{
    private readonly IUserRepository _users;
    private readonly TimeProvider _clock;

    public CancelEmailChangeHandler(IUserRepository users, TimeProvider clock)
    {
        _users = users;
        _clock = clock;
    }

    public async Task Consume(ConsumeContext<CancelEmailChangeCommand> context)
    {
        var msg = context.Message;
        var ct = context.CancellationToken;

        var user = await _users.GetByIdAsync(msg.UserId, ct);
        if (user is null)
        {
            await context.RespondAsync(Result.Failure(
                new Error("NOT_FOUND_USER", $"User not found: {msg.UserId}")));
            return;
        }

        try
        {
            var now = _clock.GetUtcNow();
            // Domain: Pending* field'larini unconditionally null'lar — idempotent no-op.
            user.CancelPendingEmailChange(now);
            await context.RespondAsync(Result.Success());
        }
        catch (DomainException ex)
        {
            await context.RespondAsync(Result.Failure(new Error("USER_RULE_VIOLATION", ex.Message)));
        }
    }
}
