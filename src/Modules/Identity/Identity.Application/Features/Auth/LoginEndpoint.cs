using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Results;
using LivestockTrading.Identity.Application.Common;

namespace LivestockTrading.Identity.Application.Features.Auth;

public static class LoginEndpoint
{
    public static async Task<IResult> Handle(
        LoginRequest body,
        IScopedMediator mediator,
        HttpContext http,
        CancellationToken ct)
    {
        var ip = http.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var ua = http.Request.Headers.UserAgent.ToString();

        var client = mediator.CreateRequestClient<LoginCommand>();
        var response = await client.GetResponse<Result<LoginResponse>, Result>(
            new LoginCommand(body, ip, ua), ct);

        if (response.Is(out Response<Result<LoginResponse>>? successResponse))
            return successResponse!.Message.ToApiResult();

        if (response.Is(out Response<Result>? failureResponse))
            return failureResponse!.Message.ToApiResult();

        throw new InvalidOperationException("Unexpected MassTransit response type.");
    }
}
