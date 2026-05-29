using LivestockTrading.Identity.Application.Abstractions;
using LivestockTrading.Identity.Application.Common.Mappers;
using LivestockTrading.Identity.Domain.Enums;
using MassTransit;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Auth;

public sealed class RefreshTokenHandler : IConsumer<RefreshTokenCommand>
{
    private readonly IUserRepository _users;
    private readonly IRefreshTokenGenerator _rtGen;
    private readonly IJwtTokenService _jwt;
    private readonly TimeProvider _clock;

    public RefreshTokenHandler(
        IUserRepository users,
        IRefreshTokenGenerator rtGen,
        IJwtTokenService jwt,
        TimeProvider clock)
    {
        _users = users;
        _rtGen = rtGen;
        _jwt = jwt;
        _clock = clock;
    }

    public async Task Consume(ConsumeContext<RefreshTokenCommand> context)
    {
        var dto = context.Message.Dto;
        var ct = context.CancellationToken;
        var now = _clock.GetUtcNow();

        try
        {
            var tokenHash = _rtGen.Hash(dto.RefreshToken);

            var user = await _users.GetByRefreshTokenHashAsync(tokenHash, ct);
            if (user is null)
            {
                await Fail(context, "INVALID_CREDENTIALS", "Invalid credentials.");
                return;
            }

            var token = user.RefreshTokens.FirstOrDefault(t => t.TokenHash == tokenHash);
            if (token is null)
            {
                await Fail(context, "INVALID_CREDENTIALS", "Invalid credentials.");
                return;
            }

            if (token.RevokedAt is not null)
            {
                user.RevokeFamilyRefreshTokens(token.FamilyId, RevocationReason.Reuse, now);
                await Fail(context, "INVALID_CREDENTIALS", "Invalid credentials.");
                return;
            }

            if (!token.IsActive(now))
            {
                await Fail(context, "INVALID_CREDENTIALS", "Invalid credentials.");
                return;
            }

            var pair = _rtGen.Generate();
            var ttl = dto.RememberMe ? TimeSpan.FromDays(90) : TimeSpan.FromDays(30);
            user.RotateRefreshToken(token.Id, pair.Hash, ttl, now);

            var access = await _jwt.IssueAsync(user, token.DeviceId, ct);
            var summary = user.ToSummary();

            await context.RespondAsync<Result<LoginResponse>>(Result.Success(
                new LoginResponse(access.Value, access.ExpiresAt, pair.Raw, now.Add(ttl), summary)));
        }
        catch (DomainException ex)
        {
            await context.RespondAsync<Result<LoginResponse>>(
                Result.Failure<LoginResponse>(new Error("USER_RULE_VIOLATION", ex.Message)));
        }
    }

    private static async Task Fail(ConsumeContext<RefreshTokenCommand> context, string code, string message)
    {
        await context.RespondAsync<Result<LoginResponse>>(
            Result.Failure<LoginResponse>(new Error(code, message)));
    }
}
