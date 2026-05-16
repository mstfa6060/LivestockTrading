namespace Shared.Contracts.Catalog.Admin;

using Shared.Pagination;

/// <summary>Cross-modül admin okuma (Karar 3d). 05-catalog §5:588-596 birebir. actorAdminId YOK (read ≠ command, doc-literal).</summary>
public interface IAdminCatalogReadService
{
    Task<CursorPage<BrandListItem>> ListBrandsAsync(BrandListFilter filter, CancellationToken ct);
    Task<MissingTranslationsReport> GetMissingTranslationsAsync(string locale, CancellationToken ct);
    Task<IReadOnlyList<RateLogEntry>> GetRateLogsAsync(int days, CancellationToken ct);
}
