using LivestockTrading.Identity.Application.Abstractions;
using LivestockTrading.Identity.Domain.Aggregates;
using LivestockTrading.Identity.Domain.Enums;
using LivestockTrading.Identity.Domain.ValueObjects;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Auth;

public sealed class ResetPasswordHandler : IConsumer<ResetPasswordCommand>
{
    // Generic mesaj — token-validity ile identifier-existence sizdirmaz.
    private static readonly Error InvalidOrExpired =
        new("INVALID_OR_EXPIRED", "Reset credential is invalid or expired.");

    private readonly IUserRepository _users;
    private readonly IEmailVerificationTicketRepository _emailTickets;
    private readonly IPhoneVerificationTicketRepository _phoneTickets;
    private readonly IEmailVerificationTokenGenerator _emailTokenGen;
    private readonly IPhoneOtpCodeGenerator _phoneOtpGen;
    private readonly IPasswordHasher<User> _hasher;
    private readonly TimeProvider _clock;

    public ResetPasswordHandler(
        IUserRepository users,
        IEmailVerificationTicketRepository emailTickets,
        IPhoneVerificationTicketRepository phoneTickets,
        IEmailVerificationTokenGenerator emailTokenGen,
        IPhoneOtpCodeGenerator phoneOtpGen,
        IPasswordHasher<User> hasher,
        TimeProvider clock)
    {
        _users = users;
        _emailTickets = emailTickets;
        _phoneTickets = phoneTickets;
        _emailTokenGen = emailTokenGen;
        _phoneOtpGen = phoneOtpGen;
        _hasher = hasher;
        _clock = clock;
    }

    public async Task Consume(ConsumeContext<ResetPasswordCommand> context)
    {
        var msg = context.Message;
        var ct = context.CancellationToken;
        var now = _clock.GetUtcNow();

        try
        {
            User? user = msg.Dto.Method switch
            {
                "email" => await ResolveEmailAsync(msg.Dto, now, ct),
                "phone" => await ResolvePhoneAsync(msg.Dto, now, ct),
                _ => null, // validator yakalar; defansif
            };

            if (user is null)
            {
                await context.RespondAsync(Result.Failure(InvalidOrExpired));
                return;
            }

            var hashed = HashedPassword.Create<User>(msg.Dto.NewPassword, _hasher);
            user.ChangePassword(hashed, msg.IpAddress, now);

            // Guvenlik: reset sonrasi tum aktif refresh-token revoke (ChangePasswordHandler.cs:65 cascade emsali).
            user.RevokeAllRefreshTokens(RevocationReason.PasswordChanged, now);

            await context.RespondAsync(Result.Success());
        }
        catch (DomainException ex)
        {
            await context.RespondAsync(Result.Failure(new Error("USER_RULE_VIOLATION", ex.Message)));
        }
    }

    private async Task<User?> ResolveEmailAsync(ResetPasswordRequest dto, DateTimeOffset now, CancellationToken ct)
    {
        EmailAddress email;
        try
        {
            email = new EmailAddress(dto.Identifier);
        }
        catch (DomainException)
        {
            return null;
        }

        var user = await _users.GetByEmailAsync(email, ct);
        if (user is null)
            return null;

        var ticket = await _emailTickets.GetActiveByEmailAsync(email, EmailPurpose.ResetPassword, ct);
        if (ticket is null)
            return null;

        var providedHash = _emailTokenGen.Hash(dto.Credential);
        return ticket.TryConsume(providedHash, now) ? user : null;
    }

    private async Task<User?> ResolvePhoneAsync(ResetPasswordRequest dto, DateTimeOffset now, CancellationToken ct)
    {
        PhoneNumber phone;
        try
        {
            phone = new PhoneNumber(dto.Identifier);
        }
        catch (DomainException)
        {
            return null;
        }

        var user = await _users.GetByPhoneAsync(phone, ct);
        if (user is null)
            return null;

        var ticket = await _phoneTickets.GetActiveByPhoneAsync(phone, PhonePurpose.ResetPassword, ct);
        if (ticket is null)
            return null;

        var providedHash = _phoneOtpGen.Hash(dto.Credential);
        return ticket.TryConsume(providedHash, now) ? user : null;
    }
}
