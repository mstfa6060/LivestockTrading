using LivestockTrading.Catalog.Application.Common;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Contracts.Catalog.Admin;
using Shared.Results;

namespace LivestockTrading.Catalog.Application.Features.Categories;

public static class CreateCategoryEndpoint
{
    public static async Task<IResult> Handle(
        CreateCategoryDto dto,
        IScopedMediator mediator,
        HttpContext http,
        CancellationToken ct)
    {
        var actorAdminId = http.GetActorAdminId();
        var client = mediator.CreateRequestClient<CreateCategoryCommand>();

        // Karar W2.0-3 + Plan-4 doc-grounded: Create için çoklu-response
        // - Handler success/domain-fail: Result<int>
        // - ValidationFilter shape-fail: non-generic Result
        var response = await client.GetResponse<Result<int>, Result>(
            new CreateCategoryCommand(dto, actorAdminId), ct);

        if (response.Is(out Response<Result<int>>? successResponse))
        {
            var result = successResponse!.Message;
            if (result.IsSuccess)
                return Results.Created($"/admin/catalog/categories/{result.Value}", result.Value);
            return result.ToApiResult();
        }

        if (response.Is(out Response<Result>? failureResponse))
            return failureResponse!.Message.ToApiResult();

        throw new InvalidOperationException("Unexpected MassTransit response type.");
    }
}
