using LivestockTrading.Identity.Application.Abstractions;
using LivestockTrading.Identity.Domain.Aggregates;
using LivestockTrading.Identity.Domain.Enums;
using LivestockTrading.Identity.Domain.ValueObjects;
using MassTransit;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Auth;

public sealed class SendPhoneOtpHandler : IConsumer<SendPhoneOtpCommand>
{
    private readonly IUserRepository _users;
    private readonly IPhoneVerificationTicketRepository _phoneTickets;
    private readonly IPhoneOtpCodeGenerator _phoneOtpGen;
    private readonly ISmsSender _smsSender;
    private readonly TimeProvider _clock;

    public SendPhoneOtpHandler(
        IUserRepository users,
        IPhoneVerificationTicketRepository phoneTickets,
        IPhoneOtpCodeGenerator phoneOtpGen,
        ISmsSender smsSender,
        TimeProvider clock)
    {
        _users = users;
        _phoneTickets = phoneTickets;
        _phoneOtpGen = phoneOtpGen;
        _smsSender = smsSender;
        _clock = clock;
    }

    public async Task Consume(ConsumeContext<SendPhoneOtpCommand> context)
    {
        var msg = context.Message;
        var ct = context.CancellationToken;
        var now = _clock.GetUtcNow();

        try
        {
            PhoneNumber phone;
            try
            {
                phone = new PhoneNumber(msg.Dto.Phone);
            }
            catch (DomainException)
            {
                // Gecersiz format: enumeration-safe (var-olmayan numara gibi davran).
                await context.RespondAsync(Result.Success());
                return;
            }

            var user = await _users.GetByPhoneAsync(phone, ct);

            var pair = _phoneOtpGen.Generate();
            var ttl = TimeSpan.FromMinutes(5); // plan-doc 05-identity §3 satir 289: 5dk TTL.
            var ticket = PhoneVerificationTicket.Issue(
                phone, PhonePurpose.Register, pair.Hash, now, ttl, msg.IpAddress, user?.Id);
            // userId = user?.Id (varsa bagla, yoksa null = register-flow, spec §3:285).

            await _phoneTickets.AddAsync(ticket, ct);
            await _smsSender.SendOtpAsync(phone, pair.Raw, ct);

            // Enumeration-safe: user var/yok ayni cevap (ForgotPasswordHandler emsali).
            await context.RespondAsync(Result.Success());
        }
        catch (DomainException ex)
        {
            await context.RespondAsync(Result.Failure(new Error("USER_RULE_VIOLATION", ex.Message)));
        }
    }
}
