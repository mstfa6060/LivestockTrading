using LivestockTrading.Identity.Application.Common;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Me;

public static class RevokeSessionEndpoint
{
    public static async Task<IResult> Handle(
        Guid sessionId,
        IScopedMediator mediator,
        HttpContext http,
        CancellationToken ct)
    {
        var userId = http.GetUserId();

        var client = mediator.CreateRequestClient<RevokeSessionCommand>();
        var response = await client.GetResponse<Result>(
            new RevokeSessionCommand(userId, sessionId), ct);

        return response.Message.ToApiResult();
    }
}
