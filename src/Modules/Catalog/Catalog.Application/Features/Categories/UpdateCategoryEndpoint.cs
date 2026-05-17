using LivestockTrading.Catalog.Application.Common;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Contracts.Catalog.Admin;
using Shared.Results;

namespace LivestockTrading.Catalog.Application.Features.Categories;

public static class UpdateCategoryEndpoint
{
    public static async Task<IResult> Handle(
        int id,
        UpdateCategoryDto dto,
        IScopedMediator mediator,
        HttpContext http,
        CancellationToken ct)
    {
        var actorAdminId = http.GetActorAdminId();
        var client = mediator.CreateRequestClient<UpdateCategoryCommand>();

        // Karar W2.0-3 + Plan-4 doc-grounded: void admin için tek-response yeterli
        // Hem handler hem ValidationFilter aynı tip (Result non-generic) döndürür
        var response = await client.GetResponse<Result>(
            new UpdateCategoryCommand(id, dto, actorAdminId), ct);

        return response.Message.ToApiResult();
    }
}
