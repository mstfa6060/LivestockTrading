using Microsoft.AspNetCore.Http;
using Shared.Contracts.Catalog.Admin;

namespace LivestockTrading.Catalog.Application.Features.AdminCatalogRead;

/// <summary>
/// GET /admin/catalog/translations/missing?locale= — per-locale missing translations report.
/// locale required (doc-literal, default YOK → minimal-API 400 if absent).
/// Backing AdminCatalogReadService NotImpl Wave 3'e kadar — uncaught
/// NotImplementedException → HTTP 500 default; 501-mapping Wave 3 host
/// concern (deviations retro 19).
/// </summary>
public static class GetMissingTranslationsEndpoint
{
    public static async Task<IResult> Handle(
        string locale,
        IAdminCatalogReadService readService,
        CancellationToken ct)
    {
        return Results.Ok(await readService.GetMissingTranslationsAsync(locale, ct));
    }
}
