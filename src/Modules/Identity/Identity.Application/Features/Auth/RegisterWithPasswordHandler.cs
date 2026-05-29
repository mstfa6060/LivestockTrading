using LivestockTrading.Identity.Application.Abstractions;
using LivestockTrading.Identity.Domain.Aggregates;
using LivestockTrading.Identity.Domain.ValueObjects;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Auth;

public sealed class RegisterWithPasswordHandler : IConsumer<RegisterWithPasswordCommand>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher<User> _hasher;
    private readonly TimeProvider _clock;

    public RegisterWithPasswordHandler(
        IUserRepository users,
        IPasswordHasher<User> hasher,
        TimeProvider clock)
    {
        _users = users;
        _hasher = hasher;
        _clock = clock;
    }

    public async Task Consume(ConsumeContext<RegisterWithPasswordCommand> context)
    {
        var dto = context.Message.Dto;
        var ct = context.CancellationToken;
        var now = _clock.GetUtcNow();

        try
        {
            var email = new EmailAddress(dto.Email);
            PhoneNumber? phone = dto.Phone is null ? null : new PhoneNumber(dto.Phone);
            NationalId? nationalId = dto.NationalId is null ? null : new NationalId(dto.NationalId);

            if (await _users.ExistsByEmailAsync(email, ct))
            {
                await Fail(context, "CONFLICT_EMAIL", $"Email already registered: {dto.Email}");
                return;
            }

            if (phone is not null && await _users.ExistsByPhoneAsync(phone.Value, ct))
            {
                await Fail(context, "CONFLICT_PHONE", "Phone already registered.");
                return;
            }

            if (nationalId is not null && await _users.ExistsByNationalIdAsync(nationalId.Value, ct))
            {
                await Fail(context, "CONFLICT_NATIONAL_ID", "NationalId already registered.");
                return;
            }

            var hashed = HashedPassword.Create<User>(dto.Password, _hasher);
            var name = new PersonName(dto.FirstName, dto.LastName);
            var consents = dto.Consents
                .Select(c => new ConsentGrant(c.Type, c.Version, c.Granted))
                .ToList();

            var user = User.RegisterWithPassword(
                email,
                hashed,
                name,
                phone,
                nationalId,
                dto.AccountType,
                dto.Preferences,
                consents,
                context.Message.IpAddress,
                context.Message.UserAgent,
                now);

            await _users.AddAsync(user, ct);

            // Email verification token is intentionally NOT issued here. Plan-doc
            // §12 exposes a dedicated POST /identity/auth/email/send-verify endpoint
            // (W4.2.C) that calls IEmailSender; register-time only creates the user
            // with Status = EmailUnverified. Decoupling matches Domain factory
            // (RegisterWithPassword issues no verify token).

            await context.RespondAsync<Result<Guid>>(Result.Success(user.Id));
        }
        catch (DomainException ex)
        {
            await context.RespondAsync<Result<Guid>>(
                Result.Failure<Guid>(new Error("USER_RULE_VIOLATION", ex.Message)));
        }
    }

    private static async Task Fail(ConsumeContext<RegisterWithPasswordCommand> context, string code, string message)
    {
        await context.RespondAsync<Result<Guid>>(
            Result.Failure<Guid>(new Error(code, message)));
    }
}
