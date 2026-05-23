using LivestockTrading.Catalog.Application.Abstractions;
using Shared.Contracts.Catalog;

namespace LivestockTrading.Catalog.Infrastructure.Caching;

/// <summary>
/// ICatalogReadService cache-aside decorator (Scrutor Decorate, lifetime-preserve Scoped).
/// 22 metot read-through cache; key prefix doc-literal `livestock:catalog:` (05-catalog.md:620).
/// TTL 7 named bucket (CacheTtl) × doc 05-catalog.md:604-618. Faz 1 TTL-only invalidation
/// (event-driven invalidation Faz 2, doc:622-623). Negative caching YOK — inner null donerse
/// cache SET ETME, sonraki cagri yine inner'a iner. Validator metotlari `bool?` boxing ile
/// cache-miss-vs-false ayrimi yapar (default(bool) = false false-positive olusturmasin).
///
/// Blok-A scope: Country trio (3 metot) cache-aside full impl; kalan 19 metot inner
/// passthrough — Blok-B'de tek tek cache-wrap'lenecek. F-S51 (Translations STJ-deser
/// uyumsuzlugu) W3.6.A.1.5 sub-batch'te JsonConverter pattern ile cozulecek; Country trio
/// Translations icermez (primitive only DTO), Blok-A risk-free.
/// </summary>
internal sealed class CachedCatalogReadService : ICatalogReadService
{
    private readonly ICatalogReadService _inner;
    private readonly ICacheService _cache;

    public CachedCatalogReadService(ICatalogReadService inner, ICacheService cache)
    {
        _inner = inner;
        _cache = cache;
    }

    // ───────── Country (Blok-A: cache-aside full impl) ─────────

    public async Task<CountryDto?> GetCountryAsync(string code, CancellationToken ct)
    {
        var key = $"livestock:catalog:country:{code}";
        var cached = await _cache.GetAsync<CountryDto>(key, ct);
        if (cached is not null)
            return cached;

        var result = await _inner.GetCountryAsync(code, ct);
        if (result is not null)
            await _cache.SetAsync(key, result, CacheTtl.ReferenceData, ct);
        return result;
    }

    public async Task<IReadOnlyList<CountryDto>> ListActiveCountriesAsync(CancellationToken ct)
    {
        const string key = "livestock:catalog:country:list:active";
        var cached = await _cache.GetAsync<IReadOnlyList<CountryDto>>(key, ct);
        if (cached is not null)
            return cached;

        var result = await _inner.ListActiveCountriesAsync(ct);
        // Empty list null degil — gecerli sonuc, cache'le. Negative caching sadece null icin.
        await _cache.SetAsync(key, result, CacheTtl.ReferenceData, ct);
        return result;
    }

    public async Task<bool> IsValidActiveCountryCodeAsync(string code, CancellationToken ct)
    {
        var key = $"livestock:catalog:country:valid:{code}";
        // bool? boxing: cache-miss = null (default(bool?)), hit = true/false.
        // default(bool) = false false-positive uretirdi.
        var cached = await _cache.GetAsync<bool?>(key, ct);
        if (cached.HasValue)
            return cached.Value;

        var result = await _inner.IsValidActiveCountryCodeAsync(code, ct);
        await _cache.SetAsync<bool?>(key, result, CacheTtl.Validator, ct);
        return result;
    }

    // ───────── Currency (Blok-B: passthrough) ─────────

    public Task<CurrencyDto?> GetCurrencyAsync(string code, CancellationToken ct)
        => _inner.GetCurrencyAsync(code, ct);

    public Task<IReadOnlyList<CurrencyDto>> ListActiveCurrenciesAsync(CancellationToken ct)
        => _inner.ListActiveCurrenciesAsync(ct);

    public Task<decimal?> GetRateToUsdAsync(string code, CancellationToken ct)
        => _inner.GetRateToUsdAsync(code, ct);

    // ───────── Language (Blok-B: passthrough) ─────────

    public Task<LanguageDto?> GetLanguageAsync(string code, CancellationToken ct)
        => _inner.GetLanguageAsync(code, ct);

    public Task<IReadOnlyList<LanguageDto>> ListActiveLanguagesAsync(CancellationToken ct)
        => _inner.ListActiveLanguagesAsync(ct);

    public Task<bool> IsValidActiveLanguageCodeAsync(string code, CancellationToken ct)
        => _inner.IsValidActiveLanguageCodeAsync(code, ct);

    // ───────── Category (Blok-B: passthrough; F-S51 Translations etkilenir) ─────────

    public Task<CategoryDto?> GetCategoryByCodeAsync(string code, CancellationToken ct)
        => _inner.GetCategoryByCodeAsync(code, ct);

    public Task<CategoryTreeDto> GetCategoryTreeAsync(CancellationToken ct)
        => _inner.GetCategoryTreeAsync(ct);

    public Task<bool> IsValidActiveCategoryCodeAsync(string code, int? requiredLevel, CancellationToken ct)
        => _inner.IsValidActiveCategoryCodeAsync(code, requiredLevel, ct);

    // ───────── Breed (Blok-B: passthrough; F-S51 Translations etkilenir) ─────────

    public Task<BreedDto?> GetBreedByCodeAsync(string code, CancellationToken ct)
        => _inner.GetBreedByCodeAsync(code, ct);

    public Task<IReadOnlyList<BreedDto>> ListBreedsByCategoryAsync(string categoryCode, CancellationToken ct)
        => _inner.ListBreedsByCategoryAsync(categoryCode, ct);

    public Task<bool> IsValidActiveBreedCodeAsync(string code, string? requiredCategoryCode, CancellationToken ct)
        => _inner.IsValidActiveBreedCodeAsync(code, requiredCategoryCode, ct);

    // ───────── Brand (Blok-B: passthrough) ─────────

    public Task<BrandDto?> GetBrandByIdAsync(Guid brandId, CancellationToken ct)
        => _inner.GetBrandByIdAsync(brandId, ct);

    public Task<bool> IsValidBrandForCategoryAsync(Guid brandId, int categoryId, CancellationToken ct)
        => _inner.IsValidBrandForCategoryAsync(brandId, categoryId, ct);

    // ───────── Location (Blok-B: passthrough) ─────────

    public Task<LocationDto?> GetLocationAsync(int locationId, CancellationToken ct)
        => _inner.GetLocationAsync(locationId, ct);

    public Task<bool> IsValidLocationIdAsync(int locationId, CancellationToken ct)
        => _inner.IsValidLocationIdAsync(locationId, ct);

    // ───────── CertificationType (Blok-B: passthrough; F-S51 Translations etkilenir) ─────────

    public Task<CertificationTypeDto?> GetCertificationTypeAsync(string code, CancellationToken ct)
        => _inner.GetCertificationTypeAsync(code, ct);

    public Task<IReadOnlyList<CertificationTypeDto>> ListActiveCertificationTypesAsync(CancellationToken ct)
        => _inner.ListActiveCertificationTypesAsync(ct);

    public Task<bool> IsValidCertificationTypeIdAsync(int id, CancellationToken ct)
        => _inner.IsValidCertificationTypeIdAsync(id, ct);
}
