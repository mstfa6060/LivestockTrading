using LivestockTrading.Identity.Application.Common;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Me;

public static class UpdateConsentsEndpoint
{
    public static async Task<IResult> Handle(
        UpdateConsentsRequest body,
        IScopedMediator mediator,
        HttpContext http,
        CancellationToken ct)
    {
        var userId = http.GetUserId();
        var ip = http.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var ua = http.Request.Headers.UserAgent.ToString();

        var client = mediator.CreateRequestClient<UpdateConsentsCommand>();
        var response = await client.GetResponse<Result>(
            new UpdateConsentsCommand(userId, body, ip, ua), ct);

        return response.Message.ToApiResult();
    }
}
