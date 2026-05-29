using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LivestockTrading.Identity.Application.Features.Auth;

/// <summary>
/// Identity auth endpoint registration — MapGroup wiring for token lifecycle.
/// Route group: /identity/auth (anonymous for login/refresh, authenticated for
/// logout). Host application invokes MapAuthEndpoints in startup configuration.
/// Register endpoints (RegisterWithPassword + RegisterWithSocial) are added in
/// W4.2.B.3; this aggregator is intentionally partial until then.
/// </summary>
public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/identity/auth").WithTags("Identity: Auth");

        group.MapPost("/login", LoginEndpoint.Handle).AllowAnonymous();
        group.MapPost("/refresh", RefreshTokenEndpoint.Handle).AllowAnonymous();
        group.MapPost("/logout", LogoutEndpoint.Handle).RequireAuthorization();

        return app;
    }
}
