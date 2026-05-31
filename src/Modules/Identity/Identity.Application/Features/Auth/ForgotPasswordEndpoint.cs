using LivestockTrading.Identity.Application.Common;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Auth;

public static class ForgotPasswordEndpoint
{
    public static async Task<IResult> Handle(
        ForgotPasswordRequest body,
        IScopedMediator mediator,
        HttpContext http,
        CancellationToken ct)
    {
        var ip = http.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var client = mediator.CreateRequestClient<ForgotPasswordCommand>();
        var response = await client.GetResponse<Result>(
            new ForgotPasswordCommand(body, ip), ct);

        return response.Message.ToApiResult();
    }
}
