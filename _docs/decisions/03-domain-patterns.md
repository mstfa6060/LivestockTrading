# Karar 3 — Domain Patterns (AR + Event + Cross-Modül + VO)

**Status:** FINAL
**Karar tarihi:** Planning sessions #3 (5 alt-karar: 3a, 3b, 3c, 3d, 3e)

## İlişkili Kararlar

- **Üst:** [Karar 2 — Modül listesi](02-modules-list.md)
- **Alt:** [Karar 5 modül detayları](05-modules/), [Karar 4 migration patterns](04-migration.md)
- **Patch:** [05-patch.md](05-patch.md) — Patch 5 (Communication Matrix güncellemesi), Patch 6 (AR sayım 28 → 30)

---

## Bölüm 1: Aggregate Root Listesi (3a)

### AR Olma Kriteri (4 koşul)

1. **Bağımsız lifecycle** — Parent olmadan oluşturulabilir mi, var olabilir mi?
2. **Invariant sahipliği** — İçindeki birden fazla entity/value arasında atomik tutulması gereken kural var mı?
3. **Transaction sınırı** — Bu nesne üzerindeki tüm değişiklikler tek transaction'da mı?
4. **Dış referans yüzeyi** — Başka kod buna ID ile mi atıfta bulunuyor (içine ulaşmadan)?

**Rule of thumb:**
> "Bu nesne kendi başına anlamlı bir şey ifade ediyor mu?"
> 
> Evet → AR adayı.
> Hayır, hep parent'ıyla anlam kazanıyor → Entity (içeride).

### AR Listesi (30 — Görev 4 sonrası)

Detay: [02-modules-list.md / Bölüm 5](02-modules-list.md#bölüm-5-görev-4--aggregate-root-listesi-30-ar--5-reference).

| Modül | AR Sayısı | AR'lar |
|---|---|---|
| Identity | 1 | User |
| Accounts | 4 | Seller, Farm, VetProfile, Review |
| Carrier | 3 | Carrier, Shipment, CarrierOffer |
| Catalog | 4 | Category, Breed, Brand, BorderRule (+ 5 reference entity) |
| Listings | 2 | Listing, SavedSearch |
| Marketplace | 4 | Offer, Deal, Favorite, Dispute |
| Messaging | 1 | Conversation |
| Notifications | 2 | NotificationPreference, InAppNotification |
| Subscription | 5 | Plan, Subscription, Invoice, BoostPackage, BoostCampaign |
| Admin | 4 | FeatureFlag, ScheduledReport, SystemVersion, ImpersonationSession |
| **TOPLAM** | **30** | |

### Reference Entity (INT PK, AR Değil)

| Entity | Modül | Seed Boyutu | Cross-modül Ref |
|---|---|---|---|
| Country | Catalog | 250 (ISO 3166-1) | String code (`"TR"`) |
| Currency | Catalog | 180 (ISO 4217) | String code (`"TRY"`) |
| Language | Catalog | 50 | String code (`"tr"`) |
| CertificationType | Catalog | 12 + admin extend | String code kebab-case |
| Location | Catalog | TR ~885K Faz 1 | **INT id** (Karar 3e istisnası) |

### Logging-Style Entity (AR Değil)

| Entity | Modül | Pattern |
|---|---|---|
| AdminAuditLog | Admin | append-only insert; UPDATE/DELETE Faz 2 DB-level revoke |
| DeliveryAttempt | Notifications | append-only with status transition |
| ReportExecution | Admin | child of ScheduledReport, append-only |
| MessageReport | Messaging | within Conversation context, lifecycle var ama AR'lık değer yok |
| ListingReport | Listings | aynı |
| UserBlock | Messaging | junction (composite unique), no lifecycle |
| BrandCategory | Catalog | junction (many-to-many) |
| RateLog | Catalog | append-only audit (currency rate fetch history) |

### Pragmatik AR İstisnaları

| AR | İstisna Sebebi |
|---|---|
| Favorite | Invariant zayıf — sadece unique constraint. AR sebebi: kendi tablosu, cross-modül, command-driven (Add/Remove) |
| InAppNotification | Invariant zayıf. AR sebebi: kullanıcı doğrudan lifecycle yönetiyor (read/archive) |

---

## Bölüm 2: Domain Event Listesi (3b — 80 Public + Internal)

### Public vs Internal Kriteri

**Public** (`Shared/Events/`) olma şartı: en az bir **başka modül** bu event'e reaksiyon verecek.

### Identity (6 Public + Internal)

| Event | Payload | Tip |
|---|---|---|
| `UserRegistered` | UserId, Email, PreferredLocale, PreferredCurrency, CountryId | **Public** |
| `UserEmailVerified` | UserId, Email, VerifiedAt | **Public** |
| `UserPasswordChanged` | UserId, ChangedAt, IpAddress | **Public** (Notifications security mail) |
| `UserSuspended` | UserId, Reason, SuspendedUntil?, ActorUserId | **Public** |
| `UserReactivated` | UserId, ActorUserId | **Public** |
| `UserDeleted` | UserId, DeletedAt | **Public** (9 consumer GDPR cascade) |
| `UserLoggedIn` | UserId, Platform, IpAddress, UserAgent | Internal |
| `UserPreferencesChanged` | UserId, Locale, Currency, CountryId | Internal |
| `RefreshTokenRotated` | UserId, OldTokenId, NewTokenId | Internal |
| `RefreshTokenRevoked` | UserId, TokenId, Reason | Internal |
| `UserPendingDeletion` | UserId, ScheduledFor | Internal (30-day grace warning) |
| `UserConsentRecorded` | UserId, ConsentType, Version | Internal |

### Accounts (11 Public)

| Event | Tip | Consumer |
|---|---|---|
| `SellerOnboardingSubmitted` | Public | Notifications (moderator queue) |
| `SellerVerified` | Public | Identity (role grant), Notifications |
| `SellerSuspended` | Public | Listings, Marketplace, Subscription, Notifications, Admin (5 consumer) |
| `SellerReactivated` | Public | Notifications |
| `SellerSubscriptionActivated` (eski) | → Subscription'a taşındı (Patch 5) |
| `SellerSubscriptionExpired` (eski) | → Subscription'a taşındı |
| `SellerBoostPurchased` (eski) | → Subscription'a taşındı |
| `VetVerified` | Public (Identity v2 Q3) | Identity (vet role grant), Notifications |
| `VetSuspended` | Public | Identity (role revoke), Notifications |
| `VetReactivated` | Public | Identity, Notifications |
| `ReviewWritten` | Public (Accounts v2 D6) | Notifications (review edilen kullanıcıya), Admin (rating aggregate) |
| `SellerCertificationExpired` | Public (D8) | Notifications (seller uyarı) |
| `FollowerAdded` (Internal Faz 1) | Internal | Faz 2 notification |

### Carrier (8 Public)

| Event | Tip | Consumer |
|---|---|---|
| `CarrierOnboardingSubmitted` | Public | Notifications, Admin queue |
| `CarrierVerified` | Public | Identity (role grant), Notifications |
| `CarrierSuspended` | Public | Marketplace (active shipments flag — sync), Notifications |
| `CarrierReactivated` | Public | Notifications |
| `CarrierShipmentCreated` | Public | Marketplace (Deal status), Notifications |
| `CarrierShipmentPickedUp` | Public | Marketplace, Notifications |
| `CarrierShipmentDelivered` | Public | Marketplace, Notifications (confirm prompt) |
| `CarrierShipmentCancelled` | Public | Marketplace (Deal handling), Notifications |
| `CarrierZoneUpdated` | Internal | — |
| `CarrierRateUpdated` | Internal | — |

### Catalog (7 Public Faz 1 + 3 Faz 2)

| Event | Tip |
|---|---|
| `CategoryDeactivated` | Public |
| `CategoryReactivated` | Public |
| `BreedDeactivated` | Public |
| `BreedReactivated` | Public |
| `BrandApproved` | Public (Catalog v2) |
| `BrandDeactivated` | Public |
| `BrandReactivated` | Public |
| `BorderRuleCreated/Updated/Deactivated` | Faz 2 activate |
| `CategoryCreated/Renamed/Moved`, `BreedCreated`, `BrandSuggested` | Internal |

### Listings (11 Public)

| Event | Tip |
|---|---|
| `ListingSubmittedForReview` | Public |
| `ListingApproved` | Public (SavedSearch matcher tetikler) |
| `ListingRejected` | Public |
| `ListingPaused` | Public |
| `ListingResumed` | Public |
| `ListingSold` | Public (Marketplace closes offers, Notifications favorites) |
| `ListingExpired` | Public |
| `ListingDeleted` | Public (3 consumer cascade) |
| `ListingPriceChanged` | Public (favori sahipleri "fiyat düştü" notification) |
| `ListingReportFiled` | Public (admin moderation queue) |
| `SavedSearchMatchFound` | Public (Notifications fan-out) |
| `ListingDrafted`, `ListingImageAdded`, `ListingAttributeChanged`, `ListingRefreshed`, `ListingTagged`, `ListingTranslated` | Internal |

### Marketplace (13 Public — Deal rename uygulandı)

#### Offer (6 Public)

| Event | Tip |
|---|---|
| `OfferSubmitted` | Public |
| `OfferAccepted` | Public (3 consumer + Deal created same TX) |
| `OfferRejected` | Public |
| `OfferWithdrawn` | Public |
| `OfferCounterProposed` | Public |
| `OfferExpired` | Public |

#### Deal (7 Public — eski Agreement* rename)

| Event | Tip | Eski isim |
|---|---|---|
| `DealPaymentConfirmed` | Public | AgreementPaymentConfirmed |
| `DealShipmentStarted` | Public | AgreementShipmentStarted |
| `DealDelivered` | Public | AgreementDelivered |
| `DealCompleted` | Public (6 consumer cascade) | AgreementCompleted |
| `DealDisputed` | Public | AgreementDisputed |
| `DealResolved` | Public | AgreementResolved |
| `DealCancelled` | Public | AgreementCancelled |
| `DealCreated` | Internal (Görev 3 Q1 — Offer payload'da DealId yeter) | AgreementCreated |

### Messaging (4 Public)

| Event | Tip |
|---|---|
| `ConversationStarted` | Public |
| `MessageSent` | Public (yüksek frekans, content payload'da yok — privacy) |
| `MessageRead` | Public (opt-out filter — Identity prefs) |
| `MessageReportFiled` | Public (admin moderation queue) |
| `ConversationArchived`, `MessageEdited`, `MessageDeleted` | Internal |
| `TypingIndicator` | (Event bus'a girmez — pure SignalR) |

### Subscription (14 Public)

| Event | Tip |
|---|---|
| `SubscriptionActivated` | Public |
| `SubscriptionRenewed` | Public |
| `SubscriptionExpired` | Public (4 consumer) |
| `SubscriptionCancelled` | Public |
| `SubscriptionUpgraded` | Public |
| `SubscriptionDowngraded` | Public |
| `InvoiceIssued` | Public |
| `InvoicePaid` | Public |
| `InvoiceFailed` | Public (7-day grace cascade) |
| `InvoiceRefunded` | Public |
| `BoostActivated` | Public (Listings apply flag) |
| `BoostExpired` | Public |
| `BoostCancelled` | Public |
| `CommissionCharged` | Public (Notifications seller, Admin revenue) |

### Notifications (3 Public — Delivery Feedback)

Patch 5 düzeltmesi: Producer = Notifications, Consumer = Identity.

| Event | Tip | Consumer |
|---|---|---|
| `EmailBounced` | Public | Identity (email_verified=false) |
| `SmsDeliveryFailed` | Public | Identity (phone flag) |
| `PushTokenInvalidated` | Public | Identity (UserDevice.PushToken=null) |
| `NotificationPreferenceUpdated`, `InAppNotificationCreated/Read/Archived` | Internal |

### Admin (0 Public)

Terminal modül. Internal: `FeatureFlagToggled`, `ImpersonationStarted`, `AdminAuditLogRecorded`.

### Toplam

```
Identity:       6
Accounts:       11
Carrier:        8
Catalog:        7 (+ 3 Faz 2)
Listings:       11
Marketplace:    13
Messaging:      4
Subscription:   14
Notifications:  3
Admin:          0
TOPLAM Faz 1:   80
```

---

## Bölüm 3: Communication Matrix (3c)

Detay: [02-modules-list.md / Bölüm 4 (Görev 3)](02-modules-list.md#bölüm-4-görev-3--communication-matrix-80-public-event).

### Cascade Chain Özet

| Chain | Consumer Sayısı | Hot Path |
|---|---|---|
| UserDeleted (GDPR) | 9 | 🔥🔥 |
| DealCompleted (commission cascade) | 6 | 🔥 |
| SellerSuspended | 5 | 🔥 |
| ListingDeleted | 3 | 🔥 |
| OfferAccepted | 3 + Deal co-creation | 🔥 hot freq |
| CarrierSuspended | 4 (hybrid sync+async) | 🔥 |
| InvoiceFailed | 4 (7-day grace) | 🔥 |
| SubscriptionExpired | 4 | 🔥 |
| MessageSent | 1 cross-modül | Hot freq |

---

## Bölüm 4: Aggregate Boundary Rules (3d)

### Kural 1: ID-Only Cross-AR Reference

AR'lar arası referans **sadece ID** üzerinden. Navigation property **yasak**.

```csharp
// ✓ DOĞRU
public class Listing : AggregateRoot
{
    public Guid SellerId { get; private set; }   // Seller AR'a ID ile
    public int CategoryId { get; private set; }  // Category AR'a ID ile
}

// ✗ YANLIŞ
public class Listing : AggregateRoot
{
    public Seller Seller { get; private set; }   // navigation — AR sınırı ihlali
}
```

**EF Core configuration:**
```csharp
public class ListingConfiguration : IEntityTypeConfiguration<Listing>
{
    public void Configure(EntityTypeBuilder<Listing> b)
    {
        b.Property(x => x.SellerId).IsRequired();
        // Navigation YOK
        b.HasIndex(x => x.SellerId);   // sadece index
    }
}
```

**Cross-modül FK:** DB seviyesinde **yok** — modül sınırı schema seviyesinde de geçerli.

**AR İÇİ navigation:** Serbest — invariant'ları AR koruyor.

### Kural 2: Cross-Aggregate Transaction Yasağı

Bir command → bir AR değişikliği. Çoklu AR değişikliği gerekiyorsa event-driven.

**İstisna (AR Co-creation Pattern — Backlog #8):** Aynı modül + aynı transaction'da factory pattern ile yeni AR yaratma:

```csharp
public sealed class AcceptOfferHandler : IConsumer<AcceptOfferCommand>
{
    public async Task Consume(...)
    {
        var offer = await _offerRepo.GetById(cmd.OfferId);
        var deal = offer.Accept(userId, carrierAtt);   // ← Offer.Accept() Deal yaratır
        _dealRepo.Add(deal);
        // Aynı SaveChanges + outbox
    }
}
```

**Cross-modül write** mutlaka eventually consistent (outbox + event) veya **kasıtlı sync command** (`IListingsCommands.ReserveAsync`).

### Kural 3: AR Boyutu Disiplini

| Çok Küçük (Anemic) | Sweet Spot | Çok Büyük |
|---|---|---|
| Sadece getter/setter | 1-3 child entity tipi | 5+ child collection |
| Logic service'lerde | Method'lar AR state'ini koruyor | Loading saniyeleri |
| Invariant yok | 2-5 invariant | Method tiny subset |

**Logical vs Physical:** AR'ın tüm child'larını her zaman load etmeyiz. Conversation içinde Message — logical AR, physically paginated.

### Kural 4: Refactor Heuristic

**Entity → AR (Promote):**
- Lifecycle parent'tan diverge
- Cross-modül ID referansı gerekiyor
- Child collection unbounded
- Kendi business event'leri üretiyor

**AR → Entity (Demote):**
- Hep parent ile yaratılıyor
- Dış referans hiç yok
- Lifecycle parent'a tam bağlı

### Kural 5: Repository Pattern — Hybrid

```
Modules/{X}/X.Infrastructure/
  Persistence/
    XDbContext.cs
    Configurations/
    Repositories/        ← thin per-AR
  Persistence/
    Migrations/          ← per modül
  ReadModels/            ← Dapper hot path
```

- DbContext per modül
- Thin repository per AR (`GetById`, `Add`, `Remove`)
- Query method'lar repository'de **DEĞİL** — read model projectörlerinde
- Read tarafı: Dapper raw SQL veya EF projection (`Select(...)`)

### Kural 6: Domain Service Yerleşimi

```
Modules/{X}/X.Domain/
  Aggregates/
    {AR}/
  Services/                 ← BURADA
    SavedSearchMatcher.cs   ← cross-AR aynı modül içinde
  Events/
    Public/                 ← Shared/Events'e copy
    Internal/
```

**Cross-modül domain service YOK** — cross-modül = event + command.

---

## Bölüm 5: Value Object Inventory (3e — 24 VO)

### Shared/ValueObjects/ (8)

| VO | Şema | Validasyon |
|---|---|---|
| `CountryCode` | `Value: string` (uppercase 2 char) | ISO 3166-1 alpha-2, Catalog whitelist |
| `LanguageCode` | `Value: string` (lowercase 2 char) | ISO 639-1, Catalog whitelist |
| `CurrencyCode` | `Value: string` (uppercase 3 char) | ISO 4217, Catalog whitelist |
| `Money` | `Amount: decimal(18,4)`, `Currency: CurrencyCode` | Amount ≥ 0; arithmetic CurrencyCode mismatch → exception |
| `EmailAddress` | `Value: string` (normalized lowercase) | RFC 5322 lite + MailKit `MailboxAddress.TryParse` |
| `PhoneNumber` | `E164: string` + computed `CountryCallingCode`, `NationalNumber` | libphonenumber-csharp |
| `PersonName` | `First: string`, `Last: string`, `Middle: string?` | Non-empty, max 50, Unicode permissive |
| `Address` | `Country: CountryCode`, `City`, `AddressLine1`, optional fields | Country+City+AddressLine1 zorunlu |

**Karar 5/Carrier Çekince 1 sonrası eklendi:** `BusinessInfo`, `BankInfo`, `Iban` Shared'a taşındı (Accounts + Carrier ikisi de kullanıyor).

| Shared/ValueObjects/Business/ | Şema |
|---|---|
| `BusinessInfo` | LegalName, TradeName?, TaxNumber (VO), TaxOffice?, RegisteredAddress (Address), ContactEmail?, ContactPhone?; partial init supported |
| `BankInfo` | AccountHolder, Iban (VO), BankName, BankBic? |
| `Iban` | `Value: string` E.164-style normalized, ISO 13616 mod-97 |
| `TaxNumber` | TR: 10-digit VKN veya 11-digit TCKN |

**Karar 5/Identity NationalId VO:**

```csharp
public readonly record struct NationalId(string Value)
{
    public static NationalId Parse(string raw) { /* 11 digit + TC checksum */ }
}
```

### Modül-İçi VO'lar

#### Identity

| VO | Notlar |
|---|---|
| `UserPreferences` | Locale + Currency + Country + TimeZone + **ReadReceiptsEnabled** + **TypingIndicatorEnabled** (Messaging #141 patch) |
| `HashedPassword` | BCrypt; equality desteklemez (timing attack); `Verify(plaintext)` method |
| `ConsentGrant` | Type + Version + Granted (KVKK consent tracking) |

#### Accounts

| VO | Notlar |
|---|---|
| `OnboardingStatus` | State machine (Draft/Submitted/UnderReview/Verified/Rejected/Suspended) |
| `ServiceRate` | Rate (Money) + Unit (PerKm/PerHead/PerTrip/PerHour) + MinCharge (Money?) |
| `ReviewReply` | Body + ReplyAt |

#### Catalog

| VO | Notlar |
|---|---|
| `Translations` | `Map: IReadOnlyDictionary<LanguageCode, string>` (JSONB serialize); fallback resolve |

#### Listings

| VO | Notlar |
|---|---|
| `ListingLocation` | Point (PostGIS) + Country (CountryCode) + City + AddressDetail? + LocationId (Catalog ref) |
| `ListingStatus` | 9-state FSM (Draft/PendingReview/Active/Reserved/Paused/Sold/Rejected/Expired/Deleted) |
| `SavedSearchFilter` | CategoryId?, BreedId?, CountryCode?, PriceMin/Max?, Keyword?, Radius (GeoCircle)?; specification pattern `IsSatisfiedBy(projection)` |
| `GeoPoint`, `GeoCircle` | NTS Point + radius km |

#### Marketplace

| VO | Notlar |
|---|---|
| `OfferStatus` | 6-state (Pending/Accepted/Rejected/Withdrawn/Expired/Countered) |
| `DealStatus` | 8-state (PendingPayment → Paid → InPreparation → InTransit → Delivered → Completed; Disputed/Cancelled paralel) |
| `EscrowStatus` | Pending/Held/Released/Refunded |
| `DisputeResolution` | Kind (RefundFull/Partial/CompleteAsIs/Custom) + RefundAmount? + DecisionNote |
| `CarrierAttachment` | CarrierId + CarrierFee + RequestedPickupAt |

#### Messaging

| VO | Notlar |
|---|---|
| `MessageContent` | Type (Text/Image/Attachment/OfferLink/System) + Text?/MediaUrl?/OfferId?/SystemKey? |
| `Participant` | UserId + Role (Initiator/Recipient) |

#### Notifications

| VO | Notlar |
|---|---|
| `DeliveryChannel` | enum (Email=1, Sms=2, Push=4, InApp=8) bitwise flag |
| `ChannelMask` | `Mask: int` (DeliveryChannel OR) + `Has/Add/Remove` methods |
| `NotificationTemplate` (entity değil VO) | Key + Locale + Channel reference |
| `RenderContext` | `Values: IReadOnlyDictionary<string, object>` |

#### Subscription

| VO | Notlar |
|---|---|
| `PlanFeatures` | MaxActiveListings, MaxFarms, BoostSlotsIncluded, **ListingRefreshesPerMonth**, **AiTranslationsPerMonth** (Patch 2.3), PrioritySupport, AnalyticsAccess, BulkImportAllowed |

#### Carrier

| VO | Notlar |
|---|---|
| (BusinessInfo, BankInfo, ServiceRate Shared'a taşındı) | — |

### Toplam VO Sayım

| Yerleşim | Sayı |
|---|---|
| Shared/ValueObjects/ | 8 (CountryCode, LanguageCode, CurrencyCode, Money, EmailAddress, PhoneNumber, PersonName, Address) |
| Shared/ValueObjects/Business/ (Carrier Çekince 1) | 4 (BusinessInfo, BankInfo, Iban, TaxNumber) |
| Shared/Identity-spesifik | 1 (NationalId) |
| Modül-içi | ~20 |
| **TOPLAM** | **~33** |

---

## Bölüm 6: Money & Currency Coupling (Önemli)

### Karar (Karar 3e): ISO Code String, INT PK Catalog'a İç

| Yaklaşım | Money Field |
|---|---|
| ❌ `Money(amount, CurrencyId int)` | Semantik bağımlılık (Catalog INT seed'i) |
| ✅ `Money(amount, CurrencyCode string)` | ISO 4217 stable global identity |
| ❌ `Money(amount, Currency VO)` | Currency nerede yaşıyor? Catalog'la yarışıyor |

**Decoupling:**
- Catalog'un internal PK INT + UNIQUE ISO Code
- Admin işlemleri INT PK
- Dış dünyaya sadece Code
- Diğer modüller DB'de `price_currency CHAR(3)` (FK yok)
- Domain'de `Money(amount, CurrencyCode("USD"))`

**Aynı pattern Country, Language için.**

**Location istisnası:** INT PK (Catalog 3e madde 1):
- 5-level hierarchy + 800K+ kayıt
- Slug kebab-case TR köyleri için çakışma yüksek
- Frontend GET /catalog/locations/{id} pattern (slug yerine)

---

## Özet Tablo

| Konu | Karar |
|---|---|
| AR sayısı | 30 (+ 5 reference entity) |
| Cross-AR reference | ID-only, navigation yasak; AR İçi navigation serbest |
| Cross-aggregate transaction | Bir command = bir AR; istisna AR Co-creation Pattern (aynı modül + aynı TX, factory) |
| AR boyutu disiplini | 1-3 child entity; unbounded → physical paginate (logical AR) |
| Repository pattern | Hybrid — DbContext per modül + thin repo per AR + Dapper read model |
| Domain service | Modül-içi `{X}.Domain/Services/`; cross-modül asla |
| Public event sayısı Faz 1 | 80 |
| VO sayısı | ~33 (8 Shared + 4 Shared Business + ~20 modül-içi + NationalId) |
| Money/Currency | ISO Code string (CurrencyCode VO), INT PK Catalog iç; cross-modül FK yok |
| Location istisnası | INT id reference (slug çakışma, hierarchical 5-level, 885K+ TR seed) |
