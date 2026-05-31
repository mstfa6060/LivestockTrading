using LivestockTrading.Identity.Application.Common;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.AdminUsers;

public static class GrantUserRoleEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        GrantUserRoleRequest request,
        IScopedMediator mediator,
        HttpContext http,
        CancellationToken ct)
    {
        var actorAdminId = http.GetUserId();
        var client = mediator.CreateRequestClient<GrantUserRoleCommand>();

        var response = await client.GetResponse<Result>(
            new GrantUserRoleCommand(id, request.Role, actorAdminId), ct);

        return response.Message.ToApiResult();
    }
}
