using LivestockTrading.Identity.Application.Common;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Contracts.Identity;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Me;

public static class UpdatePreferencesEndpoint
{
    public static async Task<IResult> Handle(
        UserPreferences body,
        IScopedMediator mediator,
        HttpContext http,
        CancellationToken ct)
    {
        var userId = http.GetUserId();

        var client = mediator.CreateRequestClient<UpdatePreferencesCommand>();
        var response = await client.GetResponse<Result>(
            new UpdatePreferencesCommand(userId, body), ct);

        return response.Message.ToApiResult();
    }
}
