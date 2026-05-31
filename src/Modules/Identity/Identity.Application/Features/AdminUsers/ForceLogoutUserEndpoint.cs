using LivestockTrading.Identity.Application.Common;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.AdminUsers;

public static class ForceLogoutUserEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        IScopedMediator mediator,
        HttpContext http,
        CancellationToken ct)
    {
        var actorAdminId = http.GetUserId();
        var client = mediator.CreateRequestClient<ForceLogoutUserCommand>();

        var response = await client.GetResponse<Result>(
            new ForceLogoutUserCommand(id, actorAdminId), ct);

        return response.Message.ToApiResult();
    }
}
