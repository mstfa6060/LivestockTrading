using LivestockTrading.Identity.Application.Common;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Auth;

/// <summary>
/// Backs both /identity/auth/oauth/google and /identity/auth/oauth/apple.
/// Provider rides on the request body (whitelist enforced by the validator,
/// Login 3-method discriminator emsali); the two routes share the same handler
/// rather than duplicating endpoint files per provider.
/// </summary>
public static class RegisterWithSocialEndpoint
{
    public static async Task<IResult> Handle(
        RegisterWithSocialRequest body,
        IScopedMediator mediator,
        HttpContext http,
        CancellationToken ct)
    {
        var ip = http.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var ua = http.Request.Headers.UserAgent.ToString();

        var client = mediator.CreateRequestClient<RegisterWithSocialCommand>();
        var response = await client.GetResponse<Result<LoginResponse>, Result>(
            new RegisterWithSocialCommand(body, ip, ua), ct);

        if (response.Is(out Response<Result<LoginResponse>>? successResponse))
            return successResponse!.Message.ToApiResult();

        if (response.Is(out Response<Result>? failureResponse))
            return failureResponse!.Message.ToApiResult();

        throw new InvalidOperationException("Unexpected MassTransit response type.");
    }
}
