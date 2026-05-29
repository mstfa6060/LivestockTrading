using LivestockTrading.Identity.Application.Common;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Auth;

public static class VerifyEmailEndpoint
{
    public static async Task<IResult> Handle(
        VerifyEmailRequest body,
        IScopedMediator mediator,
        HttpContext http,
        CancellationToken ct)
    {
        var client = mediator.CreateRequestClient<VerifyEmailCommand>();
        var response = await client.GetResponse<Result>(
            new VerifyEmailCommand(body), ct);

        return response.Message.ToApiResult();
    }
}
