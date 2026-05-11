# Karar 5 / Listings Modülü

**Status:** FINAL
**Wave:** 4 (parallel with Wave 3 Accounts — prod; sequential dev)

## İlişkili Kararlar

- **Üst:** [Karar 2](../02-modules-list.md), [Karar 3a — AR](../03-domain-patterns.md)
- **Patch:** [Patch 4 — Listings](../05-patch.md) — `IListingsCommands.ReserveAsync/UnreserveAsync/MarkSoldAsync`; `IListingsReadService.GetBoostAnalyticsAsync`; `Listing.BrandId` field
- **Frontend:** `frontend-api-inventory.md` Listings (21 endpoint — faceted search, AI translate, AI tag, refresh, paused status, multi-currency, documents)
- **Çekince 1:** ListingAttribute Faz 1 free-form; Faz 2 strict (Catalog CategoryAttribute schema'sından)
- **Çekince 2:** AI quota — Translation quota'lı; Tagging quota'sız + rate limit 1/dk per listing
- **Çekince 3:** Slug strategy `{slug-from-title}-{4-char-base32}` (UUID suffix, çakışma kontrolü yok)
- **Çekince 4:** DocumentVerifiedBy.System=1 Faz 2 idle (future-positive enum)
- **Çekince 5:** Cross-modül dependency listesi büyük (8 modüle bağımlı — Catalog/Accounts/Subscription/Identity/FileStorage + Carrier/Marketplace/Messaging). Düzeltme yok; Karar 7/Observability'de circular dependency check + cache invalidation strategy (özellikle `Subscription.PlanFeatures` değişimi → Listings effective limit cache invalidate); call graph dökümantasyonu PR review checklist'inde

---

## 1. Modülün Rolü ve Sınırları

### Sahip

| Konsept | Sahiplik |
|---|---|
| Listing AR (CRUD + lifecycle FSM 9-state) | Listings |
| ListingImage, ListingAttribute, ListingCertification, ListingDocument, ListingTranslation, ListingReport child entity'ler | Listings |
| SavedSearch AR (kullanıcı filtre + match notification) | Listings |
| AI tagging async job | Listings (worker dispatcher) |
| AI translation async job | Listings (worker, 50 dil) |
| Refresh action (quota-based, Subscription consume) | Listings |
| Faceted search (PostgreSQL aggregation Faz 1; Meilisearch Faz 2) | Listings |
| Multi-currency display (Catalog rate-based runtime conversion) | Listings |

### Sahip Olmayan

| Konsept | Sahibi |
|---|---|
| Seller business profile | Accounts |
| Boost campaign lifecycle | Subscription |
| Currency conversion rate (kur) | Catalog (sync read) |
| Photo physical storage | Shared IFileStorage → MinIO |
| AI service (translation/tagging) | External SaaS provider; adapter Listings.Infrastructure |

---

## 2. Aggregate Roots

### `Listing` AR

```csharp
public class Listing
{
    public Guid Id { get; private set; }                        // Guid v7
    public Guid SellerId { get; private set; }                  // ID-ref Accounts.Seller
    public Guid SellerUserId { get; private set; }              // denormalize fast ownership check
    public Guid FarmId { get; private set; }                    // ID-ref Accounts.Farm
    
    public string Slug { get; private set; }                    // Çekince 3 — {title-slug}-{4-char-base32}
    public int CategoryId { get; private set; }                 // Catalog reference
    public int? BreedId { get; private set; }                   // Catalog (optional)
    public Guid? BrandId { get; private set; }                  // Catalog v2 (optional) — Patch 4.4
    public LivestockKind Kind { get; private set; }
    
    public Translations Title { get; private set; }
    public Translations? Description { get; private set; }
    public LanguageCode SourceLanguage { get; private set; }    // seller'ın yazdığı dil; AI translate base
    
    public Money Price { get; private set; }
    public PriceMode PriceMode { get; private set; }            // FixedTotal/PerHead/PerKg/Negotiable
    public bool AcceptsCounterOffer;
    
    public ListingLocation Location { get; private set; }
    public int AnimalCount { get; private set; }
    public decimal? AverageWeightKg, AverageAgeMonths;
    
    public ListingStatus Status { get; private set; }
    public DateTimeOffset? SubmittedAt, ApprovedAt, RejectedAt, PausedAt, SoldAt, ReservedAt, ExpiresAt, DeletedAt;
    public Guid? ApprovedByUserId;
    public string? RejectionReason;
    public Guid? ReservedByDealId;
    
    public DateTimeOffset CreatedAt, UpdatedAt;
    public DateTimeOffset? LastRefreshedAt;
    
    // Faz 2 schema-ready
    public decimal? AiQualityScore, AiModerationScore;
    public string? AiTagsJson;                                   // jsonb array
    
    // Boost flags (denormalize Subscription)
    public bool HasTopRowBoost, HasShowcaseBoost, HasUrgentBadge;
    
    private readonly List<ListingImage> _images = new();
    private readonly List<ListingAttribute> _attributes = new();
    private readonly List<ListingCertification> _certifications = new();
    private readonly List<ListingDocument> _documents = new();
    private readonly List<ListingTranslation> _translations = new();
    private readonly List<ListingReport> _reports = new();
    
    public static Listing CreateDraft(
        Guid sellerId, Guid sellerUserId, Guid farmId,
        int categoryId, int? breedId, Guid? brandId, LivestockKind kind,
        Translations title, LanguageCode sourceLanguage,
        Money price, PriceMode priceMode, bool acceptsCounter,
        ListingLocation location, int animalCount, decimal? avgWeight, decimal? avgAgeMonths) 
    {
        var listing = new Listing
        {
            Id = Guid.CreateVersion7(),
            /* ... */
            Status = ListingStatus.Draft,
            Slug = SlugHelper.GenerateListingSlug(title.Get(sourceLanguage.Value) ?? "listing", Id),
        };
        return listing;
    }
    
    // Lifecycle FSM
    public void UpdateDraft(...) { /* sadece Draft */ }
    
    public void SubmitForReview()
    {
        if (Status != ListingStatus.Draft && Status != ListingStatus.Rejected)
            throw new DomainException();
        if (_images.Count == 0) throw new DomainException("At least one image required");
        Status = ListingStatus.PendingReview;
        SubmittedAt = DateTimeOffset.UtcNow;
        // Public event: ListingSubmittedForReview
    }
    
    public void Approve(Guid actorAdminUserId, TimeSpan? ttl = null)
    {
        if (Status != ListingStatus.PendingReview) throw new DomainException();
        Status = ListingStatus.Active;
        ApprovedAt = DateTimeOffset.UtcNow;
        ApprovedByUserId = actorAdminUserId;
        ExpiresAt = DateTimeOffset.UtcNow.Add(ttl ?? TimeSpan.FromDays(60));
        // Public event: ListingApproved → SavedSearch matcher tetiklenir
    }
    
    public void Reject(Guid actorAdminUserId, string reason) { /* ListingRejected */ }
    
    public void Pause(string? reason)
    {
        if (Status != ListingStatus.Active) throw new DomainException("Only Active can be paused");
        Status = ListingStatus.Paused;
        // Public event: ListingPaused
    }
    
    public void Resume() { /* Paused → Active, ListingResumed */ }
    
    public void Reserve(Guid dealId) 
    { 
        // Marketplace OfferAccepted'ta IListingsCommands.ReserveAsync ile çağrılır
        if (Status != ListingStatus.Active) throw new DomainException();
        Status = ListingStatus.Reserved;
        ReservedAt = DateTimeOffset.UtcNow;
        ReservedByDealId = dealId;
    }
    
    public void Unreserve() { /* OfferCancelled / DealCancelled cascade */ }
    
    public void MarkSold(Guid? dealId, Guid? buyerUserId)
    {
        // Marketplace DealCompleted cascade
        Status = ListingStatus.Sold;
        SoldAt = DateTimeOffset.UtcNow;
        // Public event: ListingSold
    }
    
    public void Expire() { /* ListingExpired */ }
    public void Delete(string? reason) { /* soft delete, ListingDeleted */ }
    
    public void Refresh()
    {
        if (Status != ListingStatus.Active && Status != ListingStatus.Paused)
            throw new DomainException("Only Active/Paused can be refreshed");
        LastRefreshedAt = DateTimeOffset.UtcNow;
        // Internal event: ListingRefreshed (search re-rank)
    }
    
    public void UpdatePrice(Money newPrice)
    {
        if (Status != ListingStatus.Active && Status != ListingStatus.Paused)
            throw new DomainException();
        var oldPrice = Price;
        Price = newPrice;
        // Public event: ListingPriceChanged (oldPrice, newPrice — favori sahipleri notification)
    }
    
    // Approved listing edit policy (Backlog #2 — kapanış)
    public void UpdateApprovedListing(Translations? newTitle, Translations? newDescription, int? animalCount)
    {
        // ✓ Edit izinli: Title, Description, AnimalCount, AverageWeightKg, AverageAgeMonths, Images add/remove
        // ✓ Edit + Price separate: UpdatePrice method (event fire)
        // ✗ Re-approval gerekiyor: CategoryId, BreedId, Location (status → PendingReview)
        // ✗ Immutable: FarmId, SellerId
    }
    
    // Image
    public ListingImage AddImage(string url, int order, string? aiAltText = null) { ... }
    public void RemoveImage(Guid imageId) { ... }
    public void ReorderImages(IReadOnlyList<Guid> orderedIds) { ... }
    
    // Document
    public ListingDocument AddDocument(ListingDocumentType type, string url, DocumentVerifiedBy? verifiedBy = null) { ... }
    
    // Translation
    public ListingTranslation AddOrUpdateTranslation(LanguageCode lang, string title, string? description, TranslationSource source) { ... }
    
    // Boost (Subscription event consumer)
    public void ApplyBoost(BoostKind kind, DateTimeOffset until) { ... }
    public void RemoveBoost(BoostKind kind) { ... }
}

public enum ListingStatus
{
    Draft = 1, PendingReview = 2, Active = 3, Reserved = 4,
    Paused = 5,           // frontend bekliyor — owner manuel pause
    Sold = 6, Rejected = 7, Expired = 8, Deleted = 9
}

public enum PriceMode { FixedTotal = 1, PerHead = 2, PerKg = 3, Negotiable = 4 }
```

### `SavedSearch` AR

```csharp
public class SavedSearch
{
    public Guid Id;
    public Guid UserId;
    public string Name;                                          // "Marmara Holstein 2-3 yaş"
    public SavedSearchFilter Filter;                             // VO — Karar 3e Madde 7
    public NotificationFrequency NotificationFreq;               // Instant/Daily/Weekly/Never
    public DateTimeOffset CreatedAt;
    public DateTimeOffset? LastNotifiedAt;
    public int MatchCount;
    
    public static SavedSearch Create(Guid userId, string name, SavedSearchFilter filter, NotificationFrequency freq) { ... }
    public void UpdateFilter(SavedSearchFilter newFilter) { ... }
    public void RecordMatch() { MatchCount++; LastNotifiedAt = DateTimeOffset.UtcNow; }
}
```

---

## 3. Child Entities

### `ListingImage`

```csharp
public class ListingImage
{
    public Guid Id, ListingId;
    public string Url;                                           // IFileStorage URL
    public int Order;
    public string? AiAltText;                                    // Faz 2 AI generated
    public bool IsCover;
    public DateTimeOffset UploadedAt;
}
```

Max 20 image per listing. Pre-signed upload pattern.

### `ListingDocument` (verifiedBy: system/ministry/vet)

```csharp
public class ListingDocument
{
    public Guid Id, ListingId;
    public ListingDocumentType Type;
    public string Url, FileName;
    public long SizeBytes;
    public DocumentVerifiedBy? VerifiedBy;                       // YENİ — system/ministry/vet
    public Guid? VerifierUserId;                                  // Vet verification ise
    public DateTimeOffset? VerifiedAt;
    public DateTimeOffset UploadedAt;
}

public enum ListingDocumentType
{
    HealthReport = 1, VaccinationCard = 2, Pedigree = 3,
    ExportCertificate = 4, OriginCertificate = 5,
    QuarantineClearance = 6, TestResult = 7, Other = 99
}

// Çekince 4 — System=1 Faz 2 idle (future-positive)
public enum DocumentVerifiedBy
{
    System = 1,           // Faz 2 — otomatik signature/format check
    Ministry = 2,         // Faz 2 — Bakanlık HBS sync
    Vet = 3               // Faz 1 — vet kullanıcısı onayladı
}
```

**Vet verification:**
1. Seller upload (e.g., VaccinationCard)
2. Seller "Vet onayı istiyorum" işaretler
3. Active Vet'e atanır (pending queue)
4. Vet onaylar → VerifiedBy=Vet, VerifierUserId=vetUserId

### `ListingTranslation`

```csharp
public class ListingTranslation
{
    public Guid Id, ListingId;
    public LanguageCode Language;
    public string Title;
    public string? Description;
    public TranslationSource Source;                             // Original/AiAuto/HumanCorrected
    public Guid? TranslatedByUserId;
    public string? AiProvider;                                   // "openai-gpt-4o", "deepl-v2"
    public decimal? AiConfidence;
    public DateTimeOffset CreatedAt, UpdatedAt;
}

public enum TranslationSource { Original = 1, AiAuto = 2, HumanCorrected = 3 }
```

50 dil hedefi; AI translate quota-based.

### `ListingAttribute` (Çekince 1 — Faz 1 free-form)

```csharp
public class ListingAttribute
{
    public Guid Id, ListingId;
    public string Key;                                           // "milk_yield_liters_daily"
    public string Value;
    public string? Unit;
    public AttributeValueType ValueType;
}
```

Faz 1 validator hafif (≤50 attribute soft limit). Faz 2 Catalog `CategoryAttribute` schema'sından strict validation.

### `ListingCertification`

```csharp
public class ListingCertification
{
    public Guid Id, ListingId;
    public int CertificationTypeId;                              // Catalog reference
    public string CertificateUrl;
    public DateTimeOffset? IssuedAt, ExpiresAt;
    public DocumentVerifiedBy? VerifiedBy;
}
```

### `ListingReport`

```csharp
public class ListingReport
{
    public Guid Id, ListingId;
    public Guid ReporterUserId;
    public string Reason;
    public string? Description;
    public ReportStatus Status;                                  // Pending/Resolved/Dismissed
    public DateTimeOffset CreatedAt, ResolvedAt?;
}
```

---

## 4. AI Tagging & Translation

### AI Tagging — Quota'sız + Rate Limit

```
POST /me/listings/{id}/ai-tag
   ↓ No quota check (Çekince 2 — Faz 1)
   ↓ Redis SETNX rate limit 1/dk per (userId, listingId)
   ↓ Dispatch worker (Quartz)
   ↓ Worker: OpenAI Vision API
   ↓ listing.AiTagsJson = [...]
   ↓ Internal event: ListingTagged
```

### AI Translation — Quota'lı

```
POST /me/listings/{id}/translate { targetLanguages: ["en", "ar", "ru"] }
   ↓ ISubscriptionReadService.GetTranslationQuotaRemainingAsync
   ↓ < N (target count) ise 403
   ↓ ISubscriptionCommands.ConsumeTranslationQuotaAsync × N
   ↓ Per target language dispatch worker
   ↓ Worker: DeepL primary / OpenAI fallback
   ↓ ListingTranslation row insert (source=AiAuto, confidence)
   ↓ Internal event: ListingTranslated
```

---

## 5. Faceted Search

### Response Shape

```csharp
public sealed record FacetedSearchResponse(
    IReadOnlyList<ListingCard> Items,
    string? NextCursor,
    int TotalCount,
    SearchFacets Facets,
    IReadOnlyList<MapPin>? MapPins);   // sort=distance veya viewMode=map

public sealed record SearchFacets(
    PriceHistogram Price,
    IReadOnlyList<CountBucket> Breeds,
    IReadOnlyList<RangeBucket<decimal>> AgeBuckets,
    IReadOnlyList<RangeBucket<decimal>> WeightBuckets,
    IReadOnlyList<CountBucket> Certifications,
    IReadOnlyList<CountBucket> Countries,
    IReadOnlyList<CountBucket> LocationsByRegion,
    IReadOnlyList<CountBucket> Sellers);

public sealed record PriceHistogram(Money Min, Money Max, IReadOnlyList<HistogramBucket> Buckets);
public sealed record MapPin(Guid ListingId, double Lat, double Lng, decimal Price, string Currency);
```

### Faz 1: PostgreSQL Aggregation

```sql
-- PriceHistogram
SELECT width_bucket(price_amount, $min, $max, 10) AS bucket,
       MIN(price_amount), MAX(price_amount), COUNT(*)
FROM listings.listings
WHERE status='active' AND /* filters */
GROUP BY bucket;

-- BreedsCount
SELECT breed_id, COUNT(*) FROM listings.listings
WHERE status='active' AND /* filters */
GROUP BY breed_id ORDER BY COUNT(*) DESC LIMIT 20;
```

10K+ listing'de PostgreSQL aggregate ~50-100ms (Faz 1 acceptable).

### Faz 2: Meilisearch (Backlog #114)

Trigger: 10K listing veya search P95 > 200ms.

---

## 6. Multi-Currency Display

`listing.Price` seller'ın preferences'tan (örn. TRY). Public detail response'ta tüm aktif currency'lere converted:

```csharp
public sealed record ListingDetailResponse(
    Guid Id, string Slug, /* fields */,
    Money Price,
    IReadOnlyDictionary<string, Money> PriceConverted);   // { "USD": ..., "EUR": ..., "AZN": ... }

public sealed class ListingDetailResponseBuilder
{
    public async Task<ListingDetailResponse> BuildAsync(Listing listing, CancellationToken ct)
    {
        var activeCurrencies = await _catalog.ListActiveCurrenciesAsync(ct);
        var sourceRate = (await _catalog.GetCurrencyAsync(listing.Price.Currency.Value, ct))!.RateToUsd!.Value;
        
        var converted = new Dictionary<string, Money>();
        foreach (var c in activeCurrencies.Where(c => c.Code != listing.Price.Currency.Value))
        {
            var targetRate = c.RateToUsd ?? 1m;
            var usdAmount = listing.Price.Amount / sourceRate;
            var targetAmount = usdAmount * targetRate;
            converted[c.Code] = new Money(Math.Round(targetAmount, c.DecimalPlaces), CurrencyCode.Parse(c.Code));
        }
        
        return new ListingDetailResponse(...) { Price = listing.Price, PriceConverted = converted };
    }
}
```

Listing detail response 5dk Redis cache.

---

## 7. Refresh Feature (Çekince 1 — Subscription'tan Ayrılan)

```csharp
public sealed class RefreshListingHandler : IConsumer<RefreshListingCommand>
{
    public async Task Consume(...)
    {
        var userId = _currentUser.GetUserId()!.Value;
        
        var remaining = await _subRead.GetRefreshQuotaRemainingAsync(userId, ct);
        if (remaining <= 0) throw new ForbiddenException("RefreshQuotaExhausted");
        
        var listing = await _db.Listings.FirstAsync(l => l.Id == cmd.ListingId);
        if (listing.SellerUserId != userId) throw new ForbiddenException("NotOwner");
        
        listing.Refresh();
        
        var consumeResult = await _subCommands.ConsumeRefreshQuotaAsync(userId, ct);
        if (consumeResult.IsFailure) throw new ConflictException();
        
        await _uow.SaveChangesAsync(ct);
    }
}
```

Rate limit per plan: Free=1/month, Standard=10/month, Pro=unlimited (-1).

---

## 8. Cross-Modül Erişim

### Tükettiği

| Modül | Method |
|---|---|
| Catalog | `ICatalogReadService` (category/breed/location/cert validation, currency rate) |
| Catalog (v2) | `IsValidBrandForCategoryAsync` (Patch 4.4) |
| Accounts | `IAccountsReadService.FarmCanListAnimalsAsync` (Y2 validator) |
| Accounts | `IsActiveSellerAsync` (status check) |
| Accounts | `GetVerifiedBreedingCertificationAsync` (badge resolve — verified_breeding) |
| Subscription | `GetMaxActiveListingsAsync` (CreateListing limit) |
| Subscription | `HasActiveBoostAsync` (search ranking) |
| Subscription | `GetRefreshQuotaRemainingAsync` + `ConsumeRefreshQuotaAsync` |
| Subscription | `GetTranslationQuotaRemainingAsync` + `ConsumeTranslationQuotaAsync` |
| Identity | `GetUserSummaryAsync` (seller display) |
| FileStorage | Image/document storage |

### Sunduğu

```csharp
public interface IListingsReadService
{
    Task<int> GetActiveListingCountAsync(Guid sellerId, CancellationToken ct);
    Task<int> GetFavoriteCountAsync(Guid userId, CancellationToken ct);   // (favorite Marketplace ama Listings cache edebilir)
    Task<int> GetPedigreeDocumentCountAsync(Guid sellerId, CancellationToken ct);
    
    Task<ListingSummary?> GetSummaryAsync(Guid listingId, CancellationToken ct);
    Task<IReadOnlyList<ListingSummary>> GetSummariesAsync(IReadOnlyList<Guid> listingIds, CancellationToken ct);
    
    // Subscription boost analytics (Çekince 3)
    Task<BoostAnalyticsSlice?> GetBoostAnalyticsAsync(
        Guid listingId, DateTimeOffset since, DateTimeOffset until, CancellationToken ct);
}

public sealed record BoostAnalyticsSlice(int ViewCount, decimal IncreasePercent, int? ClickThroughCount);
```

### Sunduğu Commands (Patch 4)

```csharp
public interface IListingsCommands
{
    // Marketplace cross-module write
    Task<Result> ReserveAsync(Guid listingId, Guid dealId, CancellationToken ct);
    Task<Result> UnreserveAsync(Guid listingId, Guid dealId, CancellationToken ct);
    Task<Result> MarkSoldAsync(Guid listingId, Guid dealId, Guid? buyerUserId, CancellationToken ct);
}
```

---

## 9. Public Event'ler (11)

| Event | Consumer |
|---|---|
| `ListingSubmittedForReview` | Notifications (moderator queue) |
| `ListingApproved` | Notifications (seller), Listings self (SavedSearch matcher) |
| `ListingRejected` | Notifications |
| `ListingPaused` | Notifications |
| `ListingResumed` | Notifications |
| `ListingSold` | Marketplace (close offers), Notifications (favori sahipleri + seller) |
| `ListingExpired` | Marketplace (close offers), Notifications |
| `ListingDeleted` | Marketplace (cancel offers), Messaging (conversation context), Notifications (3 consumer) |
| `ListingPriceChanged` | Notifications (favori sahipleri "fiyat düştü" alert) |
| `ListingReportFiled` | Notifications (admin moderation queue) |
| `SavedSearchMatchFound` | Notifications (user'a "yeni eşleşme") |

**Internal:** ListingDrafted, ListingImageAdded/Removed, ListingAttributeChanged, ListingRefreshed, ListingTagged, ListingTranslated.

---

## 10. API Endpoint Inventory (31)

### Public — Search & Discovery (4)

| Method | Path |
|---|---|
| GET | `/listings?category=&breed=&country=&priceMin=&priceMax=&radius=&sort=&cursor=` (faceted search + facets) |
| GET | `/listings/{slugOrId}` (multi-currency price) |
| GET | `/listings/{id}/related` |
| POST | `/listings/{id}/report` (auth) |

### Authenticated — My Listings (10)

`GET/POST /me/listings` + `/{id}` (GET/PATCH) + `/submit` + `/pause` + `/resume` + `/refresh` + `/delete` + `/price`.

### Authenticated — Photos & Documents (5)

`POST /me/listings/{id}/upload-url` (pre-signed) + `/photos` (POST/DELETE) + `/documents` (POST/DELETE).

### Authenticated — AI Operations (2)

`POST /me/listings/{id}/translate { targetLanguages }` + `/ai-tag` (async, response 202 + jobId).

### Authenticated — Saved Searches (4)

`GET/POST /me/saved-searches` + `PATCH/DELETE /{id}`.

### Admin (4)

`GET /admin/listings?status=pending_review&cursor=` + `/approve` + `/reject` + `/reports` + `/reports/{id}/resolve`.

### Vet Verification (1)

`POST /me/vet/listings/{listingId}/documents/{docId}/verify` — Vet onayı.

---

## 11. Discovered Backlog

| # | Konu | Hedef |
|---|---|---|
| 2 | Approved listing edit policy (kapandı) | ✓ |
| 32 | radiusKm → metres VO method | Bu doc'ta |
| 35 | IListingSearchQuery interface (Faz 2 read replica) | Karar 4f / Faz 2 |
| 113 | IListingsReadService.GetBoostAnalyticsAsync (kapandı) | ✓ |
| 114 | Meilisearch migration milestone | Karar 7 / Performance |
| 115 | AI provider selection (DeepL vs OpenAI vs Google Cloud) | Karar 7 / AI vendor |
| 116 | Listing slug strategy (uuid suffix kapandı) | ✓ Çekince 3 |
| 117 | Image moderation pipeline Faz 2 (adult content, watermark) | Karar 7 / Trust&Safety |
| 118 | Approved listing edit re-approval matrix (kapandı) | ✓ |
| 119 | AI quota granular Faz 2 | Karar 5 / Subscription |
| 120 | PriceConverted cache strategy (5dk Redis, event-driven invalidate) | Karar 7 / Performance |

---

## 12. Özet Tablo

| Konu | Karar |
|---|---|
| AR sayısı | 2 (Listing, SavedSearch) |
| Listing child entities | Image, Document (verifiedBy), Translation, Attribute, Certification, Report |
| Public events | 11 |
| Status FSM | Draft → PendingReview → Active → Reserved/Paused/Sold/Expired/Rejected → Deleted; **paused** frontend hizalı |
| Multi-currency | Catalog rate-based runtime conversion, 5dk cache |
| Faceted search | PostgreSQL aggregation Faz 1; Meilisearch Faz 2 |
| AI tagging/translation | Async worker (Quartz); Translation quota'lı + Tagging rate limit |
| Refresh | Quota-based, ISubscriptionCommands.ConsumeRefreshQuota |
| verified_breeding badge | Accounts.SellerCertification cross-module read |
| BrandId | Optional UUID field (Patch 4.4) |
| Slug | `{title-slug}-{4-char-base32}` (Çekince 3) |
| Endpoint | 31 (4 public + 23 auth + 4 admin) |
