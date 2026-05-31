using LivestockTrading.Identity.Application.Common;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.AdminUsers;

public static class RevokeUserRoleEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        RevokeUserRoleRequest request,
        IScopedMediator mediator,
        HttpContext http,
        CancellationToken ct)
    {
        var actorAdminId = http.GetUserId();
        var client = mediator.CreateRequestClient<RevokeUserRoleCommand>();

        var response = await client.GetResponse<Result>(
            new RevokeUserRoleCommand(id, request.Role, actorAdminId), ct);

        return response.Message.ToApiResult();
    }
}
