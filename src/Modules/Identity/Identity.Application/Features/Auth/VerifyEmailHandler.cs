using LivestockTrading.Identity.Application.Abstractions;
using LivestockTrading.Identity.Domain.ValueObjects;
using MassTransit;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Auth;

public sealed class VerifyEmailHandler : IConsumer<VerifyEmailCommand>
{
    private readonly IUserRepository _users;
    private readonly IEmailVerificationTicketRepository _tickets;
    private readonly IEmailVerificationTokenGenerator _tokenGen;
    private readonly TimeProvider _clock;

    public VerifyEmailHandler(
        IUserRepository users,
        IEmailVerificationTicketRepository tickets,
        IEmailVerificationTokenGenerator tokenGen,
        TimeProvider clock)
    {
        _users = users;
        _tickets = tickets;
        _tokenGen = tokenGen;
        _clock = clock;
    }

    public async Task Consume(ConsumeContext<VerifyEmailCommand> context)
    {
        var ct = context.CancellationToken;
        var now = _clock.GetUtcNow();

        try
        {
            var email = new EmailAddress(context.Message.Dto.Email);

            var ticket = await _tickets.GetActiveByEmailAsync(email, ct);
            if (ticket is null)
            {
                await context.RespondAsync(Result.Failure(
                    new Error("INVALID_VERIFICATION_TOKEN", "Invalid verification token.")));
                return;
            }

            var providedHash = _tokenGen.Hash(context.Message.Dto.Token);
            if (!ticket.TryConsume(providedHash, now))
            {
                await context.RespondAsync(Result.Failure(
                    new Error("INVALID_VERIFICATION_TOKEN", "Invalid verification token.")));
                return;
            }

            var user = await _users.GetByEmailAsync(email, ct);
            if (user is null)
            {
                await context.RespondAsync(Result.Failure(
                    new Error("INVALID_VERIFICATION_TOKEN", "Invalid verification token.")));
                return;
            }

            user.VerifyEmail(now);

            await context.RespondAsync(Result.Success());
        }
        catch (DomainException ex)
        {
            await context.RespondAsync(Result.Failure(new Error("USER_RULE_VIOLATION", ex.Message)));
        }
    }
}
