using LivestockTrading.Identity.Application.Common;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Me;

public static class UnlinkExternalLoginEndpoint
{
    public static async Task<IResult> Handle(
        Guid externalLoginId,
        IScopedMediator mediator,
        HttpContext http,
        CancellationToken ct)
    {
        var userId = http.GetUserId();

        var client = mediator.CreateRequestClient<UnlinkExternalLoginCommand>();
        var response = await client.GetResponse<Result>(
            new UnlinkExternalLoginCommand(userId, externalLoginId), ct);

        return response.Message.ToApiResult();
    }
}
