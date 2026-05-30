using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LivestockTrading.Identity.Application.Features.Auth;

/// <summary>
/// Identity auth endpoint registration — MapGroup wiring for token lifecycle,
/// password register, email verification and OAuth social register. Route
/// group: /identity/auth (anonymous for everything except logout). Host
/// application invokes MapAuthEndpoints in startup configuration.
/// </summary>
public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/identity/auth").WithTags("Identity: Auth");

        group.MapPost("/login", LoginEndpoint.Handle).AllowAnonymous();
        group.MapPost("/refresh", RefreshTokenEndpoint.Handle).AllowAnonymous();
        group.MapPost("/logout", LogoutEndpoint.Handle).RequireAuthorization();
        group.MapPost("/register", RegisterWithPasswordEndpoint.Handle).AllowAnonymous();
        group.MapPost("/email/send-verify", SendEmailVerifyEndpoint.Handle).AllowAnonymous();
        group.MapPost("/email/verify", VerifyEmailEndpoint.Handle).AllowAnonymous();
        group.MapPost("/oauth/google", RegisterWithSocialEndpoint.Handle).AllowAnonymous();
        group.MapPost("/oauth/apple", RegisterWithSocialEndpoint.Handle).AllowAnonymous();

        return app;
    }
}
