namespace Shared.Contracts.Catalog.Admin;

using Shared.Results;

/// <summary>
/// Cross-modül admin yazma (Karar 3d). 05-catalog §5:542-585 birebir; SetLocationCentroidAsync Faz 1
/// HARİÇ (NTS C3); BorderRule tam tanımlı, impl Faz 2 placeholder. Id route'tan, payload body DTO.
/// </summary>
public interface IAdminCatalogCommands
{
    // Category
    Task<Result<int>> CreateCategoryAsync(CreateCategoryDto dto, Guid actorAdminId, CancellationToken ct);
    Task<Result> UpdateCategoryAsync(int id, UpdateCategoryDto dto, Guid actorAdminId, CancellationToken ct);
    Task<Result> DeactivateCategoryAsync(int id, Guid actorAdminId, CancellationToken ct);

    // Category Attributes (v2)
    Task<Result> AddCategoryAttributeAsync(int categoryId, AttributeDto dto, Guid actorAdminId, CancellationToken ct);
    Task<Result> RemoveCategoryAttributeAsync(int categoryId, Guid attributeId, Guid actorAdminId, CancellationToken ct);

    // Breed
    Task<Result<int>> CreateBreedAsync(CreateBreedDto dto, Guid actorAdminId, CancellationToken ct);
    Task<Result> UpdateBreedAsync(int id, UpdateBreedDto dto, Guid actorAdminId, CancellationToken ct);
    Task<Result> DeactivateBreedAsync(int id, Guid actorAdminId, CancellationToken ct);

    // Brand (v2)
    Task<Result<Guid>> CreateBrandAsync(CreateBrandDto dto, Guid actorAdminId, CancellationToken ct);
    Task<Result> ApproveBrandAsync(Guid brandId, Guid actorAdminId, CancellationToken ct);
    Task<Result> RejectBrandAsync(Guid brandId, string reason, Guid actorAdminId, CancellationToken ct);
    Task<Result> DeactivateBrandAsync(Guid brandId, Guid actorAdminId, CancellationToken ct);
    Task<Result<BulkImportResult>> BulkImportBrandsAsync(Stream csvFile, Guid actorAdminId, CancellationToken ct);

    // Location (v2) — SetLocationCentroidAsync Faz 1 HARİÇ (NTS C3)
    Task<Result<int>> CreateLocationAsync(CreateLocationDto dto, Guid actorAdminId, CancellationToken ct);
    Task<Result> UpdateLocationAsync(int id, UpdateLocationDto dto, Guid actorAdminId, CancellationToken ct);

    // BorderRule (v2) — tam tanımlı, impl Faz 2
    Task<Result<Guid>> CreateBorderRuleAsync(CreateBorderRuleDto dto, Guid actorAdminId, CancellationToken ct);
    Task<Result> UpdateBorderRuleAsync(Guid id, UpdateBorderRuleDto dto, Guid actorAdminId, CancellationToken ct);
    Task<Result> DeactivateBorderRuleAsync(Guid id, Guid actorAdminId, CancellationToken ct);

    // CertificationType
    Task<Result<int>> CreateCertificationTypeAsync(CreateCertificationTypeDto dto, Guid actorAdminId, CancellationToken ct);
    Task<Result> UpdateCertificationTypeAsync(int id, UpdateCertificationTypeDto dto, Guid actorAdminId, CancellationToken ct);
    Task<Result> DeactivateCertificationTypeAsync(int id, Guid actorAdminId, CancellationToken ct);

    // Country/Currency/Language — toggle + rate refresh (DTO'suz, doc-literal §5:580-585)
    Task<Result> ToggleCountryActiveAsync(int id, bool active, Guid actorAdminId, CancellationToken ct);
    Task<Result> ToggleCurrencyActiveAsync(int id, bool active, Guid actorAdminId, CancellationToken ct);
    Task<Result> ToggleLanguageActiveAsync(int id, bool active, Guid actorAdminId, CancellationToken ct);
    Task<Result> RefreshExchangeRatesAsync(Guid actorAdminId, CancellationToken ct);
}
