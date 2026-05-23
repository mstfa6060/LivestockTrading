namespace LivestockTrading.Catalog.Infrastructure.Caching;

/// <summary>
/// Catalog cache TTL constants — 05-catalog.md:604-618 doc-literal grounded.
/// 7 named bucket × 22 metot mapping (CachedCatalogReadService). Faz 1 TTL-only
/// invalidation; event-driven invalidation Faz 2 (doc:622-623).
/// </summary>
internal static class CacheTtl
{
    /// <summary>1h: Country, Currency, Language, Location list/single. Doc 05-catalog.md:606,607,609,617.</summary>
    public static readonly TimeSpan ReferenceData = TimeSpan.FromHours(1);

    /// <summary>30dk: CertificationType list/single. Admin yeni ekleyebilir. Doc 05-catalog.md:610.</summary>
    public static readonly TimeSpan CertificationType = TimeSpan.FromMinutes(30);

    /// <summary>5dk: Category tree/single. Admin daha sik edit eder. Doc 05-catalog.md:611-612.</summary>
    public static readonly TimeSpan Category = TimeSpan.FromMinutes(5);

    /// <summary>10dk: Brand single/by-category. Admin approval frequency dusuk. Doc 05-catalog.md:613-614.</summary>
    public static readonly TimeSpan Brand = TimeSpan.FromMinutes(10);

    /// <summary>5dk: Breed list (by category)/single. Doc 05-catalog.md:615-616.</summary>
    public static readonly TimeSpan Breed = TimeSpan.FromMinutes(5);

    /// <summary>1h: Currency rate (GetRateToUsdAsync). Daily job ile fresh garanti. Semantic ReferenceData'dan ayri — Faz 2 daily-job hook. Doc 05-catalog.md:608.</summary>
    public static readonly TimeSpan CurrencyRate = TimeSpan.FromHours(1);

    /// <summary>1dk: IsValid*Async validators (hot path — Listings.CreateListing). Doc 05-catalog.md:618.</summary>
    public static readonly TimeSpan Validator = TimeSpan.FromMinutes(1);
}
