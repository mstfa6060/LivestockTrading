using LivestockTrading.Identity.Application.Abstractions;
using LivestockTrading.Identity.Domain.ValueObjects;
using MassTransit;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Me;

public sealed class RequestEmailChangeHandler : IConsumer<RequestEmailChangeCommand>
{
    private readonly IUserRepository _users;
    private readonly IEmailSender _email;
    private readonly TimeProvider _clock;

    public RequestEmailChangeHandler(
        IUserRepository users,
        IEmailSender email,
        TimeProvider clock)
    {
        _users = users;
        _email = email;
        _clock = clock;
    }

    public async Task Consume(ConsumeContext<RequestEmailChangeCommand> context)
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

        EmailAddress newEmail;
        try
        {
            newEmail = new EmailAddress(msg.Dto.NewEmail);
        }
        catch (DomainException ex)
        {
            await context.RespondAsync(Result.Failure(new Error("USER_RULE_VIOLATION", ex.Message)));
            return;
        }

        // UX guard: EmailAddress lowercase-normalized record struct; .Equals deger esitligi.
        if (user.Email.Equals(newEmail))
        {
            await context.RespondAsync(Result.Failure(
                new Error("INVALID_REQUEST", "Yeni email mevcut email ile ayni.")));
            return;
        }

        try
        {
            var now = _clock.GetUtcNow();
            var ttl = TimeSpan.FromHours(24);
            var raw = user.RequestEmailChange(newEmail, ttl, now);
            // Raw token SADECE mail icin; HTTP response'a YAZILMAZ (D.1c kontrati).
            await _email.SendEmailChangeAsync(newEmail, raw, ct);
            await context.RespondAsync(Result.Success());
        }
        catch (DomainException ex)
        {
            await context.RespondAsync(Result.Failure(new Error("USER_RULE_VIOLATION", ex.Message)));
        }
    }
}
