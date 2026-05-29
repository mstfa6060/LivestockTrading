using LivestockTrading.Identity.Application.Abstractions;
using MassTransit;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Me;

public sealed class RegisterDeviceHandler : IConsumer<RegisterDeviceCommand>
{
    private readonly IUserRepository _users;
    private readonly TimeProvider _clock;

    public RegisterDeviceHandler(IUserRepository users, TimeProvider clock)
    {
        _users = users;
        _clock = clock;
    }

    public async Task Consume(ConsumeContext<RegisterDeviceCommand> context)
    {
        var ct = context.CancellationToken;
        var now = _clock.GetUtcNow();

        try
        {
            var user = await _users.GetByIdAsync(context.Message.UserId, ct);
            if (user is null)
            {
                await context.RespondAsync<Result<Guid>>(Result.Failure<Guid>(
                    new Error("NOT_FOUND_USER", $"User not found: {context.Message.UserId}")));
                return;
            }

            var device = user.RegisterDevice(
                context.Message.Dto.Platform,
                context.Message.UserAgent,
                context.Message.Dto.DeviceFingerprint,
                context.Message.Dto.PushToken,
                now);

            await context.RespondAsync<Result<Guid>>(Result.Success(device.Id));
        }
        catch (DomainException ex)
        {
            await context.RespondAsync<Result<Guid>>(Result.Failure<Guid>(
                new Error("USER_RULE_VIOLATION", ex.Message)));
        }
    }
}
