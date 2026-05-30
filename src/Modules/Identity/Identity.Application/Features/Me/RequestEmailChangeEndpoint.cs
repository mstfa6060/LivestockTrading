using LivestockTrading.Identity.Application.Common;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Me;

public static class RequestEmailChangeEndpoint
{
    public static async Task<IResult> Handle(
        RequestEmailChangeRequest body,
        IScopedMediator mediator,
        HttpContext http,
        CancellationToken ct)
    {
        var userId = http.GetUserId();
        var ip = http.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var client = mediator.CreateRequestClient<RequestEmailChangeCommand>();
        var response = await client.GetResponse<Result>(
            new RequestEmailChangeCommand(userId, body, ip), ct);

        return response.Message.ToApiResult();
    }
}
