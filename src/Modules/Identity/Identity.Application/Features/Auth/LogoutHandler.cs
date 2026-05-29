using LivestockTrading.Identity.Application.Abstractions;
using LivestockTrading.Identity.Domain.Enums;
using MassTransit;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Auth;

public sealed class LogoutHandler : IConsumer<LogoutCommand>
{
    private readonly IUserRepository _users;
    private readonly IRefreshTokenGenerator _rtGen;
    private readonly TimeProvider _clock;

    public LogoutHandler(
        IUserRepository users,
        IRefreshTokenGenerator rtGen,
        TimeProvider clock)
    {
        _users = users;
        _rtGen = rtGen;
        _clock = clock;
    }

    public async Task Consume(ConsumeContext<LogoutCommand> context)
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

            var tokenHash = _rtGen.Hash(context.Message.Dto.RefreshToken);
            var token = user.RefreshTokens.FirstOrDefault(t => t.TokenHash == tokenHash);
            if (token is not null)
                user.RevokeRefreshToken(token.Id, RevocationReason.Logout, now);

            // Access-token jti blacklist intentionally skipped here — the jti is carried
            // on the access token (JWT) which the host auth layer (W4.4) will surface.
            // Until then logout only revokes the refresh token; access tokens expire
            // naturally within 15 minutes.

            await context.RespondAsync(Result.Success());
        }
        catch (DomainException ex)
        {
            await context.RespondAsync(Result.Failure(
                new Error("USER_RULE_VIOLATION", ex.Message)));
        }
    }
}
