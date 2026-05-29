using LivestockTrading.Identity.Application.Common;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Me;

public static class RegisterDeviceEndpoint
{
    public static async Task<IResult> Handle(
        RegisterDeviceRequest body,
        IScopedMediator mediator,
        HttpContext http,
        CancellationToken ct)
    {
        var userId = http.GetUserId();
        var ua = http.Request.Headers.UserAgent.ToString();

        var client = mediator.CreateRequestClient<RegisterDeviceCommand>();
        var response = await client.GetResponse<Result<Guid>, Result>(
            new RegisterDeviceCommand(userId, body, ua), ct);

        if (response.Is(out Response<Result<Guid>>? successResponse))
        {
            var result = successResponse!.Message;
            if (result.IsSuccess)
                return Results.Created($"/identity/users/me/devices/{result.Value}", result.Value);
            return result.ToApiResult();
        }

        if (response.Is(out Response<Result>? failureResponse))
            return failureResponse!.Message.ToApiResult();

        throw new InvalidOperationException("Unexpected MassTransit response type.");
    }
}
