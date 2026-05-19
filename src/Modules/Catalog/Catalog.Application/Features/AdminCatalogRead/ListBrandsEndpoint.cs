using Microsoft.AspNetCore.Http;
using Shared.Contracts.Catalog.Admin;

namespace LivestockTrading.Catalog.Application.Features.AdminCatalogRead;

/// <summary>
/// GET /admin/catalog/brands — admin Brand cursor list ([AsParameters] BrandListFilter).
/// Backing AdminCatalogReadService NotImpl Wave 3'e kadar — uncaught
/// NotImplementedException → HTTP 500 default; 501-mapping Wave 3 host
/// concern (deviations retro 19).
/// </summary>
public static class ListBrandsEndpoint
{
    public static async Task<IResult> Handle(
        [AsParameters] BrandListFilter filter,
        IAdminCatalogReadService readService,
        CancellationToken ct)
    {
        return Results.Ok(await readService.ListBrandsAsync(filter, ct));
    }
}
