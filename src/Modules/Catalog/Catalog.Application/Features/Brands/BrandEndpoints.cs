using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LivestockTrading.Catalog.Application.Features.Brands;

/// <summary>
/// Brand endpoint registration — MapGroup wiring for 4 Brand admin endpoints.
/// Route group: /admin/catalog/brands (RequireRole admin/moderator, tag Admin: Catalog).
/// Host application invokes MapBrandEndpoints in startup configuration.
/// {id:guid} route (Brand Guid v7 PK — Breed/Category {id:int} ile asimetri).
/// PATCH /admin/catalog/brands/{id} doc endpoint tablosunda var ama port'ta UpdateBrandAsync
/// YOK → yazilmadi (KAYDET-14 port baskin; doc-port inconsistency Wave-2-sonu retro).
/// Auth enforcement (RequireRole) host-auth wave'inde aktif olur; W2.3 endpoint'leri
/// yapisal tam, JWT bearer henuz host'ta yapilandirilmadi (auth-inert kabul).
/// </summary>
public static class BrandEndpoints
{
    public static IEndpointRouteBuilder MapBrandEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/admin/catalog/brands")
            .RequireAuthorization(policy => policy.RequireRole("admin", "moderator"))
            .WithTags("Admin: Catalog");

        group.MapPost("/", CreateBrandEndpoint.Handle);
        group.MapPost("/{id:guid}/approve", ApproveBrandEndpoint.Handle);
        group.MapPost("/{id:guid}/reject", RejectBrandEndpoint.Handle);
        group.MapPost("/{id:guid}/deactivate", DeactivateBrandEndpoint.Handle);

        return app;
    }
}
