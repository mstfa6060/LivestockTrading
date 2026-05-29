using LivestockTrading.Identity.Application.Common;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Auth;

public static class SendEmailVerifyEndpoint
{
    public static async Task<IResult> Handle(
        SendEmailVerifyRequest body,
        IScopedMediator mediator,
        HttpContext http,
        CancellationToken ct)
    {
        var ip = http.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var client = mediator.CreateRequestClient<SendEmailVerifyCommand>();
        var response = await client.GetResponse<Result>(
            new SendEmailVerifyCommand(body, ip), ct);

        return response.Message.ToApiResult();
    }
}
