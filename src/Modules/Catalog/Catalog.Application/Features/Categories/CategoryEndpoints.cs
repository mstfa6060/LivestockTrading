using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LivestockTrading.Catalog.Application.Features.Categories;

/// <summary>
/// Category endpoint registration — MapGroup wiring for 5 Category admin endpoints.
/// Route group: /admin/catalog/categories (RequireRole admin/moderator, tag Admin: Catalog).
/// Host application invokes MapCategoryEndpoints in startup configuration.
/// Auth enforcement (RequireRole) host-auth wave'inde aktif olur; W2.1 endpoint'leri
/// yapisal tam, JWT bearer henuz host'ta yapilandirilmadi (auth-inert kabul).
/// </summary>
public static class CategoryEndpoints
{
    public static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/admin/catalog/categories")
            .RequireAuthorization(policy => policy.RequireRole("admin", "moderator"))
            .WithTags("Admin: Catalog");

        group.MapPost("/", CreateCategoryEndpoint.Handle);
        group.MapPut("/{id:int}", UpdateCategoryEndpoint.Handle);
        group.MapPost("/{id:int}/deactivate", DeactivateCategoryEndpoint.Handle);
        group.MapPost("/{categoryId:int}/attributes", AddCategoryAttributeEndpoint.Handle);
        group.MapDelete("/{categoryId:int}/attributes/{attributeId:guid}", RemoveCategoryAttributeEndpoint.Handle);

        return app;
    }
}
