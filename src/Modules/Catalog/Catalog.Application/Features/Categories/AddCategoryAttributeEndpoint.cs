using LivestockTrading.Catalog.Application.Common;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Contracts.Catalog.Admin;
using Shared.Results;

namespace LivestockTrading.Catalog.Application.Features.Categories;

public static class AddCategoryAttributeEndpoint
{
    public static async Task<IResult> Handle(
        int categoryId,
        AttributeDto dto,
        IScopedMediator mediator,
        HttpContext http,
        CancellationToken ct)
    {
        var actorAdminId = http.GetActorAdminId();
        var client = mediator.CreateRequestClient<AddCategoryAttributeCommand>();

        var response = await client.GetResponse<Result>(
            new AddCategoryAttributeCommand(categoryId, dto, actorAdminId), ct);

        return response.Message.ToApiResult();
    }
}
