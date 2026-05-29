using LivestockTrading.Identity.Application.Abstractions;
using LivestockTrading.Identity.Domain.Aggregates;
using LivestockTrading.Identity.Domain.Enums;
using LivestockTrading.Identity.Domain.ValueObjects;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Me;

public sealed class ChangePasswordHandler : IConsumer<ChangePasswordCommand>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher<User> _hasher;
    private readonly TimeProvider _clock;

    public ChangePasswordHandler(
        IUserRepository users,
        IPasswordHasher<User> hasher,
        TimeProvider clock)
    {
        _users = users;
        _hasher = hasher;
        _clock = clock;
    }

    public async Task Consume(ConsumeContext<ChangePasswordCommand> context)
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

            // Social-only kullanici (Password null) password-change yapamaz.
            // INVALID_CREDENTIALS doneriz (current-password match edemeyecegi icin
            // semantik olarak ayni; CONFLICT_NO_PASSWORD social-login flag verir).
            if (user.Password is null)
            {
                await context.RespondAsync(Result.Failure(
                    new Error("INVALID_CREDENTIALS", "Invalid credentials.")));
                return;
            }

            if (!user.Password.Value.Verify<User>(context.Message.Dto.CurrentPassword, _hasher))
            {
                await context.RespondAsync(Result.Failure(
                    new Error("INVALID_CREDENTIALS", "Invalid credentials.")));
                return;
            }

            var newHashed = HashedPassword.Create<User>(context.Message.Dto.NewPassword, _hasher);
            user.ChangePassword(newHashed, context.Message.IpAddress, now);

            // Guvenlik: sifre degisince tum aktif refresh-token revoke (oturum guvenligi).
            // Domain ChangePassword bunu cascade etmiyor, handler'da inline.
            user.RevokeAllRefreshTokens(RevocationReason.PasswordChanged, now);

            await context.RespondAsync(Result.Success());
        }
        catch (DomainException ex)
        {
            await context.RespondAsync(Result.Failure(new Error("USER_RULE_VIOLATION", ex.Message)));
        }
    }
}
