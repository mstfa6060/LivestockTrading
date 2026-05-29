using LivestockTrading.Identity.Application.Common;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Me;

public static class RemoveDeviceEndpoint
{
    public static async Task<IResult> Handle(
        Guid deviceId,
        IScopedMediator mediator,
        HttpContext http,
        CancellationToken ct)
    {
        var userId = http.GetUserId();

        var client = mediator.CreateRequestClient<RemoveDeviceCommand>();
        var response = await client.GetResponse<Result>(
            new RemoveDeviceCommand(userId, deviceId), ct);

        return response.Message.ToApiResult();
    }
}
