namespace Shared.Contracts.Catalog;

/// <summary>
/// Cross-modül senkron okuma (Karar 3d). 05-catalog §5 birebir. Locale param YOK — DTO ham
/// Translations taşır, locale çözümü Api/consumer katmanında TranslationHelper.Resolve ile.
/// Impl: Catalog.Infrastructure.CatalogReadService (Redis cache + DbContext fallback).
/// </summary>
public interface ICatalogReadService
{
    // Country
    Task<CountryDto?> GetCountryAsync(string code, CancellationToken ct);
    Task<IReadOnlyList<CountryDto>> ListActiveCountriesAsync(CancellationToken ct);
    Task<bool> IsValidActiveCountryCodeAsync(string code, CancellationToken ct);

    // Currency
    Task<CurrencyDto?> GetCurrencyAsync(string code, CancellationToken ct);
    Task<IReadOnlyList<CurrencyDto>> ListActiveCurrenciesAsync(CancellationToken ct);
    Task<decimal?> GetRateToUsdAsync(string code, CancellationToken ct);

    // Language
    Task<LanguageDto?> GetLanguageAsync(string code, CancellationToken ct);
    Task<IReadOnlyList<LanguageDto>> ListActiveLanguagesAsync(CancellationToken ct);
    Task<bool> IsValidActiveLanguageCodeAsync(string code, CancellationToken ct);

    // Category
    Task<CategoryDto?> GetCategoryByCodeAsync(string code, CancellationToken ct);
    Task<CategoryTreeDto> GetCategoryTreeAsync(CancellationToken ct);
    Task<bool> IsValidActiveCategoryCodeAsync(string code, int? requiredLevel, CancellationToken ct);

    // Breed
    Task<BreedDto?> GetBreedByCodeAsync(string code, CancellationToken ct);
    Task<IReadOnlyList<BreedDto>> ListBreedsByCategoryAsync(string categoryCode, CancellationToken ct);
    Task<bool> IsValidActiveBreedCodeAsync(string code, string? requiredCategoryCode, CancellationToken ct);

    // Brand (v2)
    Task<BrandDto?> GetBrandByIdAsync(Guid brandId, CancellationToken ct);
    Task<bool> IsValidBrandForCategoryAsync(Guid brandId, int categoryId, CancellationToken ct);

    // Location (v2)
    Task<LocationDto?> GetLocationAsync(int locationId, CancellationToken ct);
    Task<bool> IsValidLocationIdAsync(int locationId, CancellationToken ct);

    // CertificationType
    Task<CertificationTypeDto?> GetCertificationTypeAsync(string code, CancellationToken ct);
    Task<IReadOnlyList<CertificationTypeDto>> ListActiveCertificationTypesAsync(CancellationToken ct);
    Task<bool> IsValidCertificationTypeIdAsync(int id, CancellationToken ct);
}
