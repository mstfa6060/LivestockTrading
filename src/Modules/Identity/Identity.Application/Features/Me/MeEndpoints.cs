using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LivestockTrading.Identity.Application.Features.Me;

/// <summary>
/// Identity self-service endpoint registration — MapGroup wiring for /me/* surface.
/// All routes require authentication (group-level RequireAuthorization). Host
/// application invokes MapMeEndpoints in startup configuration. Avatar, devices
/// and external-logins endpoints are added in later W4.2.C sub-batches; this
/// aggregator is partial until those land.
/// </summary>
public static class MeEndpoints
{
    public static IEndpointRouteBuilder MapMeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/identity/users/me")
            .WithTags("Identity: Me")
            .RequireAuthorization();

        group.MapGet("", MeProfileEndpoint.Handle);
        group.MapPatch("", UpdateProfileEndpoint.Handle);
        group.MapPatch("/preferences", UpdatePreferencesEndpoint.Handle);
        group.MapPatch("/consents", UpdateConsentsEndpoint.Handle);
        group.MapPost("/password", ChangePasswordEndpoint.Handle);
        group.MapGet("/sessions", SessionsEndpoint.Handle);
        group.MapDelete("/sessions/{sessionId:guid}", RevokeSessionEndpoint.Handle);
        group.MapPost("/account/delete-request", RequestDeletionEndpoint.Handle);
        group.MapPost("/account/restore", RestoreAccountEndpoint.Handle);
        group.MapPost("/data-export", RequestDataExportEndpoint.Handle);

        return app;
    }
}
