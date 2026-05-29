using LivestockTrading.Identity.Application.Abstractions;
using LivestockTrading.Identity.Domain.Events.Internal;
using MassTransit;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Me;

public sealed class RequestDataExportHandler : IConsumer<RequestDataExportCommand>
{
    private readonly IUserRepository _users;
    private readonly IPublishEndpoint _publish;
    private readonly TimeProvider _clock;

    public RequestDataExportHandler(
        IUserRepository users,
        IPublishEndpoint publish,
        TimeProvider clock)
    {
        _users = users;
        _publish = publish;
        _clock = clock;
    }

    public async Task Consume(ConsumeContext<RequestDataExportCommand> context)
    {
        var ct = context.CancellationToken;
        var now = _clock.GetUtcNow();

        try
        {
            var user = await _users.GetByIdAsync(context.Message.UserId, ct);
            if (user is null)
            {
                await context.RespondAsync<Result<DataExportAccepted>>(Result.Failure<DataExportAccepted>(
                    new Error("NOT_FOUND_USER", $"User not found: {context.Message.UserId}")));
                return;
            }

            // No Domain mutation here — the worker (W4.3 Quartz job, plan-doc §11)
            // collects sections from IDataExportContributor implementations, encrypts,
            // signs the URL and emails it. The Application layer's job is only to
            // accept the request and emit the trigger.
            var jobId = Guid.CreateVersion7();
            var estimatedReadyAt = now.AddMinutes(15);

            await _publish.Publish(
                new DataExportRequested(user.Id, context.Message.Dto.Format, jobId, now),
                ct);

            await context.RespondAsync<Result<DataExportAccepted>>(Result.Success(
                new DataExportAccepted(jobId, estimatedReadyAt)));
        }
        catch (DomainException ex)
        {
            await context.RespondAsync<Result<DataExportAccepted>>(Result.Failure<DataExportAccepted>(
                new Error("USER_RULE_VIOLATION", ex.Message)));
        }
    }
}
