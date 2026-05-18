using LivestockTrading.Catalog.Application.Common;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Contracts.Catalog.Admin;
using Shared.Results;

namespace LivestockTrading.Catalog.Application.Features.BorderRules;

public static class UpdateBorderRuleEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        UpdateBorderRuleDto dto,
        IScopedMediator mediator,
        HttpContext http,
        CancellationToken ct)
    {
        var actorAdminId = http.GetActorAdminId();
        var client = mediator.CreateRequestClient<UpdateBorderRuleCommand>();

        // Karar W2.0-3 + Plan-4 doc-grounded: void admin için tek-response yeterli
        // Hem handler hem ValidationFilter aynı tip (Result non-generic) döndürür
        var response = await client.GetResponse<Result>(
            new UpdateBorderRuleCommand(id, dto, actorAdminId), ct);

        return response.Message.ToApiResult();
    }
}
