using LivestockTrading.Identity.Application.Abstractions;
using LivestockTrading.Identity.Domain.Enums;
using MassTransit;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Me;

public sealed class RevokeSessionHandler : IConsumer<RevokeSessionCommand>
{
    private readonly IUserRepository _users;
    private readonly TimeProvider _clock;

    public RevokeSessionHandler(IUserRepository users, TimeProvider clock)
    {
        _users = users;
        _clock = clock;
    }

    public async Task Consume(ConsumeContext<RevokeSessionCommand> context)
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

            var token = user.RefreshTokens.FirstOrDefault(t => t.Id == context.Message.SessionId);
            if (token is null)
            {
                await context.RespondAsync(Result.Failure(
                    new Error("NOT_FOUND_SESSION", $"Session not found: {context.Message.SessionId}")));
                return;
            }

            user.RevokeRefreshToken(token.Id, RevocationReason.Logout, now);

            await context.RespondAsync(Result.Success());
        }
        catch (DomainException ex)
        {
            await context.RespondAsync(Result.Failure(new Error("USER_RULE_VIOLATION", ex.Message)));
        }
    }
}
