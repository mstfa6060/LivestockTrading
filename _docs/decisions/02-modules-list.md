# Karar 2 — Modül Listesi + Wave Planı + Communication Matrix + AR Listesi

**Status:** FINAL (revize 7 → 10 modül)
**Karar tarihi:** Planning sessions #2, #6 (revizyon Karar 2 revize sırasında)

## İlişkili Kararlar

- **Üst:** [Karar 1 — Solution yapısı](01-architecture.md)
- **Alt:** [Karar 3 — Domain patterns](03-domain-patterns.md), [Karar 4a — Migration sırası](04-migration.md#4a-migration-sırası), [Karar 5 modül detayları](05-modules/)
- **Patch:** [05-patch.md](05-patch.md) — Patch 5 (Communication Matrix güncellemesi: Notifications producer 3 delivery feedback event, Catalog 4 → 7 event), Patch 6 (AR sayım 28 → 30)

---

## Bölüm 1: 10 Modül Listesi (Revize)

İlk planlamada 7 modül vardı; Karar 2 revize'de **3 modül ayrıldı**:
- **Carrier** Accounts'tan ayrıldı (Public dizin + onboarding + fleet + shipments + offers; 18 endpoint, kendi UI scope)
- **Subscription** Accounts'tan ayrıldı (Plans + commission table + invoices + boost; 16 endpoint, finansal domain)
- **Admin** yeni modül (cross-module aggregator; 22 endpoint, dashboard + moderation + impersonation + audit)

### Final Modül Listesi

| # | Modül | Schema | Tip | Endpoint |
|---|---|---|---|---|
| 1 | Catalog | `catalog` | Reference + admin-managed (Brand, Location 5-level, BorderRule eklendi v2'de) | 33 |
| 2 | Identity | `identity` | Auth + identity + profile + devices + roles + KVKK + GDPR data export | 40 |
| 3 | Accounts | `accounts` | Seller + Farm + VetProfile + Review + verifications + IBAN + ÇKS | 73 |
| 4 | Listings | `listings` | Listing + SavedSearch + faceted search + AI tagging/translation | 31 |
| 5 | **Carrier** | `carrier` | YENİ — Carrier persona + Fleet + ServiceArea + Shipment + CarrierOffer | 33 |
| 6 | Marketplace | `marketplace` | Offer + Deal (renamed from Agreement) + Favorite + Dispute + escrow | 28 |
| 7 | Messaging | `messaging` | Conversation + Message + SignalR + typing + read receipt + block | 25 |
| 8 | Notifications | `notifications` | NotificationPreference + InAppNotification + multi-channel + DND + Digest | 16 |
| 9 | **Subscription** | `subscription` | YENİ — Plan + Subscription + Invoice + BoostPackage + BoostCampaign | 32 |
| 10 | **Admin** | `admin` | YENİ — Dashboard + FeatureFlag + ScheduledReport + SystemVersion + ImpersonationSession + AdminAuditLog | 27 (own) + 74 (cross-module) |

**Toplam: ~417 endpoint** (10 modül + admin cross-module).

---

## Bölüm 2: Bounded Context Sınırları (Karar 2 orijinal tartışmaları)

### Marketplace — Tek context mi yoksa parçalara mı?

**Karar:** Listings + Marketplace + Messaging ayrı modüller (önerim onaylandı).

Tek "Marketplace" şişer:
- Listings → İlan CRUD, fotoğraf, moderasyon, filtre, arama
- Marketplace → Offer, Agreement (Deal), Favorite — "ilan üzerinde yapılan eylemler"
- Messaging → Conversation + Message, SignalR — gerçek zamanlı infrastrüktür farklı

### Identity vs UserProfile?

**Karar:** Identity modülü içinde iki feature grubu:
- `Identity/Features/Auth/` → register, login, refresh, social, passkey
- `Identity/Features/Profile/` → me, preferences, locale, avatar

Profile her zaman User'a dayanıyor, kendi aggregate'i yok; modül sınırı çizmek için yeterli sebep yok.

### Sellers ve Carriers nereye?

**İlk öneri (7 modül):** Accounts modülü altında Sellers + Carriers.
**Revize (10 modül):** Carrier ayrı modül — Public dizin, onboarding, fleet, shipments, offers, rates matrix (8 region × livestock type), zones. 18 endpoint, kendi UI scope.

### Reference Data nereye?

**Karar:** Catalog ayrı modül.
- Country, Currency, Language → seed-based, INT PK, nadiren değişir
- Category (tree), Breed, CertificationType → admin tarafından yönetilir
- v2'de eklendi: Brand, Location (5-level), BorderRule, CategoryAttribute

Shared Kernel'e değil — admin tarafından yönetilen feature seti.

### Notifications ayrı modül mü?

**Karar:** Evet, ayrı modül — push + email + SMS + in-app fan-out cross-cutting concern.

### Geo ayrı modül mü?

**Karar:** Modül değil, Shared servis. `Shared/Services/GeoService` + `Shared/Services/CurrencyService`.

---

## Bölüm 3: Wave Planı (DAG)

Karar 4a'da [Wave order](04-migration.md#4a-migration-sırası) detaylı. Burada özet.

### Logical Dependency Map

```
                    Catalog
                       │
                       ▼
                    Identity
              ┌────────┼────────┬─────────────┐
              ▼        ▼        ▼             ▼
          Accounts  Listings  Carrier    Subscription
              │        │        │             │
              ▼        ▼        │             │
              └────────┼────────┘             │
                       │                      │
              ┌────────┼──────────────────────┘
              ▼        ▼          ▼
        Marketplace  Messaging  Notifications
              │        │          │
              └────────┼──────────┘
                       ▼
                     Admin
                  (cross-module
                  aggregator)
```

### Wave Plan

```
Wave 0: Infrastructure (init.sql)
   └─ extensions + schemas + 2 role (livestock_app, livestock_migrator)
                       ↓
Wave 1: Catalog
   └─ Country/Currency/Language/Category/Breed/CertificationType/Brand/Location/BorderRule
                       ↓
Wave 2: Identity
   └─ User, RefreshToken, UserDevice, UserExternalLogin, UserRole, UserConsent, PhoneVerificationTicket
                       ↓
Wave 3 + Wave 4 (paralel — prod; sequential — dev fixtures)
   ├─ Wave 3: Accounts (Seller, Farm, VetProfile, Review)
   └─ Wave 4: Listings (Listing, SavedSearch)
                       ↓
Wave 5 (paralel — prod ve dev)
   ├─ Carrier (Carrier, Shipment, CarrierOffer + ServiceArea/Zone/Rate/Vehicle/Driver entities)
   └─ Subscription (Plan, Subscription, Invoice, BoostPackage, BoostCampaign)
                       ↓
Wave 6 (paralel)
   ├─ Marketplace (Offer, Deal, Favorite, Dispute)
   ├─ Messaging (Conversation, Message, MessageReport, MessageReadReceipt + UserBlock junction)
   └─ Notifications (NotificationPreference, InAppNotification + NotificationTemplate entity + DeliveryAttempt log)
                       ↓
Wave 7: Admin
   └─ FeatureFlag, ScheduledReport, SystemVersion, ImpersonationSession + AdminAuditLog log entity
```

### Parallel-Safe Analiz

| Wave | Parallel? | Sebep |
|---|---|---|
| Wave 3 + 4 (Accounts + Listings) | Prod: ✓ / Dev: ✗ | Farklı schema, no cross-modül FK; dev fixtures sample Listings için Seller bekliyor |
| Wave 5 (Carrier + Subscription) | ✓ Prod + Dev | Bağımsız modüller, no FK |
| Wave 6 (Marketplace + Messaging + Notifications) | ✓ Prod + Dev | 3 ayrı schema, no FK |
| Wave 7 (Admin) | Tek modül | Son wave, cross-module aggregator gerekçesiyle |

### Admin Niye Wave 7?

1. **Cross-Module Read Bağımlılığı** — Admin'in primary fonksiyonu diğer modüllerin verilerini agregate etmek (dashboard, moderation queues, KPI)
2. **Cross-Module Write Pattern** — Admin'in write işlemleri ya event publish ya `IAdminCommandService<Module>` interface — diğer modüllerin command surface'i hazır olmalı
3. **Event Consumer Registration** — Admin tüm modüllerin Public event'lerini dinleyip read model projection yapıyor; event tipleri `Shared/Events/` altında **var olmalı**
4. **Schema Migration Bağımsızlığı (Pratik)** — Marginal performans kaybı, anlamlı operasyonel kazanç (deployment order = wave order, predictability)

---

## Bölüm 4: Görev 3 — Communication Matrix (80 Public Event)

### Public vs Internal Kriteri

**Public** (`Shared/Events/`) olma şartı: en az bir **başka modül** bu event'e reaksiyon verecek.

| Senaryo | Public? |
|---|---|
| Cross-modül side effect (cascade pause, status flip) | Public |
| Notifications fan-out (her zaman cross-modül) | Public |
| Cross-modül read model / cache invalidate | Public |
| Aynı modül içi UI realtime push (SignalR) | Internal |
| Audit log / module-internal state machine | Internal |

**Teknik dispatch:**
- Internal events → MassTransit.Mediator in-process, aynı transaction
- Public events → transactional outbox → RabbitMQ → diğer modüllerin consumer'ları

### Public Event Final Sayım (Patch 5'ten)

```
Identity:       6  (UserRegistered, UserEmailVerified, UserPasswordChanged, 
                    UserSuspended, UserReactivated, UserDeleted)
Accounts:       11 (Seller × 7 + Vet × 3 + Review × 1)
Carrier:        8  (Onboarding/Verified/Suspended/Reactivated + Shipment × 4)
Catalog:        7  (CategoryDeactivated/Reactivated + Breed × 2 + Brand × 3) 
                   [+ BorderRule × 3 Faz 2 activate olunca = 10]
Listings:       11 (Submitted, Approved, Rejected, Paused, Resumed, Sold, Expired, 
                    Deleted, PriceChanged, ReportFiled, SavedSearchMatchFound)
Marketplace:    13 (Offer × 6 + Deal × 7)
Messaging:      4  (ConversationStarted, MessageSent, MessageRead, MessageReportFiled)
Subscription:   14 (Subscription × 6 + Invoice × 4 + Boost × 3 + Commission × 1)
Notifications:  3  (EmailBounced, SmsDeliveryFailed, PushTokenInvalidated 
                    — producer = Notifications, consumer = Identity — Patch 5 düzeltmesi)
Admin:          0  (terminal aggregator)

TOPLAM:         80 Public event (Faz 1 active)
```

Detay: [03-domain-patterns.md / Bölüm 2 (Domain Events)](03-domain-patterns.md#bölüm-2-domain-event-listesi).

### Hot Path Tablosu

| Event | Consumer Sayısı | Frekans | Kritiklik |
|---|---|---|---|
| `UserDeleted` | 9 | Düşük | 🔥🔥 MEGA GDPR cascade |
| `DealCompleted` (eski AgreementCompleted) | 6 | Orta | 🔥 chain + commission |
| `SellerSuspended` | 5 | Düşük | 🔥 |
| `CarrierSuspended` | 4 | Düşük | 🔥 |
| `InvoiceFailed` | 4 | Düşük | 🔥 revenue kritik |
| `SubscriptionExpired` | 4 | Düşük | 🔥 |
| `ListingDeleted` | 3 | Orta | 🔥 |
| `OfferAccepted` | 3 | Yüksek | 🔥 hot freq |
| `MessageSent` | 1 cross-modül | Çok Yüksek | Hot frequency |

### Cascade Chain Örnekleri

#### Chain 1: `UserDeleted` (GDPR — 9 consumer)

```
UserDeleted (Identity)
   ├─ Accounts: Seller + Farm + VetProfile + Review anonimize
   ├─ Carrier: Carrier + Fleet + Driver anonimize
   ├─ Listings: ilanları anonimleştir
   ├─ Marketplace: Offer/Deal/Dispute participant anonimize
   ├─ Messaging: Conversation participant anonimize, UserBlock cleanup
   ├─ Subscription: aktif subscription cancel + invoice void + boost cancel
   ├─ Notifications: prefs sil, in-app temizle, queue'dan çıkar
   ├─ Admin: AdminAuditLog kayıt
   └─ (Identity kendi içinde): RefreshTokens + Devices + ExternalLogins + Consents delete
```

#### Chain 2: `DealCompleted` (Marketplace → 6 consumer)

```
DealCompleted (Marketplace)
   ├─ Listings: MarkAsSold → ListingSold chain
   ├─ Subscription: ChargeCommissionAsync sync → CommissionCharged event
   ├─ Carrier: ShipmentFinalized (varsa)
   ├─ Messaging: system message post
   ├─ Notifications: review prompt fan-out
   └─ Admin: KPI dashboard GMV counter
```

#### Chain 3: `SubscriptionExpired`

```
Subscription detects expiry → SubscriptionExpired published
   ├─ Listings: Seller'ın aktif ilanlarındaki boost flag temizle, free tier limit
   ├─ Notifications: email + in-app + push
   └─ Admin: subscriptions counter + churn timeseries
```

#### Chain 4: `CarrierSuspended` (hybrid sync + async — Görev 3 Backlog #63 kararı)

```
Admin → IAdminCarrierCommands.SuspendAsync(carrierId, reason, adminId)
   ↓ (synchronous)
Carrier module:
   1. carrier.Suspend()
   2. IMarketplaceShipmentCommands.FlagCarrierShipmentsAsync(carrierId)  ← SYNC cross-modül
        ↓ Marketplace
        ↓ active shipments → status FlaggedForReassignment
   3. SaveChanges + outbox → CarrierSuspended event
       ↓ HTTP 200 (admin sees Suspended immediately)
       
Async cascade:
   ├─ Identity: role revoke
   ├─ Subscription: pause (Faz 1 muhtemelen no-op)
   └─ Notifications: bildirim fan-out
```

#### Chain 5: `InvoiceFailed` (7-day grace)

```
Stripe webhook → invoice.MarkFailed(reason)
   ↓ Subscription.EnterGracePeriod(daysGrace = 7)
   
Async cascade:
   ├─ Notifications: email + in-app urgent + SMS
   ├─ Identity (opsiyonel): User'a "Account at risk" flag
   └─ Admin: escalation_queue insert + KPI failed_payments
   
7 gün sonu cron job:
   → Subscription.Expire() → SubscriptionExpired Public event
   → Listings: boost flag temizle, free tier limit
```

### Cross-Modül Pattern Matrix

| Çağrı | İzin | Pattern |
|---|---|---|
| Modül → Modül (peer) | **YASAK** | Event-only veya kasıtlı istisna |
| Admin → Modül (write) | İZİNLİ (sync) | `IAdminXCommands` |
| Admin → Modül (read) | İZİNLİ (sync, cross-schema Dapper OK) | `IAdminXReadService` |
| Modül → Identity (read) | İZİNLİ (sync) | `ICurrentUserService` (JWT) + `IIdentityReadService` |
| Modül → Catalog (read) | İZİNLİ (sync, cached) | `ICatalogReadService` |
| Modül → Modül (event) | İZİNLİ (async) | RabbitMQ public event |
| Modül → Modül (sync write) | **İSTİSNAYLA** | Sadece kritik (Marketplace → Carrier.CreateShipment, Marketplace → Listings.Reserve, etc.); justification gerekli |

### Karar 7 (Observability) İçin Çıkan Gereksinimler

1. **CorrelationId propagation** — Public event base contract'ında zorunlu
2. **OpenTelemetry event correlation** — RabbitMQ message header'ında trace context
3. **Hot path metrics** — UserDeleted, DealCompleted, OfferAccepted, MessageSent, SellerSuspended, CarrierSuspended için Prometheus
4. **Outbox sağlık metric'leri** — pending count, publish latency p95, DLQ count
5. **Idempotency tablosu** — `processed_event_ids` her consumer modülde
6. **Chain timeout alert'leri** — `AgreementCompleted` (DealCompleted) → `ListingSold` 30sn içinde yoksa alert
7. **Per-listing fan-out throttle** — `SellerSuspended` gibi cascade'lerde Notifications batching

---

## Bölüm 5: Görev 4 — Aggregate Root Listesi (30 AR + 5 Reference)

### Identity (1 AR)

| AR | Faz | Notlar |
|---|---|---|
| **User** | Faz 1 | Tek AR; içinde RefreshToken, UserDevice, UserExternalLogin, UserRole, UserConsent (YENİ), PhoneVerificationTicket (YENİ) entity'leri |

### Accounts (4 AR — Görev 4 + Identity Q3 + Listings D6)

| AR | Faz | Notlar |
|---|---|---|
| **Seller** | Faz 1 | UserId ID-ref Identity; SellerDocument, SellerVerification, BusinessInfo VO, IBAN VO, CksNumber, ChamberOfCommerce |
| **Farm** | Faz 1 | SellerId ID-ref; FarmLocation VO (PostGIS), HealthRecord entity, VaccineRecord entity, FarmPurpose [Flags] |
| **VetProfile** | Faz 1 (Identity v2 Q3 kararı) | UserId ID-ref; admin manuel verification gerekir; LicenseNumber (TVHB), Specialization, ClinicAddress |
| **Review** | Faz 1 (Accounts v2 D6) | DealId ID-ref, ReviewerUserId + ReviewedUserId, Direction (BuyerToSeller / SellerToBuyer), ReviewReply VO |

### Carrier (3 AR)

| AR | Faz | Notlar |
|---|---|---|
| **Carrier** | Faz 1 | UserId ID-ref Identity; içinde CarrierServiceArea (PostGIS MultiPolygon), CarrierZone, CarrierRate, Vehicle, Driver, CarrierDocument |
| **Shipment** | Faz 1 | DealId ID-ref Marketplace, CarrierId ID-ref Carrier; lifecycle: Pending → Assigned → PickedUp → InTransit → Delivered → Cancelled |
| **CarrierOffer** | Faz 1 | ShipmentRequestId + CarrierId; lifecycle: Pending → Accepted/Rejected/Withdrawn/Expired |

### Catalog (4 AR + 5 Reference Entity)

| Entity | Tip | Faz |
|---|---|---|
| **Category** | AR | Faz 1 — Tree (2 level); + `CategoryAttribute` child entity (v2) |
| **Breed** | AR | Faz 1 — CategoryId ref |
| **Brand** | AR (v2 yeni) | Faz 1 — admin onay + seller suggestion + bulk import; `BrandCategory` child junction |
| **BorderRule** | AR (v2 yeni) | Faz 1 schema-ready, Faz 2 feature pasif |
| `Country` | Reference (INT PK) | 250 ISO seed |
| `Currency` | Reference (INT PK) | 180 ISO seed |
| `Language` | Reference (INT PK) | 50 seed |
| `CertificationType` | Reference (INT PK) | 12 seed + admin extend |
| `Location` | Reference (INT PK, v2 yeni) | 5-level hierarchy, TR ~885K Faz 1 |

### Listings (2 AR)

| AR | Faz | Notlar |
|---|---|---|
| **Listing** | Faz 1 | SellerId/FarmId/CategoryId/BreedId/CountryCode/LocationId references; child: Image, Document (verifiedBy: system/ministry/vet), Translation, Attribute, Certification, Report |
| **SavedSearch** | Faz 1 | UserId; SavedSearchFilter VO (specification pattern); RadiusCenter PostGIS Point |

### Marketplace (4 AR — Deal rename: Agreement → Deal)

| AR | Faz | Notlar |
|---|---|---|
| **Offer** | Faz 1 | Counter offer chain (parent_offer_id, max depth 5); BuyerToSeller / SellerToBuyer direction |
| **Deal** | Faz 1 | (Eski Agreement, RENAME); 8-state FSM; EscrowStatus Faz 1 in-DB tracking |
| **Favorite** | Faz 1 | UserId + ListingId, idempotent toggle |
| **Dispute** | Faz 1 | DealId ID-ref; Evidence append-only entity; Resolution VO (RefundFull/Partial/CompleteAsIs) |

### Messaging (1 AR)

| AR | Faz | Notlar |
|---|---|---|
| **Conversation** | Faz 1 | İki UserId + opsiyonel ListingId/DealId/OfferId context; Message + ConversationParticipant + MessageReadReceipt + MessageReport entity'leri |

`UserBlock` junction entity (AR değil — Karar 3d kuralı).

### Notifications (2 AR)

| AR | Faz | Notlar |
|---|---|---|
| **NotificationPreference** | Faz 1 | UserId; DND + ChannelMask + DigestFrequency JSONB |
| **InAppNotification** | Faz 1 | UserId; lifecycle Unread → Read → Archived; 90-day expiry |

`NotificationTemplate` (entity — admin-managed config), `DeliveryAttempt` (append-only log), `DigestQueueItem` (queue) — AR değil.

### Subscription (5 AR)

| AR | Faz | Notlar |
|---|---|---|
| **Plan** | Faz 1 | Code + Tier + PlanFeatures VO; child: PlanPricing, CommissionRule |
| **Subscription** | Faz 1 | SubscriberUserId (Görev 1/F future-positive naming); seller-only guard application-layer; Trial 14-day default Pro/Standard |
| **Invoice** | Faz 1 | InvoiceLineItem child; 4 kind (SubscriptionRenewal/Upgrade/Boost/Commission); InvoiceStatus FSM |
| **BoostPackage** | Faz 1 | Admin catalog (top-row 7gün, showcase 30gün, urgent-label 3gün); BoostPackagePricing child |
| **BoostCampaign** | Faz 1 | SubscriberUserId + AppliedListingId (TEKİL — Çekince 2 kararı); InvoiceId; ValidUntil |

Universal PaymentMethod entity (Subscription içinde — buyer ve seller her ikisi için universal Stripe customer).

### Admin (4 AR)

| AR | Faz | Notlar |
|---|---|---|
| **FeatureFlag** | Faz 1 | RolloutPercentage + targeting (users/countries/roles) + stable hash bucket |
| **ScheduledReport** | Faz 1 | Cron + ReportType enum + DeliveryChannel (Email/MinioBucket/AdminDashboard); ReportExecution child |
| **SystemVersion** | Faz 1 | Platform × Version + MinSupportedVersion + ForceUpdate; 1 active per platform |
| **ImpersonationSession** | Faz 1 | AdminUserId + TargetUserId + Justification + 4h max; jti blacklist on end |

`AdminAuditLog` entity (logging concern, AR değil — append-only with Faz 2 DB-level REVOKE UPDATE/DELETE).

### Toplam Sayım

| Modül | AR | Reference Entity |
|---|---|---|
| Identity | 1 | — |
| Accounts | 4 | — |
| Carrier | 3 | — |
| Catalog | 4 | 5 |
| Listings | 2 | — |
| Marketplace | 4 | — |
| Messaging | 1 | — |
| Notifications | 2 | — |
| Subscription | 5 | — |
| Admin | 4 | — |
| **TOPLAM** | **30** | **5** |

**Toplam entity family: 35.**

---

## Bölüm 6: Cross-Module Interface Inventory (Karar 5 sonu Patch 7)

### Read Services (Shared/Contracts/{Module}/)

| Interface | Modül |
|---|---|
| `ICurrentUserService` | Identity (request-scoped, JWT'den) |
| `IIdentityReadService` | Identity (cached lookup) |
| `IAccountsReadService` | Accounts |
| `ICarrierReadService` | Carrier |
| `ICatalogReadService` | Catalog |
| `IListingsReadService` | Listings |
| `IMarketplaceReadService` | Marketplace |
| `IMessagingReadService` | Messaging |
| `INotificationsReadService` | Notifications |
| `ISubscriptionReadService` | Subscription |

### Commands (Sync Cross-Modül Write)

| Interface | Modül | Çağıran |
|---|---|---|
| `IListingsCommands` | Listings | Marketplace (Reserve/Unreserve/MarkSold) |
| `ICarrierShipmentCommands` | Carrier | Marketplace (CreateShipment, CancelByDeal, FlagCarrierShipments) |
| `ISubscriptionCommands` | Subscription | Listings (quota), Messaging (quota), Marketplace (Charge/Refund/ChargeCommission) |

### Admin (Sync Cross-Modül)

| Interface | Modül |
|---|---|
| `IAdminUserCommands` + `IAdminUserReadService` | Identity |
| `IAdminSellerCommands` + `IAdminSellerReadService` | Accounts |
| `IAdminVetCommands` + `IAdminVetReadService` | Accounts |
| `IAdminCarrierCommands` + `IAdminCarrierReadService` | Carrier |
| `IAdminCatalogCommands` + `IAdminCatalogReadService` | Catalog |
| `IAdminListingCommands` + `IAdminListingReadService` | Listings |
| `IAdminMarketplaceCommands` + `IAdminMarketplaceReadService` | Marketplace |
| `IAdminMessagingCommands` + `IAdminMessagingReadService` | Messaging |
| `IAdminNotificationsCommands` | Notifications |
| `IAdminSubscriptionCommands` + `IAdminSubscriptionReadService` | Subscription |

### Cross-Cutting Services

| Interface | Modül |
|---|---|
| `IFileStorage` | Shared/Contracts/Storage (MinIO impl) |
| `IFeatureFlagService` | Admin (cross-cutting consumer) |
| `IDataExportContributor` | Per modül implement, Identity orchestrator |
| `IAdminAuditService` | Admin (modüller IAdminXCommands'tan çağırır) |
| `IPaymentProvider` | Subscription (internal) |
| `IEmailProvider`, `ISmsProvider`, `IPushProvider` | Notifications (internal) |
| `IGeoIpService` | Identity (sessions enrichment) |

---

## Özet Tablo

| Konu | Karar |
|---|---|
| Modül sayısı | 10 (Karar 2 revize sonrası) |
| Wave sayısı | 7 (Wave 0 infra + Wave 1-7 modül grupları) |
| Public event Faz 1 | 80 |
| Aggregate Root | 30 |
| Reference Entity | 5 |
| Cross-modül write interface | 3 (`IListingsCommands`, `ICarrierShipmentCommands`, `ISubscriptionCommands`) |
| Admin command/read interface | 10 modül × 2 = 20 |
| Cascade chain hot path | 5 (UserDeleted 9 consumer, DealCompleted 6, SellerSuspended 5, CarrierSuspended 4, InvoiceFailed 4) |
