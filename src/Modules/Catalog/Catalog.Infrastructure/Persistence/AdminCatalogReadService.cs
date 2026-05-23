using LivestockTrading.Catalog.Domain.Aggregates;
using LivestockTrading.Catalog.Domain.Entities;
using LivestockTrading.Catalog.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.Catalog;
using Shared.Contracts.Catalog.Admin;
using Shared.Pagination;
using Shared.ValueObjects;

namespace LivestockTrading.Catalog.Infrastructure.Persistence;

/// <summary>
/// IAdminCatalogReadService Infrastructure impl (Karar 3d, 05-catalog §5:588-596).
/// 3 metot: ListBrandsAsync (CursorPage Guid v7 chronological) + GetMissingTranslations
/// Async (4 bucket B-pattern jsonb ContainsKey LINQ emsali yok) + GetRateLogsAsync (RateLog
/// son N gün). AsNoTracking + CancellationToken her metot. DbContext _db.Set&lt;T&gt;()
/// pattern (W3.3 emsali). BrandStatus enum cast (Shared.Contracts.Catalog.BrandStatus)
/// W3.5B.2 emsali (S12+S14 emsali).
/// </summary>
internal sealed class AdminCatalogReadService : IAdminCatalogReadService
{
    private readonly CatalogDbContext _db;

    public AdminCatalogReadService(CatalogDbContext db) => _db = db;

    public async Task<CursorPage<BrandListItem>> ListBrandsAsync(BrandListFilter filter, CancellationToken ct)
    {
        Guid? cursorId = null;
        if (!string.IsNullOrEmpty(filter.Cursor))
        {
            if (!Guid.TryParse(filter.Cursor, out var parsed))
                throw new ArgumentException("Cursor format invalid.", nameof(filter));
            cursorId = parsed;
        }

        var query = _db.Set<Brand>().AsNoTracking();

        if (filter.Status.HasValue)
        {
            var statusFilter = filter.Status.Value;
            query = query.Where(b => (Shared.Contracts.Catalog.BrandStatus)b.Status == statusFilter);
        }

        if (cursorId.HasValue)
        {
            var cursorValue = cursorId.Value;
            query = query.Where(b => b.Id > cursorValue);
        }

        var pageSize = filter.PageSize;

        var items = await query
            .OrderBy(b => b.Id)
            .Take(pageSize + 1)
            .Select(b => new BrandListItem(
                b.Id,
                b.Slug,
                b.Name,
                (Shared.Contracts.Catalog.BrandStatus)b.Status,
                b.IsActive,
                b.OriginCountry == null ? null : b.OriginCountry.Value.Value,
                b.SuggestedByUserId,
                b.SuggestedAt,
                b.CreatedAt,
                b.DisplayOrder))
            .ToListAsync(ct);

        var hasMore = items.Count > pageSize;
        var resultItems = hasMore ? items.Take(pageSize).ToList() : items;
        string? nextCursor = hasMore ? resultItems[^1].Id.ToString() : null;

        return new CursorPage<BrandListItem>(resultItems, nextCursor, hasMore, TotalCount: null);
    }

    public async Task<MissingTranslationsReport> GetMissingTranslationsAsync(string locale, CancellationToken ct)
    {
        // VO ctor validation: invalid locale → DomainException (LanguageCode.cs:17-22).
        var targetLocale = new LanguageCode(locale);

        // 4 bucket (B) inline pattern — Func<T, Translations> helper EF translate uyumsuz
        // (lambda invocation projection içinde dispatch edilemez). Kod tekrarı kabul
        // (W3.5B.2 emsali premature abstraction kaçınma). jsonb ContainsKey LINQ emsali
        // yok (S15) — ToListAsync + in-memory filter güvenli.

        var catList = await _db.Set<Category>().AsNoTracking()
            .Where(c => c.IsActive)
            .Select(c => new { c.Code, c.Name })
            .ToListAsync(ct);
        var categories = catList
            .Where(x => !x.Name.Map.ContainsKey(targetLocale))
            .Select(x => new MissingTranslationItem(x.Code, x.Name.FirstOrEmpty()))
            .ToList();

        var breedList = await _db.Set<Breed>().AsNoTracking()
            .Where(b => b.IsActive)
            .Select(b => new { b.Code, b.Name })
            .ToListAsync(ct);
        var breeds = breedList
            .Where(x => !x.Name.Map.ContainsKey(targetLocale))
            .Select(x => new MissingTranslationItem(x.Code, x.Name.FirstOrEmpty()))
            .ToList();

        var brandList = await _db.Set<Brand>().AsNoTracking()
            .Where(b => b.IsActive)
            .Select(b => new { b.Slug, b.Name })
            .ToListAsync(ct);
        var brands = brandList
            .Where(x => !x.Name.Map.ContainsKey(targetLocale))
            .Select(x => new MissingTranslationItem(x.Slug, x.Name.FirstOrEmpty()))
            .ToList();

        var certList = await _db.Set<CertificationType>().AsNoTracking()
            .Where(ct1 => ct1.IsActive)
            .Select(ct1 => new { ct1.Code, Name = ct1.NameTranslations })
            .ToListAsync(ct);
        var certifications = certList
            .Where(x => !x.Name.Map.ContainsKey(targetLocale))
            .Select(x => new MissingTranslationItem(x.Code, x.Name.FirstOrEmpty()))
            .ToList();

        return new MissingTranslationsReport(locale, categories, breeds, brands, certifications);
    }

    public async Task<IReadOnlyList<RateLogEntry>> GetRateLogsAsync(int days, CancellationToken ct)
    {
        var cutoff = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-days));
        return await _db.Set<RateLog>().AsNoTracking()
            .Where(r => r.RateDate >= cutoff)
            .OrderByDescending(r => r.FetchedAt)
            .Select(r => new RateLogEntry(
                r.RateDate,
                r.Source,
                r.RatesJson,
                r.Success,
                r.Error,
                r.FetchedAt))
            .ToListAsync(ct);
    }
}
