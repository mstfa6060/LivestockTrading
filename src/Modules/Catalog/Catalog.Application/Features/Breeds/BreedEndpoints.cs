using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LivestockTrading.Catalog.Application.Features.Breeds;

/// <summary>
/// Breed endpoint registration — MapGroup wiring for 3 Breed admin endpoints.
/// Route group: /admin/catalog/breeds (RequireRole admin/moderator, tag Admin: Catalog).
/// Host application invokes MapBreedEndpoints in startup configuration.
/// Update endpoint PATCH (doc 06-api-contract §798 birebir — Category PUT ile asimetri).
/// Auth enforcement (RequireRole) host-auth wave'inde aktif olur; W2.2 endpoint'leri
/// yapisal tam, JWT bearer henuz host'ta yapilandirilmadi (auth-inert kabul).
/// </summary>
public static class BreedEndpoints
{
    public static IEndpointRouteBuilder MapBreedEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/admin/catalog/breeds")
            .RequireAuthorization(policy => policy.RequireRole("admin", "moderator"))
            .WithTags("Admin: Catalog");

        group.MapPost("/", CreateBreedEndpoint.Handle);
        group.MapPatch("/{id:int}", UpdateBreedEndpoint.Handle);
        group.MapPost("/{id:int}/deactivate", DeactivateBreedEndpoint.Handle);

        return app;
    }
}
