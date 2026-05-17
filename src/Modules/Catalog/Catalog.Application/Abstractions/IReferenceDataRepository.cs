using LivestockTrading.Catalog.Domain.Entities;

namespace LivestockTrading.Catalog.Application.Abstractions;

/// <summary>
/// Reference entity repository — Get*ById and Add* operations only.
/// Remove operations are NOT included (FK risk on reference data).
/// IsActive lifecycle managed via §5 IAdminCatalogCommands Toggle/Deactivate
/// commands; Currency/Language/CertType entity-level Activate/Deactivate
/// methods pending W2.4/W2.5 Domain-amendment (KAYDET-14 — §4↔§5 doc gap,
/// Wave 7 doc-finalize backlog).
/// Implementation in Catalog.Infrastructure (Wave 3) — single concrete class
/// covering all reference entities via EF Core DbContext.
/// </summary>
public interface IReferenceDataRepository
{
    Task<Country?> GetCountryByIdAsync(int id, CancellationToken ct);
    Task<Currency?> GetCurrencyByIdAsync(int id, CancellationToken ct);
    Task<Language?> GetLanguageByIdAsync(int id, CancellationToken ct);
    Task<CertificationType?> GetCertificationTypeByIdAsync(int id, CancellationToken ct);
    Task<Location?> GetLocationByIdAsync(int id, CancellationToken ct);

    Task AddCountryAsync(Country entity, CancellationToken ct);
    Task AddCurrencyAsync(Currency entity, CancellationToken ct);
    Task AddLanguageAsync(Language entity, CancellationToken ct);
    Task AddCertificationTypeAsync(CertificationType entity, CancellationToken ct);
    Task AddLocationAsync(Location entity, CancellationToken ct);
}
