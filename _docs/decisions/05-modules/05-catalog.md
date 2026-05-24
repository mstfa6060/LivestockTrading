# Karar 5 / Catalog Modülü (v2)

**Status:** FINAL (v1 → v2 revize; v2 ile Brand + Location 5-level + BorderRule + CategoryAttribute + TCMB 3-tier rate provider eklendi)
**Wave:** 1 (foundation)

## İlişkili Kararlar

- **Üst:** [Karar 2 — Modül listesi](../02-modules-list.md), [Karar 3a — AR listesi](../03-domain-patterns.md#bölüm-1-aggregate-root-listesi-3a), [Karar 3e — VO inventory](../03-domain-patterns.md#bölüm-5-value-object-inventory-3e--24-vo)
- **Patch:** [Patch 3 — Catalog revize](../05-patch.md) — Brand AR + Location 5-level + BorderRule + CategoryAttribute child entity + TCMB rate provider
- **Frontend:** `frontend-api-inventory.md` Catalog section (Country/Currency/Language dropdown + Category tree + Breed filter)
- **Bağımlı modüller:** Tüm modüller (ICatalogReadService consumer); Listings (CategoryAttribute schema)

---

## 1. Modülün Rolü ve Sınırları

### Sahip Olduğu Domain

| Konsept | Sahiplik |
|---|---|
| ISO standard reference data (Country/Currency/Language) | Catalog (seed, admin-managed `is_active` flag) |
| Domain reference taxonomies (Category tree, Breed) | Catalog (admin CRUD + seed) |
| CertificationType catalog (12 seed + admin extend) | Catalog |
| Brand AR (admin-managed + seller-suggested approval) | Catalog (v2 yeni) |
| Location 5-level hierarchy (Country → Region → State → District → Neighborhood) | Catalog (v2 yeni, TR ~885K seed) |
| BorderRule (gümrük geçiş kuralları, Faz 2 feature) | Catalog (v2 yeni, schema-ready) |
| CategoryAttribute schema (kategori-spesifik dinamik attribute) | Catalog (v2 yeni; Listings için strict schema Faz 2) |
| Currency exchange rate (TCMB primary + ECB + exchangerate.host fallback) | Catalog (cron job daily 13:00 UTC) |
| Translation strings (Category/Breed/Brand/CertificationType name + description) | Catalog (admin yönetimli) |

### Sahip Olmadığı

| Konsept | Sahibi |
|---|---|
| "Bu kategoride kaç ilan var" | Listings (analytics) |
| ISO kod **anlamı** ile business kuralları | Yalnızca "kod valid mi?" doğrulamasını verir; iş kuralı tüketici modüllerin |
| Listing translation (per-listing content çevirisi) | Listings (AI translation worker) |
| Vehicle GPS / Real-time location | Carrier (Faz 2 GPS stream) |

---

## 2. Aggregate Roots

### `Category` AR

```csharp
public class Category
{
    public int Id { get; private set; }                          // INT PK
    public string Code { get; private set; }                     // "livestock-cattle/dairy-cow" — IMMUTABLE
    public int? ParentId { get; private set; }                   // self-ref FK
    public int Level { get; private set; }                       // 1 (top) | 2 (sub)
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; }
    public string? IconKey { get; private set; }
    public Translations Name { get; private set; }
    public Translations? Description { get; private set; }
    
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    
    // Child collection (v2 yeni)
    private readonly List<CategoryAttribute> _attributes = new();
    public IReadOnlyList<CategoryAttribute> Attributes => _attributes.AsReadOnly();
    
    // Factories
    public static Category CreateTopLevel(string code, Translations name, ...) 
    {
        // level = 1, parent_id = null
    }
    
    public static Category CreateSubcategory(string code, Category parent, Translations name, ...)
    {
        if (parent.Level != 1) 
            throw new DomainException("Subcategory parent must be top-level");
        // level = 2
    }
    
    public void UpdateTranslations(Translations name, Translations? description) { ... }
    public void UpdateDisplayOrder(int order) { ... }
    public void Activate() { /* DomainEvent: CategoryReactivated (Public) */ }
    public void Deactivate() { /* DomainEvent: CategoryDeactivated (Public) */ }
    
    // CategoryAttribute management (v2)
    public CategoryAttribute AddAttribute(
        string key, AttributeValueType valueType, bool required, bool filterable,
        string? unit, string? optionsJson, Translations label, Translations? help, int order)
    {
        if (_attributes.Any(a => a.Key == key))
            throw new DomainException($"Attribute key '{key}' already exists");
        if (valueType == AttributeValueType.Enum && string.IsNullOrWhiteSpace(optionsJson))
            throw new DomainException("Enum attribute requires options");
        
        var attr = new CategoryAttribute(Id, key, valueType, required, filterable, 
            unit, optionsJson, label, help, order);
        _attributes.Add(attr);
        Touch();
        return attr;
    }
    
    public void UpdateAttribute(Guid attrId, ...) { ... }
    public void RemoveAttribute(Guid attrId) { /* Listings reference uyarısı admin UI'da */ }
    
    // CRITICAL invariants:
    // - code immutable (create sonrası değişmez — cross-modül referans)
    // - parent_id immutable (move yasak — alt kategori başka tepe altına taşınamaz)
    // - Tree depth max 2 (Karar 4c kararı — 3. seviye Breed'e dönüşür)
}
```

### `Breed` AR

```csharp
public class Breed
{
    public int Id { get; private set; }                         // INT PK
    public string Code { get; private set; }                    // kebab-case — IMMUTABLE
    public int CategoryId { get; private set; }                 // IMMUTABLE — move yasak
    public string? OriginCountryCode { get; private set; }      // ISO 3166-1 alpha-2
    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }
    public Translations Name { get; private set; }
    public Translations? Description { get; private set; }
    
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    
    public static Breed Create(string code, Category category, Translations name, ...)
    {
        if (category.Level != 2)
            throw new DomainException("Breed parent category must be level-2");
        // ...
    }
    
    // CRITICAL: code + category_id immutable
    // Yanlış kategoride yaratıldıysa: deactivate + yenisini doğru kategoride yarat
    // (cross-modül Listing.BreedCode referansını korumak için)
}
```

### `Brand` AR (v2 YENİ)

```csharp
public class Brand
{
    public Guid Id { get; private set; }                        // Guid v7
    public string Slug { get; private set; }                    // unique kebab-case
    public Translations Name { get; private set; }
    public Translations? Description { get; private set; }
    public string? LogoUrl { get; private set; }                // IFileStorage URL
    public string? Website { get; private set; }
    public CountryCode? OriginCountry { get; private set; }
    
    public BrandStatus Status { get; private set; }
    public Guid? SuggestedByUserId { get; private set; }        // seller suggested ise
    public DateTimeOffset? SuggestedAt { get; private set; }
    public Guid? ReviewedByUserId { get; private set; }
    public DateTimeOffset? ReviewedAt { get; private set; }
    public string? RejectionReason { get; private set; }
    
    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    
    // Child collection — many-to-many junction
    private readonly List<BrandCategory> _categories = new();
    public IReadOnlyList<BrandCategory> Categories => _categories.AsReadOnly();
    
    // Factories
    public static Brand SuggestBySeller(
        string slug, Translations name, Guid sellerUserId, 
        IReadOnlyList<int> categoryIds)
    {
        var brand = new Brand
        {
            Id = Guid.CreateVersion7(),
            Slug = SlugHelper.Normalize(slug),
            Name = name,
            Status = BrandStatus.Suggested,
            SuggestedByUserId = sellerUserId,
            SuggestedAt = DateTimeOffset.UtcNow,
            IsActive = false,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };
        foreach (var cid in categoryIds)
            brand._categories.Add(new BrandCategory(brand.Id, cid));
        return brand;
        // Internal event: BrandSuggested
    }
    
    public static Brand CreateByAdmin(
        string slug, Translations name, Guid adminUserId,
        IReadOnlyList<int> categoryIds, string? logoUrl, string? website,
        CountryCode? originCountry)
    {
        // Status=Approved + IsActive=true direkt
        // Public event: BrandApproved
    }
    
    // Admin actions
    public void Approve(Guid adminUserId) { /* Suggested → Approved + Public event */ }
    public void Reject(Guid adminUserId, string reason) { /* Suggested → Rejected + Internal event */ }
    public void Deactivate(Guid adminUserId, string? reason) { /* Public event: BrandDeactivated */ }
    public void Reactivate(Guid adminUserId) { /* Public event: BrandReactivated */ }
    
    public void UpdateTranslations(Translations name, Translations? desc) { ... }
    public void UpdateLogo(string url) { ... }
    public void AddCategory(int categoryId) { ... }
    public void RemoveCategory(int categoryId) { ... }
}

public enum BrandStatus
{
    Suggested = 1,    // seller suggested, awaiting admin
    Approved = 2,
    Rejected = 3,
    Deactivated = 4
}
```

### `BorderRule` AR (v2 YENİ — Faz 2 Feature)

```csharp
public class BorderRule
{
    public Guid Id { get; private set; }
    public CountryCode FromCountry { get; private set; }
    public CountryCode ToCountry { get; private set; }
    public int? CategoryId { get; private set; }                // null = all categories
    public int? BreedId { get; private set; }                   // breed-spesifik
    public BorderRuleKind Kind { get; private set; }
    
    public string RestrictionsJson { get; private set; }
    /*  Örnek:
        {
          "requiredCertifications": ["health-cert", "vaccination-card", "export-approval"],
          "quarantineDays": 14,
          "additionalFeePercent": 0.08,
          "maxQuantity": 50,
          "documentationLanguage": "en",
          "expiryDays": 7
        }
    */
    
    public Translations Notes { get; private set; }
    public DateTimeOffset? EffectiveFrom { get; private set; }
    public DateTimeOffset? EffectiveUntil { get; private set; }
    public bool IsActive { get; private set; }
    
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    
    public static BorderRule Create(...) { ... }
    public void UpdateRestrictions(string json, Guid actorAdminId) { ... }
    public void Deactivate(Guid actorAdminId) { ... }
}

public enum BorderRuleKind
{
    Banned = 1,                  // ihracat yasak
    RequiresCert = 2,
    RequiresQuarantine = 3,
    AdditionalFee = 4,
    QuantityLimit = 5
}
```

**Faz 1:** Admin manuel CRUD; Listings yeni ilan'da **uyarı** seviyesi gösterir (block etmez).
**Faz 2:** Gümrük API entegrasyonu, listing approval'ında otomatik check, Public event'ler aktif.

---

## 3. Child Entities

### `CategoryAttribute` (Category AR İçinde — v2 YENİ)

```csharp
public class CategoryAttribute
{
    public Guid Id { get; private set; }
    public int CategoryId { get; private set; }
    public string Key { get; private set; }                     // "milk_yield_liters_daily"
    public AttributeValueType ValueType { get; private set; }
    public bool Required { get; private set; }
    public bool Filterable { get; private set; }                // arama UI filter
    public string? Unit { get; private set; }                   // "litre/gün", "kg", "ay"
    public string? OptionsJson { get; private set; }            // Enum tipinde JSON array
    public Translations Label { get; private set; }
    public Translations? HelpText { get; private set; }
    public int DisplayOrder { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
}

public enum AttributeValueType
{
    Text = 1, Number = 2, Boolean = 3, Enum = 4, Date = 5, File = 6
}
```

DB constraint: `UNIQUE(category_id, key)`.

### `BrandCategory` (Many-to-Many Junction)

```csharp
public class BrandCategory
{
    public Guid BrandId { get; private set; }
    public int CategoryId { get; private set; }
    
    public BrandCategory(Guid brandId, int categoryId) { ... }
}
```

PRIMARY KEY (brand_id, category_id) — junction, AR değil.

---

## 4. Reference Entities (INT PK, AR Değil)

### `Country` Reference

```csharp
public class Country
{
    public int Id { get; private set; }
    public string Code { get; private set; }                    // ISO 3166-1 alpha-2 — IMMUTABLE
    public string NameEn { get; private set; }
    public string NativeName { get; private set; }
    public string Region { get; private set; }                  // "MENA", "EU", "CIS"
    public string DefaultCurrencyCode { get; private set; }     // ISO 4217
    public string DefaultLanguageCode { get; private set; }     // ISO 639-1
    public string PhonePrefix { get; private set; }
    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }
    
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    
    public void Activate() { /* Internal event */ }
    public void Deactivate() { ... }
    public void UpdateDisplayOrder(int order) { ... }
    public void UpdateNativeName(string name) { ... }
    
    internal void UpdateStableFieldsFromSeed(CountrySeed seed) 
    {
        // code (immutable), name_en, region — preserveAdminEdits
        // is_active, display_order, native_name asla ezilmez
    }
}
```

### `Currency` Reference

```csharp
public class Currency
{
    public int Id { get; private set; }
    public string Code { get; private set; }                    // ISO 4217
    public string NameEn { get; private set; }
    public string Symbol { get; private set; }                  // ₺, $, د.إ
    public char SymbolPosition { get; private set; }            // 'L' / 'R'
    public int DecimalPlaces { get; private set; }              // 0/2/3/4 per ISO 4217
    public char ThousandSep { get; private set; }
    public char DecimalSep { get; private set; }
    public bool IsActive { get; private set; }
    public decimal? RateToUsd { get; private set; }             // son güncellenen kur
    public DateTimeOffset? RateUpdatedAt { get; private set; }
    
    public void UpdateRate(decimal newRate)
    {
        if (newRate <= 0) throw new DomainException();
        RateToUsd = newRate;
        RateUpdatedAt = DateTimeOffset.UtcNow;
    }
}
```

### `Language` Reference

```csharp
public class Language
{
    public int Id { get; private set; }
    public string Code { get; private set; }                    // ISO 639-1
    public string NameEn { get; private set; }
    public string NativeName { get; private set; }
    public bool IsRtl { get; private set; }
    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }
}
```

### `CertificationType` Reference

```csharp
public class CertificationType
{
    public int Id { get; private set; }
    public string Code { get; private set; }                     // kebab-case — IMMUTABLE
    public Translations NameTranslations { get; private set; }   // direkt VO field (Sapma 36 revize)
    public Translations DescriptionTranslations { get; private set; }
    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }

    public void UpdateTranslations(Translations translations)
    {
        // validate en exists via translations.TryGet("en", out _)
    }
}
```

> **Revize notu (Wave 1 Sapma 36 / KAYDET-7):** Eski tasarım `NameTranslationsJson: string` + computed `Translations.Deserialize(...)` property, Shared.Kernel/Translations.cs:5 "Domain saf — STJ attribute/converter YOK; JSONB map'i C3 Infra EF value converter (Domain saf)" kararı ile çelişiyordu. Location pattern'ine hizalandı: direkt `Translations` VO field. JSONB persistence detay'ı C3 Infrastructure EF value converter sorumluluğu. C1.1 commit `405be98` bu revize edilmiş tasarımla yaratıldı (doc-revize geriye dönük uyum).

12 seed başlangıç: `health-cert`, `vaccination-card`, `pedigree`, `export-approval`, `origin-cert`, `organic-cert`, `halal-cert` (MENA kritik), `quarantine-clearance`, `quality-stamp`, `insurance-doc`, `brucellosis-test`, `tuberculosis-test`.

### `Location` Reference (v2 YENİ — 5-Level Hierarchy)

```csharp
public class Location
{
    public int Id { get; private set; }                         // INT PK (Karar 3e istisnası — slug çakışma)
    public int? ParentId { get; private set; }                  // self-ref
    public LocationLevel Level { get; private set; }
    public CountryCode CountryCode { get; private set; }        // denormalize: tree root'tan kopya
    
    public string Code { get; private set; }                    // TR: il plaka (34), ilçe ID
                                                                 // US: state code (CA), FIPS county
    public string Slug { get; private set; }                    // kebab-case
    public string Path { get; private set; }                    // "tr/marmara/istanbul/kadikoy/caferaga"
    public Translations Name { get; private set; }
    public Translations? NativeName { get; private set; }       // local script
    
    public Point? Centroid { get; private set; }                // PostGIS Point Faz 1
    public string? PolygonGeoJsonUrl { get; private set; }      // Faz 2 MinIO MultiPolygon
    
    public bool IsActive { get; private set; }
    public int Population { get; private set; }
    public int DisplayOrder { get; private set; }
    
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
}

public enum LocationLevel
{
    Country = 1,
    Region = 2,         // TR: Marmara, Ege; bazı ülkelerde skip
    State = 3,          // TR: İl (81); US: State (50)
    District = 4,       // TR: İlçe (~970)
    Neighborhood = 5    // TR: Mahalle/Köy (~50,000+)
}
```

**Path index (hierarchical query):**
```sql
CREATE INDEX ix_locations_path_text 
ON catalog.locations USING gist (path gist_trgm_ops);

-- "Marmara'daki tüm ilçeler"
SELECT * FROM catalog.locations 
WHERE path LIKE 'tr/marmara/%' AND level = 4;
```

**TR Faz 1 Seed:**
- L1: 1 (Türkiye)
- L2: 7 (Marmara, Ege, ...)
- L3: 81 (İller)
- L4: ~970 (İlçeler)
- L5: ~50,000+ (Mahalle/Köy — Faz 1 major cities, full Faz 2)

**Diğer ülkeler:** Faz 2 kademeli (launch ülkeleri AZ/KZ/AE/SA/US/DE/GB Level 1-3 zorunlu, Level 4-5 ihtiyaca göre).

---

## 5. Cross-Modül Erişim

### Karar: Hibrit — `ICatalogReadService` Shared Interface + Redis Cache

```
Diğer Modüller        →  ICatalogReadService (Shared/Contracts/)
                             ↓ DI
                      Catalog.Infrastructure.CatalogReadService
                             ↓
                      Redis Cache (TTL'li) → fallback → Catalog DbContext
```

**Hiçbir modül Catalog tablolarına direkt query atmaz** (Karar 3d).

### `ICatalogReadService` İmza

```csharp
namespace Shared.Contracts.Catalog;

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
```

### `IAdminCatalogCommands`

```csharp
namespace Shared.Contracts.Catalog.Admin;

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
    
    // Location (v2)
    Task<Result<int>> CreateLocationAsync(CreateLocationDto dto, Guid actorAdminId, CancellationToken ct);
    Task<Result> UpdateLocationAsync(int id, UpdateLocationDto dto, Guid actorAdminId, CancellationToken ct);
    Task<Result> SetLocationCentroidAsync(int id, Point centroid, Guid actorAdminId, CancellationToken ct);
    
    // BorderRule (Faz 1 manuel, Faz 2 feature aktif)
    Task<Result<Guid>> CreateBorderRuleAsync(CreateBorderRuleDto dto, Guid actorAdminId, CancellationToken ct);
    Task<Result> UpdateBorderRuleAsync(Guid id, UpdateBorderRuleDto dto, Guid actorAdminId, CancellationToken ct);
    Task<Result> DeactivateBorderRuleAsync(Guid id, Guid actorAdminId, CancellationToken ct);
    
    // CertificationType
    Task<Result<int>> CreateCertificationTypeAsync(CreateCertificationTypeDto dto, Guid actorAdminId, CancellationToken ct);
    Task<Result> UpdateCertificationTypeAsync(int id, UpdateCertificationTypeDto dto, Guid actorAdminId, CancellationToken ct);
    Task<Result> DeactivateCertificationTypeAsync(int id, Guid actorAdminId, CancellationToken ct);
    
    // Country/Currency/Language — sadece is_active toggle + display order + translations
    Task<Result> ToggleCountryActiveAsync(int id, bool active, Guid actorAdminId, CancellationToken ct);
    Task<Result> ToggleCurrencyActiveAsync(int id, bool active, Guid actorAdminId, CancellationToken ct);
    Task<Result> ToggleLanguageActiveAsync(int id, bool active, Guid actorAdminId, CancellationToken ct);
    Task<Result> RefreshExchangeRatesAsync(Guid actorAdminId, CancellationToken ct);  // manual trigger
}
```

### `IAdminCatalogReadService`

```csharp
public interface IAdminCatalogReadService
{
    Task<CursorPage<BrandListItem>> ListBrandsAsync(BrandListFilter filter, CancellationToken ct);
    Task<MissingTranslationsReport> GetMissingTranslationsAsync(string locale, CancellationToken ct);
    Task<IReadOnlyList<RateLogEntry>> GetRateLogsAsync(int days, CancellationToken ct);
}
```

### Cache Stratejisi

| Veri | Redis TTL | Sebep |
|---|---|---|
| Country list/single | 1 saat | Çok nadiren değişir |
| Currency list/single | 1 saat | Aynı |
| Currency rates | 1 saat | Daily job ile fresh garanti |
| Language list/single | 1 saat | |
| CertificationType | 30 dakika | Admin yeni ekleyebilir |
| Category tree | 5 dakika | Admin daha sık edit eder |
| Category single | 5 dakika | |
| Brand single | 10 dakika | Admin approval frequency düşük |
| Brand by category | 10 dakika | |
| Breed list (by category) | 5 dakika | |
| Breed single | 5 dakika | |
| Location single | 1 saat | Reference data, stable |
| `IsValid*Async` validators | 1 dakika (hot path) | Listings.CreateListing validator |

**Cache key prefix:** `livestock:catalog:`

**Faz 1:** TTL-only invalidation (eventual consistency 5dk-1h kabul).
**Faz 2:** Event-driven invalidation (CategoryDeactivated → cache invalidate).

---

## 6. Currency Rate Provider (3-Tier)

### Provider Hierarchy

| Tier | Provider | URL/Source | Schedule | Çıktı |
|---|---|---|---|---|
| **1 (primary)** | TCMB | `https://www.tcmb.gov.tr/kurlar/today.xml` | Daily 15:30 TR (12:30 UTC) | 23 currency XML |
| **2 (fallback)** | ECB | `https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml` | Daily 16:00 CET | EUR-base XML |
| **3 (last resort)** | Fawazahmed0 currency-api | `https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@latest/v1/currencies/usd.json` | Daily anytime | USD-base JSON (lowercase nested `{date, usd: {...}}`) |

> **Revize (W3.6.B Aile 2 Sapma 115 / F-S57):** Plan-doc 2025'te `exchangerate.host` free aggregator olarak listelemişti; fiili 2026-05 itibarıyla apilayer akquisition sonrası API key paywall'a alındı (HTTP 200 + body `success:false` + `error.code:101 missing_access_key`). W3.6.B B.4 sub-batch'inde Backend Adım 2 fresh-fetch curl ile yakaladı. Frankfurter HTTP 404 pre-test sonrası **Fawazahmed0 currency-api jsdelivr CDN** swap onayı (Unlicense, multi-source aggregator). RateProvider enum `ExchangeRateHost = 3` → `CurrencyApi = 3` rename (int sabit, label semantik).

### `IRateProvider` Interface

```csharp
namespace Catalog.Infrastructure.RateProviders;

public interface IRateProvider
{
    RateProvider Source { get; }
    Task<RateFetchResult> FetchAsync(CancellationToken ct);
}

public sealed record RateFetchResult(
    DateOnly RateDate,
    IReadOnlyDictionary<string, decimal> RatesToUsd,
    bool Success,
    string? Error);
```

### Cron Job

> **Revize notu (W3.6.C/D fiili impl, KAYDET-7 fiili kod baskın):** Aşağıdaki örnek plan-doc 2025 tasarım taslağıdır. Wave 3 W3.6.C/D'de fiili impl üç farklılık taşır: (1) `[Quartz.JobKey(...)]` attribute Quartz.NET 3.x'te **mevcut değil** (JobKey identifier struct, attribute değil); DI'da `.WithIdentity("CurrencyRateUpdate")` pattern kullanıldı. (2) `_tcmb/_ecb/_erh` ayrı field yerine `IEnumerable<IRateProvider>` collection pattern (W3.6.B `AddTransient<IRateProvider>` × 3 factory delegate Microsoft DI semantic). (3) `_db.RateLogs.Add` / `_db.Currencies.Where` property access yerine **`_db.Set<T>()` pattern** (W3.6.C/D Adım 1.5 F5 kararı; repository + read service 8 dosya × 30+ metot tutarlı pattern, DbSet property eklenmemiş). (4) W3.6.D D.2'de Job 3-tier chain logic `ICurrencyRateRefresher.RefreshAsync` delege edildi (DRY single source of truth, Job 84→44 satır %48 azalma; admin `RefreshExchangeRatesHandler` aynı Refresher'ı çağırır).

```csharp
// Plan-doc 2025 tasarım taslağı (fiili impl revize notu yukarıda)
[Quartz.JobKey("CurrencyRateUpdate")]
public sealed class CurrencyRateUpdateJob : IJob
{
    public async Task Execute(IJobExecutionContext ctx)
    {
        var providers = new IRateProvider[] { _tcmb, _ecb, _erh };
        RateFetchResult? successful = null;
        
        foreach (var provider in providers)
        {
            var result = await provider.FetchAsync(ctx.CancellationToken);
            _db.RateLogs.Add(RateLog.Create(provider.Source, result));
            
            if (result.Success)
            {
                successful = result;
                _metrics.RecordSuccess(provider.Source.ToString());
                break;
            }
            _metrics.RecordFailure(provider.Source.ToString());
        }
        
        if (successful is null)
        {
            await _db.SaveChangesAsync(ctx.CancellationToken);
            throw new InvalidOperationException("All 3 rate providers failed");
        }
        
        var activeCurrencies = await _db.Currencies.Where(c => c.IsActive).ToListAsync(ctx.CancellationToken);
        foreach (var currency in activeCurrencies)
        {
            if (successful.RatesToUsd.TryGetValue(currency.Code, out var rate))
                currency.UpdateRate(rate);
        }
        await _db.SaveChangesAsync(ctx.CancellationToken);
    }
}
```

**Schedule:** Daily 13:00 UTC (TCMB ~12:30 UTC publish; 30dk buffer). **Fiili cron expression (W3.6.C C.3 DI register):** `"0 0 13 * * ?"` (Quartz 6-field format: sec min hour day-of-month month day-of-week, `?` no-spec).

### `RateLog` Entity (Audit)

```csharp
public class RateLog
{
    public Guid Id { get; private set; }
    public DateOnly RateDate { get; private set; }
    public RateProvider Source { get; private set; }
    public string RatesJson { get; private set; }
    public bool Success { get; private set; }
    public string? Error { get; private set; }
    public DateTimeOffset FetchedAt { get; private set; }
}

public enum RateProvider { Tcmb = 1, Ecb = 2, ExchangeRateHost = 3, Manual = 99 }
```

`catalog.rate_logs` append-only. Retention 1 yıl.

### Monitoring Metric'leri (Grafana)

| Metric | Type | Tag |
|---|---|---|
| `catalog_currency_rate_update_total` | Counter | source, status |
| `catalog_currency_rate_age_seconds` | Gauge | currency |
| `catalog_currency_rate_value` | Gauge | currency (anomaly detection) |
| `catalog_currency_rate_provider_used` | Gauge | hangi provider başarılı |

**Alert:**
- 3 ardışık day all-providers-failed → PagerDuty
- `rate_age_seconds > 172800` (48h) → warning
- Rate sapma > %20 in 24h → manual review

---

## 7. Public Event'ler

### Faz 1 Active (7 event)

| Event | Producer | Consumer |
|---|---|---|
| `CategoryDeactivated` | Catalog | Listings (yeni listing block) |
| `CategoryReactivated` | Catalog | Listings |
| `BreedDeactivated` | Catalog | Listings |
| `BreedReactivated` | Catalog | Listings |
| `BrandApproved` | Catalog (v2) | Notifications (suggester'a bildir), Listings |
| `BrandDeactivated` | Catalog (v2) | Listings (yeni listing block) |
| `BrandReactivated` | Catalog (v2) | Listings |

### Internal Events

`CategoryCreated`, `CategoryRenamed`, `CategoryMoved`, `BreedCreated`, `BrandSuggested`, `BrandRejected`.

### Faz 2 Activate (BorderRule × 3)

`BorderRuleCreated`, `BorderRuleUpdated`, `BorderRuleDeactivated` — Listings consumer cross-border listing impact.

---

## 8. API Endpoint Inventory

> **Revize notu (W3.7.4 fiili durum, Sapma 133):** Wave 3 W3.7 host-wire sonrası fiili durum: Wave 1+2'de **sadece admin endpoint'leri** Application katmanında yazıldı (10 endpoint extension: Categories/Breeds/Brands/Currencies/Countries/Languages/CertificationTypes/Locations/BorderRules/AdminCatalogRead, hepsi `/admin/catalog/*` prefix). **Public `/catalog/*` endpoint'leri (aşağıdaki 15 endpoint) Wave 4+ scope** — Listings/Marketplace consumer'ları `ICatalogReadService` üzerinden erişir; Frontend tarafı public API'lar Wave 4+ host-wire'da eklenir. W3.7.4 smoke test'inde `GET /catalog/categories` HTTP 404 döner (Wave 4 detour, beklenen).

### Public (15 endpoint, Wave 4+ scope)

| Method | Path |
|---|---|
| GET | `/catalog/countries?locale=` |
| GET | `/catalog/countries/{code}` |
| GET | `/catalog/currencies` |
| GET | `/catalog/currencies/{code}` |
| GET | `/catalog/languages` |
| GET | `/catalog/categories/tree?locale=` |
| GET | `/catalog/categories/{code}?locale=` (includes `attributes: [...]` v2) |
| GET | `/catalog/breeds?categoryCode=&locale=` |
| GET | `/catalog/breeds/{code}?locale=` |
| GET | `/catalog/brands?categoryId=&cursor=` (v2) |
| GET | `/catalog/brands/{slugOrId}` (v2) |
| GET | `/catalog/locations?parentId=&level=&search=&cursor=` (v2) |
| GET | `/catalog/locations/{id}` (v2) |
| GET | `/catalog/locations/{id}/children` (v2) |
| GET | `/catalog/certification-types?locale=` |

### Authenticated — Seller Brand Suggestion (1 endpoint)

| Method | Path |
|---|---|
| POST | `/me/seller/brands/suggest` (v2 — admin queue'ya düşer) |

### Admin (17 endpoint)

| Method | Path |
|---|---|
| POST | `/admin/catalog/categories` |
| PATCH | `/admin/catalog/categories/{id}` |
| POST | `/admin/catalog/categories/{id}/deactivate` + reactivate |
| POST | `/admin/catalog/categories/{id}/attributes` (v2) |
| PATCH | `/admin/catalog/categories/{id}/attributes/{attrId}` (v2) |
| DELETE | `/admin/catalog/categories/{id}/attributes/{attrId}` (v2) |
| POST | `/admin/catalog/breeds` |
| PATCH | `/admin/catalog/breeds/{id}` |
| POST | `/admin/catalog/breeds/{id}/deactivate` + reactivate |
| GET | `/admin/catalog/brands?status=&cursor=` |
| POST | `/admin/catalog/brands` |
| PATCH | `/admin/catalog/brands/{id}` |
| POST | `/admin/catalog/brands/{id}/approve` |
| POST | `/admin/catalog/brands/{id}/reject` |
| POST | `/admin/catalog/brands/{id}/deactivate` + reactivate |
| POST | `/admin/catalog/brands/import` (bulk CSV/JSON) |
| POST | `/admin/catalog/locations` (v2) |
| PATCH | `/admin/catalog/locations/{id}` (v2) |
| POST | `/admin/catalog/locations/{id}/centroid` (v2 — PostGIS Point) |
| GET | `/admin/catalog/border-rules?cursor=` (v2 — Faz 1 admin manuel) |
| POST | `/admin/catalog/border-rules` (v2) |
| PATCH | `/admin/catalog/border-rules/{id}` (v2) |
| POST | `/admin/catalog/border-rules/{id}/deactivate` (v2) |
| POST | `/admin/catalog/certification-types` |
| PATCH | `/admin/catalog/certification-types/{id}` |
| DELETE | `/admin/catalog/certification-types/{id}` |
| PATCH | `/admin/catalog/countries/{id}` (translations, display_order, default codes) |
| POST | `/admin/catalog/countries/{id}/toggle-active` |
| PATCH | `/admin/catalog/currencies/{id}` (symbol, separator, is_active) |
| PATCH | `/admin/catalog/languages/{id}` |
| GET | `/admin/catalog/translations/missing?locale=` |
| POST | `/admin/catalog/currencies/refresh-rates` (manual trigger) |
| GET | `/admin/catalog/rate-logs?cursor=` (v2 — diagnostic) |

**Toplam Catalog v2: 15 public + 1 seller + 17 admin = 33 endpoint** (v1: 27 → +6 v2).

### Pagination Pattern

| Endpoint | Pagination |
|---|---|
| Categories tree | Yok (full tree, ~54 satır) |
| Countries/Currencies/Languages | Yok (active liste küçük) |
| Brands | Cursor (could grow) |
| Locations | Cursor (885K seed!) |
| Breeds | Cursor |
| Cert types | Yok |

### Locale Parameter

Karar 6/4 — Locale resolution middleware. Query string `?locale=tr` override; default user.preferences/cookie/Accept-Language/IP-country/en chain.

---

## 9. Translation Handling

### TranslationHelper (Shared)

```csharp
public static class TranslationHelper
{
    public const string FallbackLocale = "en";
    
    public static string Resolve(
        Translations translations, 
        string preferredLocale, 
        string? customFallback = null)
    {
        if (translations is null || translations.IsEmpty) return string.Empty;
        
        if (translations.TryGet(preferredLocale, out var v) && !string.IsNullOrWhiteSpace(v))
            return v;
        if (translations.TryGetIgnoreCase(preferredLocale, out v) && !string.IsNullOrWhiteSpace(v))
            return v;
        if (customFallback != null && translations.TryGet(customFallback, out v) && !string.IsNullOrWhiteSpace(v))
            return v;
        if (translations.TryGet(FallbackLocale, out v) && !string.IsNullOrWhiteSpace(v))
            return v;
        return translations.FirstOrEmpty();
    }
}
```

### Missing Translation Report

```
GET /admin/catalog/translations/missing?locale=ar
   → { categories: [...], breeds: [...], brands: [...], certifications: [...] }
```

i18n gap detection nightly job: missing count Prometheus metric.

---

## 10. preserveAdminEdits Semantiği

Karar 4c kararı — seed re-run admin manuel düzenlemeleri ezmiyor.

| Entity | Stable (re-seed override) | Yarı-stable (NULL ise) | Admin-owned (asla ezme) |
|---|---|---|---|
| Country | code, name_en, region | phone_prefix, default_currency_code, default_language_code | is_active, display_order, native_name |
| Currency | code, name_en, decimal_places | symbol | symbol_position, separators, is_active |
| Language | code, name_en, is_rtl | native_name | is_active, display_order |
| CertificationType | code | — | name_translations, description_translations, is_active, display_order |
| Category | code, parent_id, level | — | name_translations, description_translations, is_active, display_order, icon_key |
| Breed | code, category_id, origin_country_code | — | name_translations, description_translations, is_active, display_order |
| Brand | code, slug (seed brands) | — | name_translations, description_translations, is_active, display_order, logo_url |

Method `UpdateStableFieldsFromSeed(seed)` AR'da; sadece "stable" alanları update eder.

---

## 11. Discovered Backlog

| # | Konu | Hedef |
|---|---|---|
| 159 | Diğer ülke Location seed planı (launch ülkeleri Level 1-3 zorunlu) | Karar 7 / Localization Faz 1 launch |
| 160 | Faz 2 Listings strict CategoryAttribute validation | Karar 5 / Listings Faz 2 |
| 161 | OpenStreetMap import worker — Location.PolygonGeoJsonUrl Faz 2 bulk import | Karar 7 / Data ops Faz 2 |
| 162 | TÜİK Population yearly update — Location.Population Faz 2 update job | Karar 7 / Data ops Faz 2 |
| 163 | Brand RatingAverage computed (Marketplace completed deals'tan, Faz 2) | Karar 5 / Catalog Faz 2 |
| 164 | Faz 2 cross-border listing flow — BorderRule entegrasyonu, target export countries | Karar 5 / Listings Faz 2 |
| 165 | TCMB XML parser robustness — encoding, schema değişimi, fallback retry | Karar 7 / Reliability |
| 166 | Currency rate anomaly detection — günlük %20 sapma alert + manual review | Karar 7 / Observability |

---

## 12. Özet Tablo

| Konu | Karar |
|---|---|
| AR sayısı | 4 (Category, Breed, **Brand**, **BorderRule** Faz 2 placeholder) |
| Reference entity | 5 (Country, Currency, Language, CertificationType, **Location**) |
| Public events Faz 1 | 7 (Category × 2 + Breed × 2 + Brand × 3) — Faz 2 + 3 BorderRule |
| Cross-modül erişim | `ICatalogReadService` + Redis cache (TTL 1dk-1h per veri tipi) |
| Currency rate provider | 3-tier: TCMB primary + ECB fallback + exchangerate.host last resort; daily 13:00 UTC |
| Location | 5-level hierarchy, INT PK (slug çakışma istisnası); TR ~885K Faz 1 seed |
| Brand workflow | Seller suggest → admin approve/reject; bulk import CSV/JSON |
| CategoryAttribute | Faz 1 schema (Catalog) + Faz 1 free-form Listings + Faz 2 strict validation Listings'te |
| Endpoint | 33 (15 public + 1 seller + 17 admin) — v1'den +6 |
