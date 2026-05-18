using LivestockTrading.Catalog.Application.Common;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Contracts.Catalog.Admin;
using Shared.Results;

namespace LivestockTrading.Catalog.Application.Features.BorderRules;

public static class CreateBorderRuleEndpoint
{
    public static async Task<IResult> Handle(
        CreateBorderRuleDto dto,
        IScopedMediator mediator,
        HttpContext http,
        CancellationToken ct)
    {
        var actorAdminId = http.GetActorAdminId();
        var client = mediator.CreateRequestClient<CreateBorderRuleCommand>();

        // Karar W2.0-3 + Plan-4 doc-grounded: Create için çoklu-response
        // - Handler success/domain-fail: Result<Guid>
        // - ValidationFilter shape-fail: non-generic Result
        var response = await client.GetResponse<Result<Guid>, Result>(
            new CreateBorderRuleCommand(dto, actorAdminId), ct);

        if (response.Is(out Response<Result<Guid>>? successResponse))
        {
            var result = successResponse!.Message;
            if (result.IsSuccess)
                return Results.Created($"/admin/catalog/border-rules/{result.Value}", result.Value);
            return result.ToApiResult();
        }

        if (response.Is(out Response<Result>? failureResponse))
            return failureResponse!.Message.ToApiResult();

        throw new InvalidOperationException("Unexpected MassTransit response type.");
    }
}
