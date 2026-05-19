using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LivestockTrading.Catalog.Application.Features.AdminCatalogRead;

/// <summary>
/// Admin: Catalog read endpoints aggregator — MapGroup wiring for 3 read endpoints
/// (brands cursor list, missing translations report, rate logs diagnostic).
/// Route group: /admin/catalog (RequireRole admin/moderator, tag Admin: Catalog).
/// Host-inert: LivestockTrading.Api host-wiring Wave 3 backlog item 2.
/// Backing AdminCatalogReadService NotImpl Wave 3'e kadar (501-mapping retro 19).
/// </summary>
public static class AdminCatalogReadEndpoints
{
    public static IEndpointRouteBuilder MapAdminCatalogReadEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/admin/catalog")
            .RequireAuthorization(policy => policy.RequireRole("admin", "moderator"))
            .WithTags("Admin: Catalog");

        group.MapGet("/brands", ListBrandsEndpoint.Handle);
        group.MapGet("/translations/missing", GetMissingTranslationsEndpoint.Handle);
        group.MapGet("/rate-logs", GetRateLogsEndpoint.Handle);

        return app;
    }
}
