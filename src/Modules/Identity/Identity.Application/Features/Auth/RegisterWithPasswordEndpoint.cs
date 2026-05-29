using LivestockTrading.Identity.Application.Common;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Auth;

public static class RegisterWithPasswordEndpoint
{
    public static async Task<IResult> Handle(
        RegisterWithPasswordRequest body,
        IScopedMediator mediator,
        HttpContext http,
        CancellationToken ct)
    {
        var ip = http.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var ua = http.Request.Headers.UserAgent.ToString();

        var client = mediator.CreateRequestClient<RegisterWithPasswordCommand>();
        var response = await client.GetResponse<Result<Guid>, Result>(
            new RegisterWithPasswordCommand(body, ip, ua), ct);

        if (response.Is(out Response<Result<Guid>>? successResponse))
        {
            var result = successResponse!.Message;
            if (result.IsSuccess)
                return Results.Created($"/identity/users/{result.Value}", result.Value);
            return result.ToApiResult();
        }

        if (response.Is(out Response<Result>? failureResponse))
            return failureResponse!.Message.ToApiResult();

        throw new InvalidOperationException("Unexpected MassTransit response type.");
    }
}
