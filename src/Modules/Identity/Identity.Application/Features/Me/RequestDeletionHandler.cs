using LivestockTrading.Identity.Application.Abstractions;
using MassTransit;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Me;

public sealed class RequestDeletionHandler : IConsumer<RequestDeletionCommand>
{
    private readonly IUserRepository _users;
    private readonly TimeProvider _clock;

    public RequestDeletionHandler(IUserRepository users, TimeProvider clock)
    {
        _users = users;
        _clock = clock;
    }

    public async Task Consume(ConsumeContext<RequestDeletionCommand> context)
    {
        var ct = context.CancellationToken;
        var now = _clock.GetUtcNow();

        try
        {
            var user = await _users.GetByIdAsync(context.Message.UserId, ct);
            if (user is null)
            {
                await context.RespondAsync(Result.Failure(
                    new Error("NOT_FOUND_USER", $"User not found: {context.Message.UserId}")));
                return;
            }

            user.RequestDeletion(now);

            await context.RespondAsync(Result.Success());
        }
        catch (DomainException ex)
        {
            await context.RespondAsync(Result.Failure(new Error("USER_RULE_VIOLATION", ex.Message)));
        }
    }
}
