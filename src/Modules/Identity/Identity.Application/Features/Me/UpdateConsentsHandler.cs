using LivestockTrading.Identity.Application.Abstractions;
using LivestockTrading.Identity.Domain.ValueObjects;
using MassTransit;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Me;

public sealed class UpdateConsentsHandler : IConsumer<UpdateConsentsCommand>
{
    private readonly IUserRepository _users;
    private readonly TimeProvider _clock;

    public UpdateConsentsHandler(IUserRepository users, TimeProvider clock)
    {
        _users = users;
        _clock = clock;
    }

    public async Task Consume(ConsumeContext<UpdateConsentsCommand> context)
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

            foreach (var change in context.Message.Dto.Changes)
            {
                if (change.Granted)
                {
                    user.RecordConsent(
                        new ConsentGrant(change.Type, change.Version, true),
                        context.Message.IpAddress,
                        context.Message.UserAgent,
                        now);
                }
                else
                {
                    // Validator already blocks revoking TermsAndPrivacy/MinistryDataShare;
                    // any remaining revoke here is for MarketingEmail or future opt-in types.
                    user.RevokeConsent(change.Type, now);
                }
            }

            await context.RespondAsync(Result.Success());
        }
        catch (DomainException ex)
        {
            await context.RespondAsync(Result.Failure(new Error("USER_RULE_VIOLATION", ex.Message)));
        }
    }
}
