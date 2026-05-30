using LivestockTrading.Identity.Application.Abstractions;
using LivestockTrading.Identity.Domain.ValueObjects;
using MassTransit;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Me;

public sealed class ConfirmEmailChangeHandler : IConsumer<ConfirmEmailChangeCommand>
{
    private readonly IUserRepository _users;
    private readonly IEmailVerificationTokenGenerator _tokenGen;
    private readonly TimeProvider _clock;

    public ConfirmEmailChangeHandler(
        IUserRepository users,
        IEmailVerificationTokenGenerator tokenGen,
        TimeProvider clock)
    {
        _users = users;
        _tokenGen = tokenGen;
        _clock = clock;
    }

    public async Task Consume(ConsumeContext<ConfirmEmailChangeCommand> context)
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

        EmailAddress confirmedEmail;
        try
        {
            confirmedEmail = new EmailAddress(msg.Dto.Email);
        }
        catch (DomainException ex)
        {
            await context.RespondAsync(Result.Failure(new Error("USER_RULE_VIOLATION", ex.Message)));
            return;
        }

        try
        {
            var now = _clock.GetUtcNow();
            // Hash karsilastirmasi Domain'de (FixedTimeEquals). Handler sadece byte[] hazirlar.
            var providedHash = _tokenGen.Hash(msg.Dto.Token);
            user.ConfirmEmailChange(confirmedEmail, providedHash, now);
            // Domain: 4 precondition (pending-yok / expired / email-mismatch / token-mismatch)
            // + UserEmailVerified event Raise.
            await context.RespondAsync(Result.Success());
        }
        catch (DomainException ex)
        {
            await context.RespondAsync(Result.Failure(new Error("USER_RULE_VIOLATION", ex.Message)));
        }
    }
}
