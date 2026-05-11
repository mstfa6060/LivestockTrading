# Karar 5 — Modül Detayları (10 Modül)

**Status:** FINAL — 10/10 modül üretildi
**Üst:** [../README.md](../README.md), [../02-modules-list.md](../02-modules-list.md)

---

## Modül Index — Wave Order

| Wave | Modül | Doc | AR | Public Event | Endpoint |
|---|---|---|---|---|---|
| 1 | Catalog (v2) | [05-catalog.md](05-catalog.md) | 4 + 5 ref | 7 (+3 Faz 2) | 33 |
| 2 | Identity (v2) | [05-identity.md](05-identity.md) | 1 | 6 + 3 consume | 40 |
| 3 | Accounts (v2) | [05-accounts.md](05-accounts.md) | 4 | 11 | 73 |
| 4 | Listings | [05-listings.md](05-listings.md) | 2 | 11 | 31 |
| 5 (paralel) | Carrier | [05-carrier.md](05-carrier.md) | 3 | 8 | 33 |
| 5 (paralel) | Subscription | [05-subscription.md](05-subscription.md) | 5 | 14 | 32 |
| 6 (paralel) | Marketplace | [05-marketplace.md](05-marketplace.md) | 4 | 13 | 28 |
| 6 (paralel) | Messaging | [05-messaging.md](05-messaging.md) | 1 | 4 | 25 |
| 6 (paralel) | Notifications | [05-notifications.md](05-notifications.md) | 2 | 3 | 16 |
| 7 | Admin | [05-admin.md](05-admin.md) | 4 | 0 | 27 + 74 cross-module |
| **TOPLAM** | | | **30 + 5 ref** | **80** | **~417** |

---

## Doc Convention (Her Modül)

1. **İlişkili Kararlar** — üst karar, patch, frontend ref
2. **Modülün Rolü ve Sınırları** — Sahip / Sahip değil
3. **Aggregate Roots** — C# class definition + lifecycle methods
4. **Child Entities** — junction, log entities
5. **Cross-Modül Erişim** — `IXReadService`, `IXCommands`, `IAdminXCommands`
6. **Public Event'ler** — payload + consumer mapping
7. **API Endpoint Inventory** — Public / Authenticated / Admin / Webhook / WS
8. **Özel Konular** — modüle özgü detaylar
9. **Discovered Backlog** — modül-spesifik backlog item'lar
10. **Özet Tablo**

---

## v1 → v2 Revizyon Sayım

| Modül | v1 | v2 Değişim | Sebep |
|---|---|---|---|
| **Catalog** | v1 | **v2** | Brand AR + Location 5-level + BorderRule + CategoryAttribute + 3-tier TCMB rate provider |
| **Identity** | v1 | **v2** | 3-method login + KVKK consents + Phone OTP + GDPR data export + /oauth/ rename + e-Devlet/TARSİM İPTAL + PendingEmail flow |
| **Accounts** | v1 | **v2** | Slug routing + 5-step verification + BuyerStats + Follow + Review AR + Cover photo + Certifications + Pre-signed URL + FarmPurpose Flags + Immutable HealthRecord/VaccineRecord + VetProfile AR |

Diğer 7 modül v1 final (revize yok).

---

## Cross-Modül Pattern Özeti

### Read Services (Shared/Contracts/{Module}/)

10 interface — modül başına bir tane (ICurrentUserService Identity'de scoped).

### Sync Commands (Cross-Modül Write)

3 interface:
- `IListingsCommands` — Marketplace tetikliyor (Reserve/Unreserve/MarkSold)
- `ICarrierShipmentCommands` — Marketplace tetikliyor (CreateShipment, CancelByDeal, FlagCarrierShipments)
- `ISubscriptionCommands` — Listings/Messaging tetikliyor (quota), Marketplace tetikliyor (Charge/Refund/ChargeCommission)

### Admin Sync (Görev 1/G)

10 modül × 2 interface = 20:
- `IAdminUserCommands` + `IAdminUserReadService`
- `IAdminSellerCommands` + `IAdminSellerReadService`
- `IAdminVetCommands` + `IAdminVetReadService`
- `IAdminCarrierCommands` + `IAdminCarrierReadService`
- `IAdminCatalogCommands` + `IAdminCatalogReadService`
- `IAdminListingCommands` + `IAdminListingReadService`
- `IAdminMarketplaceCommands` + `IAdminMarketplaceReadService`
- `IAdminMessagingCommands` + `IAdminMessagingReadService`
- `IAdminNotificationsCommands` + (NotificationsReadService Shared'da)
- `IAdminSubscriptionCommands` + `IAdminSubscriptionReadService`

### Cross-Cutting

- `IFileStorage` (Shared/Contracts/Storage)
- `IFeatureFlagService` (Admin)
- `IDataExportContributor` (Identity orchestrator + per-modül contributor)
- `IAdminAuditService` (Admin)
- `IPaymentProvider` (Subscription internal)
- `IEmailProvider`, `ISmsProvider`, `IPushProvider` (Notifications internal)
- `IGeoIpService` (Identity)

---

## Hot Path Cascades (Görev 3 Communication Matrix)

| Cascade | Consumer | Sebep |
|---|---|---|
| `UserDeleted` | 9 | GDPR — Identity → Accounts/Carrier/Listings/Marketplace/Messaging/Subscription/Notifications/Admin/internal |
| `DealCompleted` | 6 | Listings (MarkSold) + Subscription (Commission) + Carrier (Shipment) + Messaging (System msg) + Notifications + Admin |
| `SellerSuspended` | 5 | Listings (auto-pause) + Marketplace (cancel offers) + Subscription (pause) + Notifications + Admin |
| `CarrierSuspended` | 4 | Marketplace sync (FlagCarrierShipments) + Identity (role revoke) + Subscription + Notifications |
| `InvoiceFailed` | 4 | Notifications + Identity (account-at-risk) + Subscription (grace) + Admin (escalation) |
| `OfferAccepted` | 3 + Deal co-creation | Listings (Reserve) + Messaging + Notifications |
| `ListingDeleted` | 3 | Marketplace (cancel offers) + Messaging (context) + Notifications |
| `MessageSent` | 1 cross-modül | Notifications (push + email + counter) — hot frequency |

---

## VO Inventory (24+)

Detay: [../03-domain-patterns.md / Bölüm 5](../03-domain-patterns.md#bölüm-5-value-object-inventory-3e--24-vo).

- **Shared/ValueObjects/** (8): CountryCode, LanguageCode, CurrencyCode, Money, EmailAddress, PhoneNumber, PersonName, Address
- **Shared/ValueObjects/Business/** (4 — Carrier Çekince 1): BusinessInfo, BankInfo, Iban, TaxNumber
- **Shared/ValueObjects/** (1): NationalId
- **Modül-içi** (~20)

---

## Faz 2 Schema-Ready Liste

Karar 5 boyunca **schema-ready feature-pasif** alanlar:

| Konu | Modül | Faz 2 Aktivasyon |
|---|---|---|
| User.TotpSecretEncrypted + TwoFactorEnabled | Identity | 2FA endpoints |
| Passkey/WebAuthn | Identity | OpenIddict extension |
| Apple email relay deactivation webhook | Identity | Real handler |
| NationalId NVI sync | Identity | NVI API |
| BorderRule.* enforcement | Catalog | Gümrük entegrasyonu |
| Carrier.Vehicle.HasGps + Driver.UserId | Carrier | GPS streaming + driver mobile login |
| Listing.AiQualityScore/AiModerationScore/AiTagsJson | Listings | ML pipeline |
| Listing.Translations AI auto-fill (50 dil) | Listings | AI worker |
| Deal.EscrowAmount real money | Marketplace | Stripe escrow |
| Dispute.Resolution.RefundPartial | Marketplace | Partial refund logic |
| Plan.CommissionRule per-category | Subscription | Per-category override |
| BoostCampaign cancellation prorate | Subscription | Prorate refund |
| NotificationPreference ML priority | Notifications | ML smart digest |
| NotificationTemplate Razor/Liquid | Notifications | Conditional logic |
| WhatsApp DeliveryChannel | Notifications | Channel.WhatsApp + template approval |
| ScheduledReport.CustomDapperQuery | Admin | Sandboxed SQL |
| ImpersonationSession 2FA enforcement | Admin | 2FA confirm |
| AdminAuditLog DB-level append-only | Admin | REVOKE UPDATE/DELETE |
| Listings strict CategoryAttribute validation | Listings | Catalog schema strict |
| Catalog MinistrySync | Accounts/Catalog | Bakanlık HBS |
