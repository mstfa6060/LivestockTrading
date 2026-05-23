using LivestockTrading.Catalog.Application.Abstractions;
using LivestockTrading.Catalog.Domain.Entities;

namespace LivestockTrading.Catalog.Infrastructure.Persistence.Repositories;

/// <summary>
/// IReferenceDataRepository impl — single concrete kapsayıcı 5 reference entity için
/// (Country/Currency/Language/CertificationType/Location). Port (IReferenceDataRepository.cs)
/// 10 metot: Get{X}ByIdAsync(int) + Add{X}Async({X}) × 5. Remove YOK (port doc: "FK risk
/// on reference data"; lifecycle KAYDET-14 §5 IAdminCatalogCommands'a delege). GetByCode
/// YOK (read W3.5). 5× boilerplate (Generic&lt;T&gt; Wave 4 retro adayı; sade yapı tercih).
/// </summary>
public sealed class ReferenceDataRepository : IReferenceDataRepository
{
    private readonly CatalogDbContext _db;

    public ReferenceDataRepository(CatalogDbContext db) => _db = db;

    // Country
    public Task<Country?> GetCountryByIdAsync(int id, CancellationToken ct)
        => _db.Set<Country>().FindAsync(new object[] { id }, ct).AsTask();

    public Task AddCountryAsync(Country entity, CancellationToken ct)
    {
        _db.Set<Country>().Add(entity);
        return Task.CompletedTask;
    }

    // Currency
    public Task<Currency?> GetCurrencyByIdAsync(int id, CancellationToken ct)
        => _db.Set<Currency>().FindAsync(new object[] { id }, ct).AsTask();

    public Task AddCurrencyAsync(Currency entity, CancellationToken ct)
    {
        _db.Set<Currency>().Add(entity);
        return Task.CompletedTask;
    }

    // Language
    public Task<Language?> GetLanguageByIdAsync(int id, CancellationToken ct)
        => _db.Set<Language>().FindAsync(new object[] { id }, ct).AsTask();

    public Task AddLanguageAsync(Language entity, CancellationToken ct)
    {
        _db.Set<Language>().Add(entity);
        return Task.CompletedTask;
    }

    // CertificationType
    public Task<CertificationType?> GetCertificationTypeByIdAsync(int id, CancellationToken ct)
        => _db.Set<CertificationType>().FindAsync(new object[] { id }, ct).AsTask();

    public Task AddCertificationTypeAsync(CertificationType entity, CancellationToken ct)
    {
        _db.Set<CertificationType>().Add(entity);
        return Task.CompletedTask;
    }

    // Location
    public Task<Location?> GetLocationByIdAsync(int id, CancellationToken ct)
        => _db.Set<Location>().FindAsync(new object[] { id }, ct).AsTask();

    public Task AddLocationAsync(Location entity, CancellationToken ct)
    {
        _db.Set<Location>().Add(entity);
        return Task.CompletedTask;
    }
}
