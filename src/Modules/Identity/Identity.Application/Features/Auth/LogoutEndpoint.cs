using LivestockTrading.Identity.Application.Common;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Auth;

public static class LogoutEndpoint
{
    public static async Task<IResult> Handle(
        LogoutRequest body,
        IScopedMediator mediator,
        HttpContext http,
        CancellationToken ct)
    {
        var userId = http.GetUserId();

        var client = mediator.CreateRequestClient<LogoutCommand>();
        var response = await client.GetResponse<Result>(
            new LogoutCommand(userId, body), ct);

        return response.Message.ToApiResult();
    }
}
