using LivestockTrading.Identity.Application.Common;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Me;

public static class ChangePasswordEndpoint
{
    public static async Task<IResult> Handle(
        ChangePasswordRequest body,
        IScopedMediator mediator,
        HttpContext http,
        CancellationToken ct)
    {
        var userId = http.GetUserId();
        var ip = http.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var client = mediator.CreateRequestClient<ChangePasswordCommand>();
        var response = await client.GetResponse<Result>(
            new ChangePasswordCommand(userId, body, ip), ct);

        return response.Message.ToApiResult();
    }
}
