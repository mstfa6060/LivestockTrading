using LivestockTrading.Catalog.Application.Common;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Results;

namespace LivestockTrading.Catalog.Application.Features.Categories;

public static class RemoveCategoryAttributeEndpoint
{
    public static async Task<IResult> Handle(
        int categoryId,
        Guid attributeId,
        IScopedMediator mediator,
        HttpContext http,
        CancellationToken ct)
    {
        var actorAdminId = http.GetActorAdminId();
        var client = mediator.CreateRequestClient<RemoveCategoryAttributeCommand>();

        var response = await client.GetResponse<Result>(
            new RemoveCategoryAttributeCommand(categoryId, attributeId, actorAdminId), ct);

        return response.Message.ToApiResult();
    }
}
