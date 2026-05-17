using LivestockTrading.Catalog.Application.Common;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Results;

namespace LivestockTrading.Catalog.Application.Features.Categories;

public static class DeactivateCategoryEndpoint
{
    public static async Task<IResult> Handle(
        int id,
        IScopedMediator mediator,
        HttpContext http,
        CancellationToken ct)
    {
        var actorAdminId = http.GetActorAdminId();
        var client = mediator.CreateRequestClient<DeactivateCategoryCommand>();

        var response = await client.GetResponse<Result>(
            new DeactivateCategoryCommand(id, actorAdminId), ct);

        return response.Message.ToApiResult();
    }
}
