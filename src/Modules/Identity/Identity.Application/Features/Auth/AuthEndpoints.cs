using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LivestockTrading.Identity.Application.Features.Auth;

/// <summary>
/// Identity auth endpoint registration — MapGroup wiring for token lifecycle
/// and password register. Route group: /identity/auth (anonymous for login,
/// refresh and register, authenticated for logout). Host application invokes
/// MapAuthEndpoints in startup configuration. Social register (oauth/google,
/// oauth/apple) is added in W4.2.C alongside IExternalLoginValidator.
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

        return app;
    }
}
