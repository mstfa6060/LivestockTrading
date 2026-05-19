using Microsoft.AspNetCore.Http;
using Shared.Contracts.Catalog.Admin;

namespace LivestockTrading.Catalog.Application.Features.AdminCatalogRead;

/// <summary>
/// GET /admin/catalog/rate-logs?days=7 — currency rate refresh audit (diagnostic).
/// days default 7 (1-hafta diagnostic window; doc §8:819 ?cursor= vs port int days,
/// port baskın KAYDET-14, deviations retro 18/19a). Param sırası service/ct/days=7
/// C# dil-kısıtı (opsiyonel param son), emsal-asimetrik (retro 19b).
/// Backing AdminCatalogReadService NotImpl Wave 3'e kadar — uncaught
/// NotImplementedException → HTTP 500 default; 501-mapping Wave 3 host
/// concern (deviations retro 19).
/// </summary>
public static class GetRateLogsEndpoint
{
    public static async Task<IResult> Handle(
        IAdminCatalogReadService readService,
        CancellationToken ct,
        int days = 7)
    {
        return Results.Ok(await readService.GetRateLogsAsync(days, ct));
    }
}
