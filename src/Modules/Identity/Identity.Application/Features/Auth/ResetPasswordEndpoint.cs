using LivestockTrading.Identity.Application.Common;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Auth;

public static class ResetPasswordEndpoint
{
    public static async Task<IResult> Handle(
        ResetPasswordRequest body,
        IScopedMediator mediator,
        HttpContext http,
        CancellationToken ct)
    {
        var ip = http.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var client = mediator.CreateRequestClient<ResetPasswordCommand>();
        var response = await client.GetResponse<Result>(
            new ResetPasswordCommand(body, ip), ct);

        return response.Message.ToApiResult();
    }
}
