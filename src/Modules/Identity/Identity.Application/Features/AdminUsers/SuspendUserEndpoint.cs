using LivestockTrading.Identity.Application.Common;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.AdminUsers;

public static class SuspendUserEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        SuspendUserRequest request,
        IScopedMediator mediator,
        HttpContext http,
        CancellationToken ct)
    {
        var actorAdminId = http.GetUserId();
        var client = mediator.CreateRequestClient<SuspendUserCommand>();

        var response = await client.GetResponse<Result>(
            new SuspendUserCommand(id, request.Reason, actorAdminId), ct);

        return response.Message.ToApiResult();
    }
}
