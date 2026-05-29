using LivestockTrading.Identity.Application.Abstractions;
using LivestockTrading.Identity.Domain.Aggregates;
using LivestockTrading.Identity.Domain.ValueObjects;
using MassTransit;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Auth;

public sealed class SendEmailVerifyHandler : IConsumer<SendEmailVerifyCommand>
{
    private readonly IUserRepository _users;
    private readonly IEmailVerificationTicketRepository _tickets;
    private readonly IEmailVerificationTokenGenerator _tokenGen;
    private readonly IEmailSender _email;
    private readonly TimeProvider _clock;

    public SendEmailVerifyHandler(
        IUserRepository users,
        IEmailVerificationTicketRepository tickets,
        IEmailVerificationTokenGenerator tokenGen,
        IEmailSender email,
        TimeProvider clock)
    {
        _users = users;
        _tickets = tickets;
        _tokenGen = tokenGen;
        _email = email;
        _clock = clock;
    }

    public async Task Consume(ConsumeContext<SendEmailVerifyCommand> context)
    {
        var ct = context.CancellationToken;
        var now = _clock.GetUtcNow();

        try
        {
            var email = new EmailAddress(context.Message.Dto.Email);
            var user = await _users.GetByEmailAsync(email, ct);

            // Email-enumeration koruma: bilinmeyen email veya zaten verified
            // kullanici icin sessizce success don. Hicbir ticket yazilmaz,
            // hicbir mail gonderilmez.
            if (user is null || user.EmailVerifiedAt is not null)
            {
                await context.RespondAsync(Result.Success());
                return;
            }

            var pair = _tokenGen.Generate();
            var ttl = TimeSpan.FromHours(24);
            var ticket = EmailVerificationTicket.Issue(
                email, pair.Hash, now, ttl, context.Message.IpAddress, user.Id);

            await _tickets.AddAsync(ticket, ct);
            await _email.SendEmailVerificationAsync(email, pair.Raw, ct);

            await context.RespondAsync(Result.Success());
        }
        catch (DomainException ex)
        {
            await context.RespondAsync(Result.Failure(new Error("USER_RULE_VIOLATION", ex.Message)));
        }
    }
}
