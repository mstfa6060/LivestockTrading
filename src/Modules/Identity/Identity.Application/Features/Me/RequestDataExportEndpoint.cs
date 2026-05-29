using LivestockTrading.Identity.Application.Common;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Me;

public static class RequestDataExportEndpoint
{
    public static async Task<IResult> Handle(
        DataExportRequest body,
        IScopedMediator mediator,
        HttpContext http,
        CancellationToken ct)
    {
        var userId = http.GetUserId();

        var client = mediator.CreateRequestClient<RequestDataExportCommand>();
        var response = await client.GetResponse<Result<DataExportAccepted>, Result>(
            new RequestDataExportCommand(userId, body), ct);

        if (response.Is(out Response<Result<DataExportAccepted>>? successResponse))
        {
            var result = successResponse!.Message;
            if (result.IsSuccess)
                return Results.Accepted(
                    $"/identity/users/me/data-export/{result.Value.JobId}",
                    result.Value);
            return result.ToApiResult();
        }

        if (response.Is(out Response<Result>? failureResponse))
            return failureResponse!.Message.ToApiResult();

        throw new InvalidOperationException("Unexpected MassTransit response type.");
    }
}
