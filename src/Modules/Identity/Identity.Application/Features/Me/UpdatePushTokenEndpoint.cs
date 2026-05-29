using LivestockTrading.Identity.Application.Common;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Me;

public static class UpdatePushTokenEndpoint
{
    public static async Task<IResult> Handle(
        Guid deviceId,
        UpdatePushTokenRequest body,
        IScopedMediator mediator,
        HttpContext http,
        CancellationToken ct)
    {
        var userId = http.GetUserId();

        var client = mediator.CreateRequestClient<UpdatePushTokenCommand>();
        var response = await client.GetResponse<Result>(
            new UpdatePushTokenCommand(userId, deviceId, body), ct);

        return response.Message.ToApiResult();
    }
}
