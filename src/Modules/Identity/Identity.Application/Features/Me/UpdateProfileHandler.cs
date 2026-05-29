using LivestockTrading.Identity.Application.Abstractions;
using LivestockTrading.Identity.Domain.ValueObjects;
using MassTransit;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Me;

public sealed class UpdateProfileHandler : IConsumer<UpdateProfileCommand>
{
    private readonly IUserRepository _users;
    private readonly TimeProvider _clock;

    public UpdateProfileHandler(IUserRepository users, TimeProvider clock)
    {
        _users = users;
        _clock = clock;
    }

    public async Task Consume(ConsumeContext<UpdateProfileCommand> context)
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

            var name = new PersonName(context.Message.Dto.FirstName, context.Message.Dto.LastName);
            user.UpdateName(name, now);

            if (context.Message.Dto.NationalId is not null)
            {
                // AssignNationalId is set-once — if the user already has one set, Domain
                // throws and we surface USER_RULE_VIOLATION via the outer catch.
                user.AssignNationalId(new NationalId(context.Message.Dto.NationalId), now);
            }

            await context.RespondAsync(Result.Success());
        }
        catch (DomainException ex)
        {
            await context.RespondAsync(Result.Failure(new Error("USER_RULE_VIOLATION", ex.Message)));
        }
    }
}
