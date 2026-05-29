using LivestockTrading.Identity.Application.Abstractions;
using MassTransit;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Me;

public sealed class LinkExternalLoginHandler : IConsumer<LinkExternalLoginCommand>
{
    private readonly IUserRepository _users;
    private readonly IExternalLoginValidator _validator;
    private readonly TimeProvider _clock;

    public LinkExternalLoginHandler(
        IUserRepository users,
        IExternalLoginValidator validator,
        TimeProvider clock)
    {
        _users = users;
        _validator = validator;
        _clock = clock;
    }

    public async Task Consume(ConsumeContext<LinkExternalLoginCommand> context)
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

            var validation = await _validator.ValidateAsync(
                context.Message.Dto.Provider,
                context.Message.Dto.IdToken,
                ct);

            if (validation.IsFailure)
            {
                await context.RespondAsync(Result.Failure(validation.Error!));
                return;
            }

            var ext = validation.Value;
            user.LinkExternalLogin(context.Message.Dto.Provider, ext.ExternalId, now);

            await context.RespondAsync(Result.Success());
        }
        catch (DomainException ex)
        {
            await context.RespondAsync(Result.Failure(new Error("USER_RULE_VIOLATION", ex.Message)));
        }
    }
}
