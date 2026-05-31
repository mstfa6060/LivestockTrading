using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LivestockTrading.Identity.Application.Features.AdminUsers;

/// <summary>
/// Identity admin user endpoint registration — MapGroup wiring for /admin/users/*.
/// Route group: /admin/users (RequireRole "admin", tag Admin: Identity).
/// Auth role host-auth wave revisit (W4.4); E.1 endpoint'leri yapisal tam,
/// JWT bearer host'ta yapilandirilmadi (auth-inert kabul; Catalog
/// CategoryEndpoints emsali).
/// W4.2.E.1 read scope: list + detail + sessions (3 GET). Write endpoint'leri
/// (suspend/reactivate/grant-role/revoke-role/force-logout) E.2-4'te eklenir.
/// Audit endpoint (spec satir 596) Wave 7 Admin module delege; bu aggregator'a
/// girmez (handover-mid §3 satir 91 onayli).
/// </summary>
public static class AdminUsersEndpoints
{
    public static IEndpointRouteBuilder MapAdminUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/admin/users")
            .RequireAuthorization(policy => policy.RequireRole("admin"))
            .WithTags("Admin: Identity");

        group.MapGet("/", ListAdminUsersEndpoint.Handle);
        group.MapGet("/{id:guid}", GetAdminUserDetailEndpoint.Handle);
        group.MapGet("/{id:guid}/sessions", GetAdminUserSessionsEndpoint.Handle);
        group.MapPost("/{id:guid}/suspend", SuspendUserEndpoint.Handle);
        group.MapPost("/{id:guid}/reactivate", ReactivateUserEndpoint.Handle);
        group.MapPost("/{id:guid}/grant-role", GrantUserRoleEndpoint.Handle);
        group.MapPost("/{id:guid}/revoke-role", RevokeUserRoleEndpoint.Handle);
        group.MapPost("/{id:guid}/force-logout", ForceLogoutUserEndpoint.Handle);

        return app;
    }
}
