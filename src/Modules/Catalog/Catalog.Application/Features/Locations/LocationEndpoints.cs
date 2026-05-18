using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LivestockTrading.Catalog.Application.Features.Locations;

/// <summary>
/// Location admin endpoint registration — MapGroup wiring for 2 Location admin endpoints.
/// Route group: /admin/catalog/locations (RequireRole admin/moderator, tag Admin: Catalog).
/// W2.4-C: host-inert (S4, Program.cs wire Wave 3'te). MapLocationEndpoints çağrılmıyor.
/// Test handler-direct (W2.4-D scope). Auth enforcement host-auth wave'inde aktif.
/// </summary>
public static class LocationEndpoints
{
    public static IEndpointRouteBuilder MapLocationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/admin/catalog/locations")
            .RequireAuthorization(policy => policy.RequireRole("admin", "moderator"))
            .WithTags("Admin: Catalog");

        group.MapPost("/", CreateLocationEndpoint.Handle);
        group.MapPatch("/{id:int}", UpdateLocationEndpoint.Handle);

        return app;
    }
}
