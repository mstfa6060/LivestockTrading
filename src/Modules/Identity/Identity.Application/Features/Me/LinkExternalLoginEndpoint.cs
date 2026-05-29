using LivestockTrading.Identity.Application.Common;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Me;

public static class LinkExternalLoginEndpoint
{
    public static async Task<IResult> Handle(
        LinkExternalLoginRequest body,
        IScopedMediator mediator,
        HttpContext http,
        CancellationToken ct)
    {
        var userId = http.GetUserId();

        var client = mediator.CreateRequestClient<LinkExternalLoginCommand>();
        var response = await client.GetResponse<Result>(
            new LinkExternalLoginCommand(userId, body), ct);

        return response.Message.ToApiResult();
    }
}
