using LivestockTrading.Catalog.Application.Common;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Results;

namespace LivestockTrading.Catalog.Application.Features.Brands;

public static class ApproveBrandEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        IScopedMediator mediator,
        HttpContext http,
        CancellationToken ct)
    {
        var actorAdminId = http.GetActorAdminId();
        var client = mediator.CreateRequestClient<ApproveBrandCommand>();

        var response = await client.GetResponse<Result>(
            new ApproveBrandCommand(id, actorAdminId), ct);

        return response.Message.ToApiResult();
    }
}
