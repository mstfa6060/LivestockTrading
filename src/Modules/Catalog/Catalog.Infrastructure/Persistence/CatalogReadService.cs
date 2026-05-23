using LivestockTrading.Catalog.Domain.Aggregates;
using LivestockTrading.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.Catalog;

namespace LivestockTrading.Catalog.Infrastructure.Persistence;

/// <summary>
/// ICatalogReadService Infrastructure impl (Karar 3d, 05-catalog §5). 22 metot EF
/// projection AsNoTracking + Select + CancellationToken. Cache wrap YOK Faz 1
/// (W3.6 decorator sub-batch). DbContext _db.Set&lt;T&gt;() pattern (W3.3 emsali,
/// DbSet property yok). Translations sealed class jsonb pass-through (W3.1
/// TranslationsToJsonConverter ValueConverter — server-side materialize).
/// VO accessor: CountryCode struct .Value (non-null + nullable struct pattern).
/// </summary>
internal sealed class CatalogReadService : ICatalogReadService
{
    private readonly CatalogDbContext _db;

    public CatalogReadService(CatalogDbContext db) => _db = db;

    // ───────── Country ─────────

    public Task<CountryDto?> GetCountryAsync(string code, CancellationToken ct)
        => _db.Set<Country>()
            .AsNoTracking()
            .Where(c => c.Code == code)
            .Select(c => new CountryDto(
                c.Code,
                c.NameEn,
                c.NativeName,
                c.Region,
                c.DefaultCurrencyCode,
                c.DefaultLanguageCode,
                c.PhonePrefix,
                c.IsActive,
                c.DisplayOrder))
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<CountryDto>> ListActiveCountriesAsync(CancellationToken ct)
        => await _db.Set<Country>()
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new CountryDto(
                c.Code,
                c.NameEn,
                c.NativeName,
                c.Region,
                c.DefaultCurrencyCode,
                c.DefaultLanguageCode,
                c.PhonePrefix,
                c.IsActive,
                c.DisplayOrder))
            .ToListAsync(ct);

    public Task<bool> IsValidActiveCountryCodeAsync(string code, CancellationToken ct)
        => _db.Set<Country>()
            .AsNoTracking()
            .AnyAsync(c => c.Code == code && c.IsActive, ct);

    // ───────── Currency ─────────

    public Task<CurrencyDto?> GetCurrencyAsync(string code, CancellationToken ct)
        => _db.Set<Currency>()
            .AsNoTracking()
            .Where(c => c.Code == code)
            .Select(c => new CurrencyDto(
                c.Code,
                c.NameEn,
                c.Symbol,
                c.SymbolPosition,
                c.DecimalPlaces,
                c.ThousandSep,
                c.DecimalSep,
                c.IsActive,
                c.RateToUsd,
                c.RateUpdatedAt))
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<CurrencyDto>> ListActiveCurrenciesAsync(CancellationToken ct)
        => await _db.Set<Currency>()
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.Code)
            .Select(c => new CurrencyDto(
                c.Code,
                c.NameEn,
                c.Symbol,
                c.SymbolPosition,
                c.DecimalPlaces,
                c.ThousandSep,
                c.DecimalSep,
                c.IsActive,
                c.RateToUsd,
                c.RateUpdatedAt))
            .ToListAsync(ct);

    public Task<decimal?> GetRateToUsdAsync(string code, CancellationToken ct)
        => _db.Set<Currency>()
            .AsNoTracking()
            .Where(c => c.Code == code)
            .Select(c => c.RateToUsd)
            .FirstOrDefaultAsync(ct);

    // ───────── Language ─────────

    public Task<LanguageDto?> GetLanguageAsync(string code, CancellationToken ct)
        => _db.Set<Language>()
            .AsNoTracking()
            .Where(l => l.Code == code)
            .Select(l => new LanguageDto(
                l.Code,
                l.NameEn,
                l.NativeName,
                l.IsRtl,
                l.IsActive,
                l.DisplayOrder))
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<LanguageDto>> ListActiveLanguagesAsync(CancellationToken ct)
        => await _db.Set<Language>()
            .AsNoTracking()
            .Where(l => l.IsActive)
            .OrderBy(l => l.DisplayOrder)
            .Select(l => new LanguageDto(
                l.Code,
                l.NameEn,
                l.NativeName,
                l.IsRtl,
                l.IsActive,
                l.DisplayOrder))
            .ToListAsync(ct);

    public Task<bool> IsValidActiveLanguageCodeAsync(string code, CancellationToken ct)
        => _db.Set<Language>()
            .AsNoTracking()
            .AnyAsync(l => l.Code == code && l.IsActive, ct);

    // ───────── Location ─────────

    public Task<LocationDto?> GetLocationAsync(int locationId, CancellationToken ct)
        => _db.Set<Location>()
            .AsNoTracking()
            .Where(l => l.Id == locationId)
            .Select(l => new LocationDto(
                l.Id,
                l.ParentId,
                (Shared.Contracts.Catalog.LocationLevel)l.Level,
                l.CountryCode.Value,
                l.Code,
                l.Slug,
                l.Path,
                l.Name,
                l.NativeName,
                l.Population,
                l.IsActive,
                l.DisplayOrder))
            .FirstOrDefaultAsync(ct);

    public Task<bool> IsValidLocationIdAsync(int locationId, CancellationToken ct)
        => _db.Set<Location>()
            .AsNoTracking()
            .AnyAsync(l => l.Id == locationId && l.IsActive, ct);

    // ───────── CertificationType ─────────

    public Task<CertificationTypeDto?> GetCertificationTypeAsync(string code, CancellationToken ct)
        => _db.Set<CertificationType>()
            .AsNoTracking()
            .Where(ct1 => ct1.Code == code)
            .Select(ct1 => new CertificationTypeDto(
                ct1.Code,
                ct1.NameTranslations,
                ct1.DescriptionTranslations,
                ct1.IsActive,
                ct1.DisplayOrder))
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<CertificationTypeDto>> ListActiveCertificationTypesAsync(CancellationToken ct)
        => await _db.Set<CertificationType>()
            .AsNoTracking()
            .Where(ct1 => ct1.IsActive)
            .OrderBy(ct1 => ct1.DisplayOrder)
            .Select(ct1 => new CertificationTypeDto(
                ct1.Code,
                ct1.NameTranslations,
                ct1.DescriptionTranslations,
                ct1.IsActive,
                ct1.DisplayOrder))
            .ToListAsync(ct);

    public Task<bool> IsValidCertificationTypeIdAsync(int id, CancellationToken ct)
        => _db.Set<CertificationType>()
            .AsNoTracking()
            .AnyAsync(ct1 => ct1.Id == id && ct1.IsActive, ct);

    // ───────── Brand ─────────

    public async Task<BrandDto?> GetBrandByIdAsync(Guid brandId, CancellationToken ct)
    {
        // (A.2) two-step: BrandCategory.Category navigation YOK (S9). Step 1: brand + categoryIds.
        var brand = await _db.Set<Brand>()
            .AsNoTracking()
            .Where(b => b.Id == brandId)
            .Select(b => new
            {
                b.Id,
                b.Slug,
                b.Name,
                b.Description,
                b.LogoUrl,
                b.Website,
                b.OriginCountry,
                b.Status,
                b.IsActive,
                b.DisplayOrder,
                CategoryIds = b.Categories.Select(bc => bc.CategoryId).ToList()
            })
            .FirstOrDefaultAsync(ct);

        if (brand == null) return null;

        // Step 2: category code lookup
        var codes = await _db.Set<Category>()
            .AsNoTracking()
            .Where(c => brand.CategoryIds.Contains(c.Id))
            .Select(c => c.Code)
            .ToListAsync(ct);

        return new BrandDto(
            brand.Id,
            brand.Slug,
            brand.Name,
            brand.Description,
            brand.LogoUrl,
            brand.Website,
            brand.OriginCountry == null ? null : brand.OriginCountry.Value.Value,
            (Shared.Contracts.Catalog.BrandStatus)brand.Status,
            brand.IsActive,
            brand.DisplayOrder,
            codes);
    }

    public Task<bool> IsValidBrandForCategoryAsync(Guid brandId, int categoryId, CancellationToken ct)
        => _db.Set<Brand>()
            .AsNoTracking()
            .AnyAsync(b => b.Id == brandId
                           && b.IsActive
                           && b.Status == LivestockTrading.Catalog.Domain.Aggregates.BrandStatus.Approved
                           && b.Categories.Any(bc => bc.CategoryId == categoryId), ct);

    // ───────── Breed ─────────

    public async Task<BreedDto?> GetBreedByCodeAsync(string code, CancellationToken ct)
    {
        // (A) Select-in-query — correlated sub-query CategoryCode resolve (denenir).
        var dto = await _db.Set<Breed>()
            .AsNoTracking()
            .Where(b => b.Code == code)
            .Select(b => new BreedDto(
                b.Code,
                _db.Set<Category>().Where(c => c.Id == b.CategoryId).Select(c => c.Code).FirstOrDefault()!,
                b.OriginCountryCode,
                b.IsActive,
                b.DisplayOrder,
                b.Name,
                b.Description))
            .FirstOrDefaultAsync(ct);
        return dto;
    }

    public async Task<IReadOnlyList<BreedDto>> ListBreedsByCategoryAsync(string categoryCode, CancellationToken ct)
    {
        // (A) Single Category.Code → Id lookup, sonra Breed.CategoryId match. İki sorgu ama
        // birinci tek-değer (Category id), correlated sub-query yerine güvenli pragma.
        var categoryId = await _db.Set<Category>()
            .AsNoTracking()
            .Where(c => c.Code == categoryCode)
            .Select(c => (int?)c.Id)
            .FirstOrDefaultAsync(ct);

        if (categoryId == null)
            return Array.Empty<BreedDto>();

        return await _db.Set<Breed>()
            .AsNoTracking()
            .Where(b => b.CategoryId == categoryId.Value && b.IsActive)
            .OrderBy(b => b.DisplayOrder)
            .Select(b => new BreedDto(
                b.Code,
                categoryCode,
                b.OriginCountryCode,
                b.IsActive,
                b.DisplayOrder,
                b.Name,
                b.Description))
            .ToListAsync(ct);
    }

    public Task<bool> IsValidActiveBreedCodeAsync(string code, string? requiredCategoryCode, CancellationToken ct)
    {
        var q = _db.Set<Breed>().AsNoTracking().Where(b => b.Code == code && b.IsActive);
        if (requiredCategoryCode != null)
        {
            q = q.Where(b => _db.Set<Category>()
                .Any(c => c.Id == b.CategoryId && c.Code == requiredCategoryCode));
        }
        return q.AnyAsync(ct);
    }

    // ───────── Category ─────────

    public async Task<CategoryDto?> GetCategoryByCodeAsync(string code, CancellationToken ct)
    {
        // (A) correlated sub-query ParentCode + Attributes collection projection.
        var dto = await _db.Set<Category>()
            .AsNoTracking()
            .Where(c => c.Code == code)
            .Select(c => new CategoryDto(
                c.Code,
                c.ParentId == null
                    ? null
                    : _db.Set<Category>().Where(p => p.Id == c.ParentId).Select(p => p.Code).FirstOrDefault(),
                c.Level,
                c.DisplayOrder,
                c.IsActive,
                c.IconKey,
                c.Name,
                c.Description,
                c.Attributes.OrderBy(a => a.DisplayOrder).Select(a => new CategoryAttributeDto(
                    a.Key,
                    (Shared.Contracts.Catalog.AttributeValueType)a.ValueType,
                    a.Required,
                    a.Filterable,
                    a.Unit,
                    a.OptionsJson,
                    a.Label,
                    a.HelpText,
                    a.DisplayOrder)).ToList()))
            .FirstOrDefaultAsync(ct);
        return dto;
    }

    public Task<bool> IsValidActiveCategoryCodeAsync(string code, int? requiredLevel, CancellationToken ct)
        => _db.Set<Category>()
            .AsNoTracking()
            .AnyAsync(c => c.Code == code
                           && c.IsActive
                           && (requiredLevel == null || c.Level == requiredLevel.Value), ct);

    public async Task<CategoryTreeDto> GetCategoryTreeAsync(CancellationToken ct)
    {
        // (B) ToListAsync + in-memory tree build. Children navigation YOK (S8); depth-2
        // invariant (Category.cs:21 Level 1/2 only). Tek pas server-side fetch + O(N)
        // in-memory grouping.
        var all = await _db.Set<Category>()
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new
            {
                c.Id,
                c.Code,
                c.Name,
                c.Level,
                c.DisplayOrder,
                c.IsActive,
                c.ParentId
            })
            .ToListAsync(ct);

        var roots = all.Where(c => c.ParentId == null).ToList();

        IReadOnlyList<CategoryTreeDto> BuildChildren(int parentId)
            => all.Where(c => c.ParentId == parentId)
                .Select(c => new CategoryTreeDto(
                    c.Code,
                    c.Name,
                    c.Level,
                    c.DisplayOrder,
                    c.IsActive,
                    Array.Empty<CategoryTreeDto>()))
                .ToList();

        // Doc-literal: GetCategoryTreeAsync döner tek CategoryTreeDto. Depth-2 invariant +
        // çoklu root → sentinel kök "" Code, "" Name (Translations.Empty), Level 0 kabuk.
        var rootDtos = roots.Select(r => new CategoryTreeDto(
            r.Code,
            r.Name,
            r.Level,
            r.DisplayOrder,
            r.IsActive,
            BuildChildren(r.Id))).ToList();

        return new CategoryTreeDto(
            string.Empty,
            Shared.ValueObjects.Translations.Empty,
            0,
            0,
            true,
            rootDtos);
    }
}
