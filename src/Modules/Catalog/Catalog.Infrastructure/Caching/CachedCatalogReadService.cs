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
/// Blok-A (W3.6.A.1) Country trio cache-aside full impl + Blok-B (W3.6.A.2) kalan 19 metot
/// cache-aside complete = 22/22 metot. F-S51 (Translations STJ-deser) W3.6.A.1.5'te
/// JsonConverter ile cozuldu — Category/Breed/CertificationType/Brand/Location metotlari
/// Redis-safe (Translations field'lari converter ile ser/deser).
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

    // ───────── Currency ─────────

    public async Task<CurrencyDto?> GetCurrencyAsync(string code, CancellationToken ct)
    {
        var key = $"livestock:catalog:currency:{code}";
        var cached = await _cache.GetAsync<CurrencyDto>(key, ct);
        if (cached is not null)
            return cached;

        var result = await _inner.GetCurrencyAsync(code, ct);
        if (result is not null)
            await _cache.SetAsync(key, result, CacheTtl.ReferenceData, ct);
        return result;
    }

    public async Task<IReadOnlyList<CurrencyDto>> ListActiveCurrenciesAsync(CancellationToken ct)
    {
        const string key = "livestock:catalog:currency:list:active";
        var cached = await _cache.GetAsync<IReadOnlyList<CurrencyDto>>(key, ct);
        if (cached is not null)
            return cached;

        var result = await _inner.ListActiveCurrenciesAsync(ct);
        await _cache.SetAsync(key, result, CacheTtl.ReferenceData, ct);
        return result;
    }

    public async Task<decimal?> GetRateToUsdAsync(string code, CancellationToken ct)
    {
        var key = $"livestock:catalog:currency:rate:{code}";
        // decimal? value type — cache-miss = default(decimal?) = null; hit HasValue=true.
        var cached = await _cache.GetAsync<decimal?>(key, ct);
        if (cached.HasValue)
            return cached;

        var result = await _inner.GetRateToUsdAsync(code, ct);
        if (result.HasValue)
            await _cache.SetAsync(key, result, CacheTtl.CurrencyRate, ct);
        return result;
    }

    // ───────── Language ─────────

    public async Task<LanguageDto?> GetLanguageAsync(string code, CancellationToken ct)
    {
        var key = $"livestock:catalog:language:{code}";
        var cached = await _cache.GetAsync<LanguageDto>(key, ct);
        if (cached is not null)
            return cached;

        var result = await _inner.GetLanguageAsync(code, ct);
        if (result is not null)
            await _cache.SetAsync(key, result, CacheTtl.ReferenceData, ct);
        return result;
    }

    public async Task<IReadOnlyList<LanguageDto>> ListActiveLanguagesAsync(CancellationToken ct)
    {
        const string key = "livestock:catalog:language:list:active";
        var cached = await _cache.GetAsync<IReadOnlyList<LanguageDto>>(key, ct);
        if (cached is not null)
            return cached;

        var result = await _inner.ListActiveLanguagesAsync(ct);
        await _cache.SetAsync(key, result, CacheTtl.ReferenceData, ct);
        return result;
    }

    public async Task<bool> IsValidActiveLanguageCodeAsync(string code, CancellationToken ct)
    {
        var key = $"livestock:catalog:language:valid:{code}";
        var cached = await _cache.GetAsync<bool?>(key, ct);
        if (cached.HasValue)
            return cached.Value;

        var result = await _inner.IsValidActiveLanguageCodeAsync(code, ct);
        await _cache.SetAsync<bool?>(key, result, CacheTtl.Validator, ct);
        return result;
    }

    // ───────── Category (Translations icerir — W3.6.A.1.5 converter ile Redis-safe) ─────────

    public async Task<CategoryDto?> GetCategoryByCodeAsync(string code, CancellationToken ct)
    {
        var key = $"livestock:catalog:category:{code}";
        var cached = await _cache.GetAsync<CategoryDto>(key, ct);
        if (cached is not null)
            return cached;

        var result = await _inner.GetCategoryByCodeAsync(code, ct);
        if (result is not null)
            await _cache.SetAsync(key, result, CacheTtl.Category, ct);
        return result;
    }

    public async Task<CategoryTreeDto> GetCategoryTreeAsync(CancellationToken ct)
    {
        const string key = "livestock:catalog:category:tree";
        var cached = await _cache.GetAsync<CategoryTreeDto>(key, ct);
        if (cached is not null)
            return cached;

        var result = await _inner.GetCategoryTreeAsync(ct);
        // CategoryTreeDto non-nullable; inner her zaman sentinel kok ile tree doner (Domain invariant).
        await _cache.SetAsync(key, result, CacheTtl.Category, ct);
        return result;
    }

    public async Task<bool> IsValidActiveCategoryCodeAsync(string code, int? requiredLevel, CancellationToken ct)
    {
        var levelToken = requiredLevel.HasValue ? $"lvl-{requiredLevel.Value}" : "lvl-any";
        var key = $"livestock:catalog:category:valid:{code}:{levelToken}";
        var cached = await _cache.GetAsync<bool?>(key, ct);
        if (cached.HasValue)
            return cached.Value;

        var result = await _inner.IsValidActiveCategoryCodeAsync(code, requiredLevel, ct);
        await _cache.SetAsync<bool?>(key, result, CacheTtl.Validator, ct);
        return result;
    }

    // ───────── Breed (Translations icerir — W3.6.A.1.5 converter ile Redis-safe) ─────────

    public async Task<BreedDto?> GetBreedByCodeAsync(string code, CancellationToken ct)
    {
        var key = $"livestock:catalog:breed:{code}";
        var cached = await _cache.GetAsync<BreedDto>(key, ct);
        if (cached is not null)
            return cached;

        var result = await _inner.GetBreedByCodeAsync(code, ct);
        if (result is not null)
            await _cache.SetAsync(key, result, CacheTtl.Breed, ct);
        return result;
    }

    public async Task<IReadOnlyList<BreedDto>> ListBreedsByCategoryAsync(string categoryCode, CancellationToken ct)
    {
        var key = $"livestock:catalog:breed:list:by-category:{categoryCode}";
        var cached = await _cache.GetAsync<IReadOnlyList<BreedDto>>(key, ct);
        if (cached is not null)
            return cached;

        var result = await _inner.ListBreedsByCategoryAsync(categoryCode, ct);
        await _cache.SetAsync(key, result, CacheTtl.Breed, ct);
        return result;
    }

    public async Task<bool> IsValidActiveBreedCodeAsync(string code, string? requiredCategoryCode, CancellationToken ct)
    {
        var catToken = requiredCategoryCode is not null ? $"cat-{requiredCategoryCode}" : "cat-any";
        var key = $"livestock:catalog:breed:valid:{code}:{catToken}";
        var cached = await _cache.GetAsync<bool?>(key, ct);
        if (cached.HasValue)
            return cached.Value;

        var result = await _inner.IsValidActiveBreedCodeAsync(code, requiredCategoryCode, ct);
        await _cache.SetAsync<bool?>(key, result, CacheTtl.Validator, ct);
        return result;
    }

    // ───────── Brand (BrandDto Translations icerir — W3.6.A.1.5 converter ile Redis-safe) ─────────

    public async Task<BrandDto?> GetBrandByIdAsync(Guid brandId, CancellationToken ct)
    {
        var key = $"livestock:catalog:brand:{brandId}";
        var cached = await _cache.GetAsync<BrandDto>(key, ct);
        if (cached is not null)
            return cached;

        var result = await _inner.GetBrandByIdAsync(brandId, ct);
        if (result is not null)
            await _cache.SetAsync(key, result, CacheTtl.Brand, ct);
        return result;
    }

    public async Task<bool> IsValidBrandForCategoryAsync(Guid brandId, int categoryId, CancellationToken ct)
    {
        var key = $"livestock:catalog:brand:valid-for-category:{brandId}:{categoryId}";
        var cached = await _cache.GetAsync<bool?>(key, ct);
        if (cached.HasValue)
            return cached.Value;

        var result = await _inner.IsValidBrandForCategoryAsync(brandId, categoryId, ct);
        await _cache.SetAsync<bool?>(key, result, CacheTtl.Validator, ct);
        return result;
    }

    // ───────── Location (LocationDto Translations icerir — W3.6.A.1.5 converter ile Redis-safe) ─────────

    public async Task<LocationDto?> GetLocationAsync(int locationId, CancellationToken ct)
    {
        var key = $"livestock:catalog:location:{locationId}";
        var cached = await _cache.GetAsync<LocationDto>(key, ct);
        if (cached is not null)
            return cached;

        var result = await _inner.GetLocationAsync(locationId, ct);
        if (result is not null)
            await _cache.SetAsync(key, result, CacheTtl.ReferenceData, ct);
        return result;
    }

    public async Task<bool> IsValidLocationIdAsync(int locationId, CancellationToken ct)
    {
        var key = $"livestock:catalog:location:valid:{locationId}";
        var cached = await _cache.GetAsync<bool?>(key, ct);
        if (cached.HasValue)
            return cached.Value;

        var result = await _inner.IsValidLocationIdAsync(locationId, ct);
        await _cache.SetAsync<bool?>(key, result, CacheTtl.Validator, ct);
        return result;
    }

    // ───────── CertificationType (Translations icerir — W3.6.A.1.5 converter ile Redis-safe) ─────────

    public async Task<CertificationTypeDto?> GetCertificationTypeAsync(string code, CancellationToken ct)
    {
        var key = $"livestock:catalog:cert-type:{code}";
        var cached = await _cache.GetAsync<CertificationTypeDto>(key, ct);
        if (cached is not null)
            return cached;

        var result = await _inner.GetCertificationTypeAsync(code, ct);
        if (result is not null)
            await _cache.SetAsync(key, result, CacheTtl.CertificationType, ct);
        return result;
    }

    public async Task<IReadOnlyList<CertificationTypeDto>> ListActiveCertificationTypesAsync(CancellationToken ct)
    {
        const string key = "livestock:catalog:cert-type:list:active";
        var cached = await _cache.GetAsync<IReadOnlyList<CertificationTypeDto>>(key, ct);
        if (cached is not null)
            return cached;

        var result = await _inner.ListActiveCertificationTypesAsync(ct);
        await _cache.SetAsync(key, result, CacheTtl.CertificationType, ct);
        return result;
    }

    public async Task<bool> IsValidCertificationTypeIdAsync(int id, CancellationToken ct)
    {
        var key = $"livestock:catalog:cert-type:valid:{id}";
        var cached = await _cache.GetAsync<bool?>(key, ct);
        if (cached.HasValue)
            return cached.Value;

        var result = await _inner.IsValidCertificationTypeIdAsync(id, ct);
        await _cache.SetAsync<bool?>(key, result, CacheTtl.Validator, ct);
        return result;
    }
}
