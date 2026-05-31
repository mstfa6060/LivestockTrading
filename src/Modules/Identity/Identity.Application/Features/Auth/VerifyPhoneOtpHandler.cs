using LivestockTrading.Identity.Application.Abstractions;
using LivestockTrading.Identity.Domain.Enums;
using LivestockTrading.Identity.Domain.ValueObjects;
using MassTransit;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Auth;

public sealed class VerifyPhoneOtpHandler : IConsumer<VerifyPhoneOtpCommand>
{
    // Generic mesaj — ticket-validity ile phone-existence sizdirmaz (Reset emsali).
    private static readonly Error InvalidOrExpired =
        new("INVALID_OR_EXPIRED", "Verification code is invalid or expired.");

    private readonly IUserRepository _users;
    private readonly IPhoneVerificationTicketRepository _phoneTickets;
    private readonly IPhoneOtpCodeGenerator _phoneOtpGen;
    private readonly TimeProvider _clock;

    public VerifyPhoneOtpHandler(
        IUserRepository users,
        IPhoneVerificationTicketRepository phoneTickets,
        IPhoneOtpCodeGenerator phoneOtpGen,
        TimeProvider clock)
    {
        _users = users;
        _phoneTickets = phoneTickets;
        _phoneOtpGen = phoneOtpGen;
        _clock = clock;
    }

    public async Task Consume(ConsumeContext<VerifyPhoneOtpCommand> context)
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
                await context.RespondAsync(Result.Failure(InvalidOrExpired));
                return;
            }

            var ticket = await _phoneTickets.GetActiveByPhoneAsync(phone, PhonePurpose.Register, ct);
            if (ticket is null)
            {
                await context.RespondAsync(Result.Failure(InvalidOrExpired));
                return;
            }

            var providedHash = _phoneOtpGen.Hash(msg.Dto.Code);
            if (!ticket.TryConsume(providedHash, now))
            {
                await context.RespondAsync(Result.Failure(InvalidOrExpired));
                return;
            }

            // Ticket consumed (numara-sahipligi kanitlandi). User VAR ise PhoneVerifiedAt
            // set edilir; user-null = anonim pre-register verify, ticket consume yeterli
            // (register sonrasi ayri verify cagrisi veya register handler ticket'i goz onunde
            //  bulundurabilir; bu D2-out scope disi gelecek akis).
            var user = await _users.GetByPhoneAsync(phone, ct);
            user?.VerifyPhone(now);

            await context.RespondAsync(Result.Success());
        }
        catch (DomainException ex)
        {
            await context.RespondAsync(Result.Failure(new Error("USER_RULE_VIOLATION", ex.Message)));
        }
    }
}
