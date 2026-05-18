using LivestockTrading.Catalog.Application.Common;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Results;

namespace LivestockTrading.Catalog.Application.Features.BorderRules;

public static class DeactivateBorderRuleEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        IScopedMediator mediator,
        HttpContext http,
        CancellationToken ct)
    {
        var actorAdminId = http.GetActorAdminId();
        var client = mediator.CreateRequestClient<DeactivateBorderRuleCommand>();

        var response = await client.GetResponse<Result>(
            new DeactivateBorderRuleCommand(id, actorAdminId), ct);

        return response.Message.ToApiResult();
    }
}
