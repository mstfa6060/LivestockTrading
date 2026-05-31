using LivestockTrading.Identity.Application.Abstractions;
using LivestockTrading.Identity.Domain.Enums;
using MassTransit;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.AdminUsers;

public sealed class ForceLogoutUserHandler : IConsumer<ForceLogoutUserCommand>
{
    private readonly IUserRepository _users;
    private readonly TimeProvider _clock;

    public ForceLogoutUserHandler(IUserRepository users, TimeProvider clock)
    {
        _users = users;
        _clock = clock;
    }

    public async Task Consume(ConsumeContext<ForceLogoutUserCommand> context)
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

            // No dedicated User.ForceLogout in Domain; handler-cascade pattern
            // (D2-out.1 ResetPasswordHandler emsali). Domain RevokeAllRefreshTokens
            // raises RefreshTokenRevoked per active token (User.cs:560-572).
            user.RevokeAllRefreshTokens(RevocationReason.AdminRevoked, now);

            await context.RespondAsync(Result.Success());
        }
        catch (DomainException ex)
        {
            await context.RespondAsync(Result.Failure(new Error("USER_RULE_VIOLATION", ex.Message)));
        }
    }
}
