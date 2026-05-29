using LivestockTrading.Identity.Application.Common;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Auth;

public static class RefreshTokenEndpoint
{
    public static async Task<IResult> Handle(
        RefreshTokenRequest body,
        IScopedMediator mediator,
        HttpContext http,
        CancellationToken ct)
    {
        var ip = http.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var ua = http.Request.Headers.UserAgent.ToString();

        var client = mediator.CreateRequestClient<RefreshTokenCommand>();
        var response = await client.GetResponse<Result<LoginResponse>, Result>(
            new RefreshTokenCommand(body, ip, ua), ct);

        if (response.Is(out Response<Result<LoginResponse>>? successResponse))
            return successResponse!.Message.ToApiResult();

        if (response.Is(out Response<Result>? failureResponse))
            return failureResponse!.Message.ToApiResult();

        throw new InvalidOperationException("Unexpected MassTransit response type.");
    }
}
