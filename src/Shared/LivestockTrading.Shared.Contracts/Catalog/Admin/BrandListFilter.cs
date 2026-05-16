namespace Shared.Contracts.Catalog.Admin;

using Shared.Contracts.Catalog;

/// <summary>Brand list query (§8 /admin/catalog/brands?status=&cursor=). Doc-aligned minimal; PageSize 06-api CursorRequest hizalı (50 default).</summary>
public sealed record BrandListFilter(
    BrandStatus? Status,
    string? Cursor,
    int PageSize = 50);
