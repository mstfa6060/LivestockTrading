using LivestockTrading.Identity.Application.Abstractions;
using LivestockTrading.Identity.Domain.Aggregates;
using LivestockTrading.Identity.Domain.Enums;
using LivestockTrading.Identity.Domain.ValueObjects;
using MassTransit;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Auth;

public sealed class ForgotPasswordHandler : IConsumer<ForgotPasswordCommand>
{
    private readonly IUserRepository _users;
    private readonly IEmailVerificationTicketRepository _emailTickets;
    private readonly IPhoneVerificationTicketRepository _phoneTickets;
    private readonly IEmailVerificationTokenGenerator _emailTokenGen;
    private readonly IPhoneOtpCodeGenerator _phoneOtpGen;
    private readonly IEmailSender _emailSender;
    private readonly ISmsSender _smsSender;
    private readonly TimeProvider _clock;

    public ForgotPasswordHandler(
        IUserRepository users,
        IEmailVerificationTicketRepository emailTickets,
        IPhoneVerificationTicketRepository phoneTickets,
        IEmailVerificationTokenGenerator emailTokenGen,
        IPhoneOtpCodeGenerator phoneOtpGen,
        IEmailSender emailSender,
        ISmsSender smsSender,
        TimeProvider clock)
    {
        _users = users;
        _emailTickets = emailTickets;
        _phoneTickets = phoneTickets;
        _emailTokenGen = emailTokenGen;
        _phoneOtpGen = phoneOtpGen;
        _emailSender = emailSender;
        _smsSender = smsSender;
        _clock = clock;
    }

    public async Task Consume(ConsumeContext<ForgotPasswordCommand> context)
    {
        var msg = context.Message;
        var ct = context.CancellationToken;
        var now = _clock.GetUtcNow();

        try
        {
            switch (msg.Dto.Method)
            {
                case "email":
                    await HandleEmailAsync(msg.Dto.Identifier, msg.IpAddress, now, ct);
                    break;
                case "phone":
                    await HandlePhoneAsync(msg.Dto.Identifier, msg.IpAddress, now, ct);
                    break;
                // Validator method'u "email" | "phone"e kisitliyor; default kolu defansif.
            }

            // Enumeration-safe: user var/yok ayni cevap. SendEmailVerifyHandler.cs:42-49 emsali.
            await context.RespondAsync(Result.Success());
        }
        catch (DomainException ex)
        {
            await context.RespondAsync(Result.Failure(new Error("USER_RULE_VIOLATION", ex.Message)));
        }
    }

    private async Task HandleEmailAsync(string identifier, string ip, DateTimeOffset now, CancellationToken ct)
    {
        EmailAddress email;
        try
        {
            email = new EmailAddress(identifier);
        }
        catch (DomainException)
        {
            // Format gecersiz: enumeration-safe (var-olmayan email gibi davran).
            return;
        }

        var user = await _users.GetByEmailAsync(email, ct);
        if (user is null)
            return;

        var pair = _emailTokenGen.Generate();
        var ttl = TimeSpan.FromHours(24);
        var ticket = EmailVerificationTicket.Issue(
            email, EmailPurpose.ResetPassword, pair.Hash, now, ttl, ip, user.Id);

        await _emailTickets.AddAsync(ticket, ct);
        await _emailSender.SendPasswordResetAsync(email, pair.Raw, ct);
    }

    private async Task HandlePhoneAsync(string identifier, string ip, DateTimeOffset now, CancellationToken ct)
    {
        PhoneNumber phone;
        try
        {
            phone = new PhoneNumber(identifier);
        }
        catch (DomainException)
        {
            return;
        }

        var user = await _users.GetByPhoneAsync(phone, ct);
        if (user is null)
            return;

        var pair = _phoneOtpGen.Generate();
        var ttl = TimeSpan.FromMinutes(5); // plan-doc 05-identity §3 satir 289: PhoneVerificationTicket 5dk TTL.
        var ticket = PhoneVerificationTicket.Issue(
            phone, PhonePurpose.ResetPassword, pair.Hash, now, ttl, ip, user.Id);

        await _phoneTickets.AddAsync(ticket, ct);
        await _smsSender.SendOtpAsync(phone, pair.Raw, ct);
    }
}
