using Shared.Contracts.Catalog.Admin;
using Shared.Pagination;

namespace LivestockTrading.Catalog.Application.Features.AdminCatalogRead;

/// <summary>
/// W2.6 NotImpl stub. Real implementation Catalog.Infrastructure Wave 3 —
/// IAdminCatalogReadService EF read service. Host-inert (DI registration
/// Wave 3 Infra'da; CatalogApplicationModule dokunulmaz, F-S15).
/// </summary>
public sealed class AdminCatalogReadService : IAdminCatalogReadService
{
    /// <inheritdoc/>
    public Task<CursorPage<BrandListItem>> ListBrandsAsync(BrandListFilter filter, CancellationToken ct)
    {
        throw new NotImplementedException(
            "Catalog.Infrastructure Wave 3 EF read dependency — IBrandRepository cursor query henüz yok.");
    }

    /// <inheritdoc/>
    public Task<MissingTranslationsReport> GetMissingTranslationsAsync(string locale, CancellationToken ct)
    {
        throw new NotImplementedException(
            "Catalog.Infrastructure Wave 3 EF read dependency — cross-aggregate translation scan (Country/Currency/Language/Category/Breed/Brand/CertificationType) henüz yok.");
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<RateLogEntry>> GetRateLogsAsync(int days, CancellationToken ct)
    {
        throw new NotImplementedException(
            "Catalog.Infrastructure.RateProviders bağımlılığı Wave 3 — issue #161/165 backlog. RateLog Domain + RateLogRepository + IRateProvider 3-tier (TCMB/ECB/exchangerate.host) henüz yok.");
    }
}
