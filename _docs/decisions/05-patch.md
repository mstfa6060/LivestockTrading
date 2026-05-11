# Karar 5 — Konsolide Patch Dokümanı

**Status:** FINAL
**Amaç:** Karar 5 boyunca farklı modüllerin doc'larını etkileyen 6 patch grubu (3 ek patch ile toplam 9 patch grubu). Implementer Karar 5/X doc'unu okurken bu patch'i de okur.

## İlişkili Kararlar

- **Üst:** [Karar 5 modülleri](05-modules/)
- **Application order:** Wave 0 implementation öncesi tüm patch'ler uygulanmış olmalı

---

## Patch Application Order

```
1. Patch 1 (Identity v2)       — modül başı, UserPreferences VO ek
2. Patch 4 (Listings)           — IListingsCommands tanımı (interface only)
3. Patch 2 (Subscription)       — Interface + PaymentMethod entity
4. Patch 3 (Catalog revize)     — Brand AR, Location reference (Catalog v2 doc'unda detay)
5. Patch 5 (Görev 3 düzeltme)   — doc-only, event ownership netleştirme
6. Patch 6 (Görev 4 sayım)      — doc-only AR sayım
7. Patch 7 (Interface listesi)  — referans
8. Patch 8 (Backlog snapshot)   — Karar 6/7 planning input
9. Patch 9 (Faz 2 schema-ready) — referans, migration disipline
```

---

## Patch 1 — Identity v2

### 1.1 UserPreferences VO Genişletme

`Shared/ValueObjects/Identity/UserPreferences.cs`:

```csharp
public sealed record UserPreferences(
    LanguageCode Locale,
    CurrencyCode Currency,
    CountryCode Country,
    string TimeZone,
    bool ReadReceiptsEnabled = true,           // YENİ (Messaging #141)
    bool TypingIndicatorEnabled = true);       // YENİ (Messaging #141)
```

**Etki:**
- Identity v2 Madde 2 `User.Preferences` VO genişletildi
- Messaging `MarkMessageReadHandler` opt-out check
- `PATCH /identity/users/me/preferences` body'ye iki bool eklendi
- Schema migration: `identity.users.preferences_read_receipts_enabled BOOL NOT NULL DEFAULT true`, aynı şekilde `typing_indicator_enabled`

### 1.2 User AR — Email Change Pattern

```csharp
public class User
{
    public EmailAddress? PendingEmail { get; private set; }           // YENİ
    public DateTimeOffset? PendingEmailRequestedAt { get; private set; }  // YENİ
    
    public EmailVerificationToken RequestEmailChange(EmailAddress newEmail, TimeSpan ttl) { ... }
    public void ConfirmEmailChange(EmailAddress confirmedNewEmail) { ... }
    public void CancelPendingEmailChange() { ... }
}
```

**Schema migration:**
```sql
ALTER TABLE identity.users 
    ADD COLUMN pending_email VARCHAR(320) NULL,
    ADD COLUMN pending_email_requested_at TIMESTAMPTZ NULL;
```

**Cron cleanup (Backlog #80):** Daily Quartz job — `pending_email_requested_at < now - 24h` → reset to NULL.

**Endpoint güncellemesi:** Mevcut `PATCH /identity/users/me` body'ye `email?: string` eklendi.

### 1.3 GDPR Data Export Job

`DataExportJob` AR Identity'de:
- Schema: `identity.data_export_jobs (id, user_id, format, status, artifact_url, requested_at, completed_at)`
- Status enum: Pending, Running, Succeeded, Failed
- Worker: `IDataExportContributor` per modül collect → bundle → GPG encrypt → MinIO → email signed URL

Rate limit: max 1 export request per user per 24h.

### 1.4 Görev 3 İlişkili Düzeltme

Delivery feedback event'leri (EmailBounced/SmsDeliveryFailed/PushTokenInvalidated) producer = Notifications, consumer = Identity. **Detay Patch 5.**

---

## Patch 2 — Subscription

### 2.1 ISubscriptionCommands Interface Genişletme

```csharp
namespace Shared.Contracts.Subscription;

public interface ISubscriptionCommands
{
    // Quota consumes (Listings, Messaging)
    Task<Result> ConsumeRefreshQuotaAsync(Guid userId, CancellationToken ct);
    Task<Result> ConsumeTranslationQuotaAsync(Guid userId, CancellationToken ct);
    
    // Deal payment & refund (Marketplace)
    Task<Result<ChargeOutcome>> ChargeAsync(
        Guid userId, string paymentMethodId, Money amount, 
        string description, string idempotencyKey, CancellationToken ct);
    Task<Result> RefundAsync(string chargeId, Money? amount, string reason, CancellationToken ct);
    
    // Commission charge (DealCompleted cascade)
    Task<Result> ChargeCommissionAsync(
        Guid sellerUserId, Guid dealId, Money commissionAmount, CancellationToken ct);
}

public sealed record ChargeOutcome(string ChargeId, DateTimeOffset ChargedAt, string PaymentMethodLast4);
```

### 2.2 ISubscriptionReadService Interface Genişletme

```csharp
public interface ISubscriptionReadService
{
    // ... mevcut method'lar
    
    Task<int> GetRefreshQuotaRemainingAsync(Guid userId, CancellationToken ct);
    Task<int> GetTranslationQuotaRemainingAsync(Guid userId, CancellationToken ct);
    Task<IReadOnlyList<PaymentMethodDto>> GetPaymentMethodsAsync(Guid userId, CancellationToken ct);
    Task<PaymentMethodDto?> GetDefaultPaymentMethodAsync(Guid userId, CancellationToken ct);
}

public sealed record PaymentMethodDto(
    string Id, PaymentMethodKind Kind, string Last4,
    string? BrandName, int? ExpiryMonth, int? ExpiryYear, bool IsDefault);
```

### 2.3 PlanFeatures VO Genişletme (Listings Çekince 1)

```csharp
public sealed record PlanFeatures(
    int MaxActiveListings,
    int MaxFarms,
    int BoostSlotsIncluded,
    int ListingRefreshesPerMonth,        // YENİ — refresh quota
    int AiTranslationsPerMonth,          // YENİ — Listings + Messaging shared
    bool PrioritySupport,
    bool AnalyticsAccess,
    bool BulkImportAllowed);
```

**Plan seed güncellemesi:**

| Plan | RefreshesPerMonth | AiTranslationsPerMonth |
|---|---|---|
| free | 1 | 0 |
| standard | 10 | 5 |
| pro | -1 (unlimited) | -1 |
| enterprise | -1 | -1 |

### 2.4 PaymentMethod Entity — Universal Sahiplik

Subscription modülü "Billing" rolünü üstleniyor — buyer **ve** seller için tek payment method storage.

```csharp
public class PaymentMethod
{
    public Guid Id, UserId;
    public string StripePaymentMethodId;
    public PaymentMethodKind Kind;
    public string Last4;
    public string? BrandName;
    public int? ExpiryMonth, ExpiryYear;
    public bool IsDefault;
    public DateTimeOffset CreatedAt;
    public DateTimeOffset? RemovedAt;
}
```

DB: `UNIQUE(user_id, stripe_payment_method_id)`.

Marketplace'in kendi PM endpoint'i **YOK** — frontend `/subscription/me/payment-methods` çağırır.

### 2.5 PaymentProvider Interface Genişletme

```csharp
public interface IPaymentProvider
{
    // ... mevcut
    
    Task<PaymentMethodAttachResult> AttachPaymentMethodAsync(string customerId, string paymentMethodToken, CancellationToken ct);
    Task<bool> DetachPaymentMethodAsync(string paymentMethodId, CancellationToken ct);
    Task<Result<RefundOutcome>> RefundAsync(string chargeId, Money? amount, CancellationToken ct);
    Task<Result<ChargeOutcome>> ChargeForDealAsync(
        string customerId, string paymentMethodId, Money amount, 
        string description, string idempotencyKey, CancellationToken ct);
}
```

Faz 1 `MockPaymentProvider`, Faz 2 `StripePaymentProvider`.

---

## Patch 3 — Catalog Revize (Özet — Tam Doc 05-catalog.md)

### Özet İmpact

| Değişiklik | Etki |
|---|---|
| `Brand` AR eklendi | Catalog AR 2 → 4 |
| `Location` reference entity (INT PK, 5-level) | TR seed ~885K; cross-modül `LocationId` int reference |
| `BorderRule` AR Faz 2 placeholder | Schema-ready, feature pasif |
| `CategoryAttribute` child entity (Category AR içinde) | Listings için attribute schema; `GET /catalog/categories/{code}` response.attributes |
| Currency rate provider | TCMB primary + ECB fallback + exchangerate.host tier 3 |
| Catalog public event sayısı | 4 → 7 (+3 Brand × Approved/Deactivated/Reactivated); +3 Faz 2 BorderRule |
| Katalog endpoint | 27 → 33 (+6 Brand/Location/BorderRule/CategoryAttribute admin) |

**Detay:** [05-modules/05-catalog.md](05-modules/05-catalog.md).

---

## Patch 4 — Listings

### 4.1 IListingsCommands Interface

```csharp
namespace Shared.Contracts.Listings;

public interface IListingsCommands
{
    Task<Result> ReserveAsync(Guid listingId, Guid dealId, CancellationToken ct);
    Task<Result> UnreserveAsync(Guid listingId, Guid dealId, CancellationToken ct);
    Task<Result> MarkSoldAsync(Guid listingId, Guid dealId, Guid? buyerUserId, CancellationToken ct);
}
```

Marketplace cross-module sync call eder (OfferAccepted, DealCancelled, DealCompleted cascade).

### 4.2 IListingsReadService Interface Genişletme

```csharp
public interface IListingsReadService
{
    // ... mevcut
    
    Task<BoostAnalyticsSlice?> GetBoostAnalyticsAsync(
        Guid listingId, DateTimeOffset since, DateTimeOffset until, CancellationToken ct);
}

public sealed record BoostAnalyticsSlice(int ViewCount, decimal IncreasePercent, int? ClickThroughCount);
```

Subscription `BoostCampaignDetailResponse` (Çekince 3) için.

### 4.3 Approved Listing Edit Policy (Backlog #2 Kapanış)

| Field | Edit Edilebilir? | Re-approval? |
|---|---|---|
| Title, Description | ✓ | ✗ |
| AnimalCount, AverageWeightKg/AgeMonths | ✓ | ✗ |
| Price | ✓ (separate `UpdatePrice` method) | ✗ — event fire |
| Images add/remove | ✓ | ✗ |
| **CategoryId, BreedId** | ✗ — yeni listing | — |
| **Location** | ⚠️ Re-approval zorunlu | ✓ status → PendingReview |
| **FarmId, SellerId** | ✗ Immutable | — |

### 4.4 Listing.BrandId Field

```csharp
public class Listing
{
    public Guid? BrandId { get; private set; }                    // YENİ — opsiyonel
    public void SetBrand(Guid? brandId) { /* cross-module validator */ }
}
```

**Schema migration:**
```sql
ALTER TABLE listings.listings ADD COLUMN brand_id UUID NULL;
CREATE INDEX ix_listings_brand_id ON listings.listings(brand_id);
-- NO foreign key (cross-modül FK yasağı; logical only)
```

**Validator:**
```csharp
RuleFor(x => x).MustAsync(async (cmd, ct) =>
    cmd.BrandId is null 
    || await _catalog.IsValidBrandForCategoryAsync(cmd.BrandId.Value, cmd.CategoryId, ct))
    .WithMessage("Brand kategoride desteklenmiyor veya inactive");
```

Catalog `ICatalogReadService` ek method `IsValidBrandForCategoryAsync`.

### 4.5 AI Quota Birleşim (Çekince 2)

| Operation | Quota | Rate Limit |
|---|---|---|
| AI Translation | ✓ `PlanFeatures.AiTranslationsPerMonth` | 1/sn per listing |
| AI Tagging | ✗ Quota yok | 1/dk per listing (Redis SETNX) |

---

## Patch 5 — Görev 3 Communication Matrix

### 5.1 Producer Düzeltmesi — Delivery Feedback

**Önceki yanlış:** Identity producer (Görev 3 Q1).
**Doğru:** Producer = Notifications, Consumer = Identity.

| Event | **Producer** | Consumer |
|---|---|---|
| `EmailBounced` | **Notifications** | Identity (email_verified=false) |
| `SmsDeliveryFailed` | **Notifications** | Identity (phone flag) |
| `PushTokenInvalidated` | **Notifications** | Identity (UserDevice.PushToken=null) |

### 5.2 Yeni Public Event'ler (Karar 5 boyunca eklendi)

| Event | Producer | Tetikleyici |
|---|---|---|
| `VetVerified` | Accounts | Identity v2 Q3 |
| `VetSuspended` | Accounts | |
| `VetReactivated` | Accounts | |
| `ReviewWritten` | Accounts | D6 |
| `EmailBounced` | Notifications | Email provider bounce webhook |
| `SmsDeliveryFailed` | Notifications | Twilio status callback |
| `PushTokenInvalidated` | Notifications | FCM/APNS invalid token |
| `BrandApproved` | Catalog (v2) | Brand suggest workflow |
| `BrandDeactivated` | Catalog (v2) | |
| `BrandReactivated` | Catalog (v2) | |

### 5.3 Hot Path Güncellemeleri

**UserDeleted: 9 consumer** (GDPR cascade):

```
UserDeleted (Identity)
   ├─ Accounts: Seller + Farm + VetProfile + Review anonimize
   ├─ Carrier: Carrier + Fleet + Driver anonimize
   ├─ Listings: ilanları anonimleştir
   ├─ Marketplace: Offer/Deal/Dispute participant anonimize
   ├─ Messaging: Conversation participant anonimize, UserBlock cleanup
   ├─ Subscription: aktif subscription cancel + invoice void + boost cancel
   ├─ Notifications: prefs sil, in-app temizle, queue
   ├─ Admin: AdminAuditLog
   └─ Identity self: RefreshTokens + Devices + ExternalLogins + Consents delete
```

**DealCompleted: 6 consumer** (Listings/Subscription/Carrier/Messaging/Notifications/Admin).

### 5.4 Public Event Final Sayım

```
Identity:       6
Accounts:       11 (Seller × 7 + Vet × 3 + Review × 1)
Carrier:        8
Catalog:        7 (Faz 1 active; +3 Faz 2 BorderRule = 10)
Listings:       11
Marketplace:    13 (6 Offer + 7 Deal — Agreement* → Deal* rename)
Messaging:      4
Subscription:   14
Notifications:  3 (delivery feedback — producer)
Admin:          0 (terminal)

TOPLAM Faz 1:   80
```

### 5.5 Marketplace Deal Rename

| Yer | Eski (Agreement) | Yeni (Deal) |
|---|---|---|
| AR sınıfı | `Agreement` | `Deal` |
| Tablo | `marketplace.agreements` | `marketplace.deals` |
| FK referans | `agreement_id` | `deal_id` |
| Cross-modül ID ref | `AgreementId` | `DealId` |
| Domain events | `Agreement*` | `Deal*` |
| Shared/Events/Marketplace klasörü | Agreement*.cs | Deal*.cs |
| `IAdminMarketplaceCommands.ResolveAgreementDispute` | | `ResolveDealDispute` |
| `IMarketplaceReadService.GetAgreementByIdAsync` | | `GetDealByIdAsync` |

**Etki:** Faz 1'in ilk migration'ında direkt `marketplace.deals` ile başla (Agreement hiç oluşturulmadı varsayımı).

---

## Patch 6 — Görev 4 AR Listesi

### 6.1 AR Sayım Güncelleme: 28 → 30

| Modül | Önceki | Patch | Değişim |
|---|---|---|---|
| Identity | 1 | 1 | — |
| Accounts | 2 (Seller, Farm) | **4** (+ VetProfile, + Review) | +2 |
| Catalog | 4 | 4 | — |
| Carrier | 3 | 3 | — |
| Subscription | 5 | 5 | — |
| Listings | 2 | 2 | — |
| Marketplace | 4 | 4 | — |
| Messaging | 1 | 1 | — |
| Notifications | 2 | 2 | — |
| Admin | 4 | 4 | — |
| **Toplam AR** | **28** | **30** | **+2** |

### 6.2 Reference Entity (INT PK)

5 (Country, Currency, Language, CertificationType, Location — v2 yeni).

### 6.3 Logging-Style Entity (AR Değil)

AdminAuditLog (Admin), DeliveryAttempt (Notifications), ReportExecution (Admin child), MessageReport (Messaging), ListingReport (Listings), UserBlock junction (Messaging), BrandCategory junction (Catalog), RateLog (Catalog).

---

## Patch 7 — Cross-Module Interface Konsolide Liste

### Read Services (Shared/Contracts/{Module}/)

10 interface (modül başına 1, Identity'de 2: `ICurrentUserService` scoped + `IIdentityReadService`).

### Commands (Sync Cross-Modül Write)

3 interface:
- `IListingsCommands` (Marketplace → Listings)
- `ICarrierShipmentCommands` (Marketplace → Carrier)
- `ISubscriptionCommands` (Listings/Messaging → quota; Marketplace → charge)

### Admin (Sync) — 10 × 2 = 20 Interface

| Modül | Read + Commands |
|---|---|
| Identity | IAdminUserReadService + IAdminUserCommands |
| Accounts/Seller | IAdminSellerReadService + IAdminSellerCommands |
| Accounts/Vet | IAdminVetReadService + IAdminVetCommands |
| Carrier | IAdminCarrierReadService + IAdminCarrierCommands |
| Catalog | IAdminCatalogReadService + IAdminCatalogCommands |
| Listings | IAdminListingReadService + IAdminListingCommands |
| Marketplace | IAdminMarketplaceReadService + IAdminMarketplaceCommands |
| Messaging | IAdminMessagingReadService + IAdminMessagingCommands |
| Notifications | INotificationsReadService (Shared) + IAdminNotificationsCommands |
| Subscription | IAdminSubscriptionReadService + IAdminSubscriptionCommands |

### Cross-Cutting

| Interface | Modül |
|---|---|
| `IFileStorage` | Shared/Contracts/Storage (MinIO) |
| `IFeatureFlagService` | Admin (cross-cutting consumer) |
| `IDataExportContributor` | Per modül implement, Identity orchestrator |
| `IAdminAuditService` | Admin (modüller IAdminXCommands'tan çağırır) |
| `IPaymentProvider` | Subscription (internal) |
| `IEmailProvider`, `ISmsProvider`, `IPushProvider` | Notifications (internal) |
| `IGeoIpService` | Identity (sessions enrichment) |

---

## Patch 8 — Backlog Status Snapshot

### Karar 6'ya Taşınanlar (~13 item)

| # | Konu |
|---|---|
| 5 | Realtime Push Catalog (event → SignalR target → client group) |
| 16 | Cross-modül raporlama exception inventory |
| 30 | GeoJSON serialization paketi |
| 31 | Map UI component selection (frontend) |
| 33 | MultiPolygon GeoJSON 4-derinlik şema doğrulama |
| 47 | Cursor pagination Shared utility |
| 48 | Locale resolution middleware |
| 52 | Admin endpoint route convention |
| 76 | Consent versioning workflow UX |
| 116 | Listing slug strategy global vs per-seller |
| 125 | Counter offer chain max depth UX |
| 132 | `/marketplace/me/payment-methods` alias |

### Karar 7'ye (Operations/DevOps/Observability) ~150 item

Detay: [backlog.md](backlog.md).

### Karar 5 Boyunca Kapanan (33 item)

#1, #2, #3, #5 (kısmen), #6, #7 (kısmen), #8, #9, #10, #12, #13, #18, #19, #20, #22, #36, #49, #50, #51, #53-55, #65, #80, #81, #82, #83, #90, #103, #112, #113, #121, #123, #142

---

## Patch 9 — Schema-Ready Faz 2 Liste

| Schema Field | Modül | Faz 1 | Faz 2 Aktivasyon |
|---|---|---|---|
| `User.TotpSecretEncrypted` + `TwoFactorEnabled` | Identity | Sütun var | 2FA endpoint'leri aktif |
| `User.NationalId` | Identity | Self-declared | NVI sync |
| `Carrier.Vehicle.HasGps` + Driver.UserId | Carrier | Field var | GPS streaming + driver mobile login |
| `Listing.AiQualityScore / AiModerationScore / AiTagsJson` | Listings | Sütun var | ML pipeline |
| `BorderRule.*` | Catalog | Schema-ready, feature pasif | Gümrük entegrasyonu |
| `Deal.EscrowAmount` real money | Marketplace | State tracking | Stripe escrow gerçek |
| `Dispute.Resolution.RefundPartial` | Marketplace | Enum var | Partial refund logic |
| `Plan.CommissionRule` per-category | Subscription | Plan default rate | Per-category override |
| `BoostCampaign.CancellationProrate` | Subscription | Cancel = immediate expire | Prorate refund |
| `NotificationPreference` ML priority | Notifications | Static | ML smart digest |
| `NotificationTemplate` Razor/Liquid | Notifications | `{{var}}` substitution | Conditional logic |
| `WhatsApp` DeliveryChannel | Notifications | Enum yok | Channel.WhatsApp |
| `ScheduledReport.CustomDapperQuery` | Admin | Enum yok | Sandboxed SQL |
| `ImpersonationSession` 2FA enforcement | Admin | Plain start | 2FA confirm |
| `AdminAuditLog` aspect interceptor | Admin | Explicit | `[AuditAction]` attribute |
| `Listing.Translations` AI auto-fill | Listings | Manuel TR/EN | AI worker tüm dil |
| `Catalog.MinistrySync` | Accounts/Catalog | Quartz job disabled | Bakanlık HBS |
| `Apple Email Relay` deactivation webhook | Identity | Skeleton | Real handler |

---

## Doc Coverage Doğrulama

| Karar | Doc | Patch |
|---|---|---|
| Karar 5/Catalog v2 | 05-catalog.md (revize uygulandı) | Patch 3 referans |
| Karar 5/Identity v2 | 05-identity.md | + Patch 1 |
| Karar 5/Accounts v2 | 05-accounts.md | + Patch 6 sayım |
| Karar 5/Carrier | 05-carrier.md | + Çekince 1 (BusinessInfo Shared) |
| Karar 5/Subscription | 05-subscription.md | + Patch 2 |
| Karar 5/Listings | 05-listings.md | + Patch 4 |
| Karar 5/Marketplace | 05-marketplace.md | + Deal rename |
| Karar 5/Messaging | 05-messaging.md | OK |
| Karar 5/Notifications | 05-notifications.md | + Patch 5 producer |
| Karar 5/Admin | 05-admin.md | OK |

**Toplam: 10 modül doc + 1 patch doc = 11 referans doküman.**
