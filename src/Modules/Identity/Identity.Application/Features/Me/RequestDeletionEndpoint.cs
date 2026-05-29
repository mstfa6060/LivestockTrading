using LivestockTrading.Identity.Application.Common;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Me;

public static class RequestDeletionEndpoint
{
    public static async Task<IResult> Handle(
        IScopedMediator mediator,
        HttpContext http,
        CancellationToken ct)
    {
        var userId = http.GetUserId();

        var client = mediator.CreateRequestClient<RequestDeletionCommand>();
        var response = await client.GetResponse<Result>(
            new RequestDeletionCommand(userId), ct);

        return response.Message.ToApiResult();
    }
}
