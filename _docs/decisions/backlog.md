# Backlog — 226 Item (Faz 2 Hardening + Features + DX)

**Status:** FINAL — Karar 1-7 boyunca biriken tüm açık konular
**Üst:** [README.md](README.md)
**Kullanım:** Implementasyon sırasında Wave 0-7 kapsamı dışında kalanlar buraya akar. Karar 6/7'ye taşınanlar ilgili doc'larda kapanmış sayılır.

---

## Toplam Dağılım

| Kategori | Sayı | Status |
|---|---|---|
| Karar 5 boyunca KAPANAN | 33 | Closed (modül doc'ta detay) |
| Karar 6'ya alınıp orada KAPANAN | 13 | Closed (06-api-contract.md) |
| Karar 7'ye alınıp orada KAPANAN | 35 | Closed (07-operations.md) |
| Karar 7 / Operations — Faz 2 aktivasyon | 25 | Schema-ready, kod Faz 2 |
| Faz 2 Feature backlog (domain) | 60 | Faz 2 product roadmap |
| Frontend backlog (web + mobile) | 28 | Frontend repo'ya taşınacak |
| DX + Tooling | 14 | Sürekli iyileştirme |
| Anti-abuse + Security hardening | 18 | Faz 1.5 - Faz 2 |
| **TOPLAM** | **226** | |

---

## A. Karar 5 Boyunca Kapanan (33)

Modül doc'larında karar verilmiş — burada referans listesi.

| # | Konu | Modül | Çözüm |
|---|---|---|---|
| 1 | Listing cross-modül FK yasağı | Listings | ID-only reference (Karar 3d) |
| 2 | Approved listing edit policy | Listings | Patch 4.3 (price/title editable, location re-approval) |
| 3 | Offer.Accept() → Deal AR co-creation | Marketplace | Backlog #8 pattern (same TX, same module) |
| 5 | Realtime push catalog (kısmi) | Cross-modül | Karar 6 Madde 6 (tam matris) |
| 6 | Plan trial expiry semantics | Subscription | TrialEnded event + auto-downgrade |
| 7 | OAuth claim trust seviyesi (kısmi) | Identity | EmailVerified=true ancak verified IDP'den |
| 8 | AR co-creation kavramı | Domain pattern | Karar 3 / Bölüm 6 |
| 9 | UserPreferences VO genişletmesi | Identity | Patch 1.1 (read receipts + typing) |
| 10 | PaymentMethod universal sahiplik | Subscription | Patch 2.4 (Marketplace çağırır) |
| 12 | Brand AR + suggest workflow | Catalog v2 | 05-catalog.md / Brand bölümü |
| 13 | Location reference entity 5-level | Catalog v2 | INT PK, ~885K TR seed |
| 18 | TCMB rate provider failover | Catalog v2 | 3-tier (TCMB → ECB → exchangerate.host) |
| 19 | Seller verification D1-D8 | Accounts v2 | Seller AR state machine |
| 20 | Farm purpose Flags enum | Accounts v2 | Multi-purpose ([Flags]) |
| 22 | VetProfile AR | Accounts v2 | 4. AR Accounts'a eklendi |
| 36 | Slug routing 5-step verification | Accounts v2 | Slug uniqueness + rate limit |
| 49 | UserDeleted GDPR cascade — 9 consumer | Cross-modül | Patch 5.3 |
| 50 | DealCompleted cascade — 6 consumer | Cross-modül | Patch 5.3 |
| 51 | Agreement → Deal rename | Marketplace | Patch 5.5 (Faz 1 öncesi rename) |
| 53 | Catalog event sayım 4 → 7 | Catalog | Brand × 3 event eklendi |
| 54 | Public event final sayım 80 | Cross-modül | Patch 5.4 |
| 55 | AR sayım 28 → 30 | Görev 4 | Patch 6.1 (VetProfile + Review) |
| 65 | Phone OTP 3-method login | Identity v2 | email/phone/nationalId |
| 80 | PendingEmail cleanup cron | Identity v2 | Patch 1.2 (24h Quartz) |
| 81 | GDPR data export | Identity v2 | IDataExportContributor pattern |
| 82 | KVKK consent versioning | Identity v2 | UserConsent AR + version tracking |
| 83 | OAuth provider rename to /oauth/ | Identity v2 | Route convention |
| 90 | NationalId self-declared (NVI ileri) | Identity v2 | Faz 1 trust-on-write; Faz 2 NVI sync |
| 103 | Vet verification flow | Accounts v2 | VetProfile + cert upload |
| 112 | Review AR | Accounts v2 | DealCompleted sonrası |
| 113 | Cover photo + certifications | Accounts v2 | Seller AR genişlemesi |
| 121 | Pre-signed URL upload | Accounts v2 | MinIO + IFileStorage |
| 123 | TCMB 3-tier provider | Catalog v2 | Karar 5/Catalog v2 |
| 142 | UserBlock junction | Messaging | UserBlock entity (Mod) |

---

## B. Karar 6'ya Taşınıp Orada Kapanan (13)

API contract'ta detay var — burada özet.

| # | Konu | Karar 6 Madde |
|---|---|---|
| 4 | Frontend MSW vs real backend stratejisi | Madde 1 (Hybrid) |
| 5 (devam) | Realtime push catalog tam matris | Madde 6 (80 event × hub × group × client) |
| 16 | Cross-modül raporlama exception | Madde 9 (error code matrix) |
| 30 | GeoJSON serialization paketi | Madde 7 |
| 31 | Map UI component selection | Madde 7 (frontend Mapbox/Leaflet) |
| 33 | MultiPolygon GeoJSON 4-derinlik validation | Madde 7 |
| 47 | Cursor pagination Shared utility | Madde 3 |
| 48 | Locale resolution middleware | Madde 4 (6-step) |
| 52 | Admin endpoint route convention | Madde 5 (`/admin/...`) |
| 76 | Consent versioning workflow UX | Madde 8 + frontend ApiError |
| 116 | Listing slug strategy global vs per-seller | Madde 5 + Patch 4 (slug + per-seller) |
| 125 | Counter offer chain max depth | Madde 9 (`OFFER_COUNTER_DEPTH_EXCEEDED`) |
| 132 | `/marketplace/me/payment-methods` alias | Madde 5 + Patch 2.4 (Subscription'a alias) |

---

## C. Karar 7'ye Taşınıp Orada Kapanan (35)

Operations doc'unda detay var.

| # | Konu | Karar 7 Bölüm |
|---|---|---|
| 11 | Prometheus + Grafana setup | A.1 |
| 14 | OpenTelemetry exporter | A.1 + A.4 |
| 15 | RED + USE metric standardı | A.1 |
| 17 | Custom domain metric per modül | A.1 (200+ metric) |
| 21 | Alert rules YAML | A.1 |
| 24 | Grafana 10 dashboard | A.1 |
| 25 | SLI/SLO targets | A.1.7 + C.1 |
| 26 | Loki + Serilog log pipeline | A.2 |
| 27 | Sentry error tracking | A.3 |
| 28 | Tempo distributed tracing | A.4 |
| 29 | Health checks (live/ready/startup) | A.5 |
| 32 | Synthetic uptime (Blackbox) | A.1 + A.5 |
| 34 | CI/CD pipeline Jenkinsfile | B.1 |
| 35 | Docker compose dev profile | B.1 + DX |
| 37 | pgBackRest backup | B.2 |
| 38 | DR drill cadence | B.2 |
| 39 | Secret management Vault | B.3 |
| 40 | Vault Agent sidecar pattern | B.3 |
| 41 | JWT signing key rotation 90d | B.3 |
| 42 | External integration failover (TCMB/Stripe/Brevo) | B.4 |
| 43 | OWASP A01-A10 mapping | B.5 |
| 44 | Rate limiter (login/OTP/api-default) | B.5 |
| 45 | Cloudflare WAF + DDoS | B.5 |
| 46 | TLS 1.2+ + HSTS + OCSP | B.5 |
| 56 | k6 load test framework | C.1 |
| 57 | k6 spike test offer-flow | C.1 |
| 58 | SignalR fan-out load test | C.1 |
| 59 | Capacity planning quarterly | C.1 |
| 60 | PR template + CODEOWNERS | C.2 |
| 61 | Runbook 15 entry | C.2.2 |
| 62 | Postmortem template + blameless culture | C.2.3 |
| 63 | K3s readiness checklist | C.3.1 |
| 64 | PgBouncer trigger criteria | C.3.2 |
| 66 | Meilisearch evaluation | C.3.3 |
| 67 | setup-dev.sh + CLI helpers | C.4 |

---

## D. Karar 7 / Operations — Faz 2 Aktivasyon (25)

Schema-ready ama kod Faz 2.

| # | Konu | Trigger |
|---|---|---|
| 68 | 2FA endpoint'ler (TOTP) | Patch 9, User row schema-ready |
| 69 | Passkey/WebAuthn | OpenIddict extension |
| 70 | Apple email relay webhook | Volume threshold |
| 71 | NVI NationalId sync | Bakanlık API erişimi |
| 72 | Border rule enforcement | Gümrük entegrasyonu |
| 73 | Carrier GPS streaming | Vehicle.HasGps aktif |
| 74 | Driver mobile login | Driver.UserId ile bağ |
| 75 | AI quality/moderation score | ML pipeline |
| 77 | Listing translations AI auto-fill | AI worker |
| 78 | Real money escrow (Stripe) | Stripe Connect |
| 79 | Partial refund logic | Dispute resolution genişletme |
| 84 | Per-category commission | Plan.CommissionRule override |
| 85 | Boost prorate cancellation | Stripe credit memo |
| 86 | ML notification priority | Smart digest |
| 87 | Razor/Liquid template engine | NotificationTemplate genişletme |
| 88 | WhatsApp delivery channel | Provider onboarding |
| 89 | ScheduledReport sandboxed SQL | Admin custom report |
| 91 | Impersonation 2FA enforcement | Admin audit hardening |
| 92 | AdminAuditLog DB append-only | REVOKE UPDATE/DELETE |
| 93 | Catalog ministry sync (HBS) | Bakanlık entegrasyonu |
| 94 | Multi-region active-active | Logical replication |
| 95 | PgBouncer prod cutover | DB pool saturation |
| 96 | K3s migration | Compose saturation |
| 97 | Meilisearch live | Listings search saturation |
| 98 | ChaosMesh chaos engineering | Faz 2 stability |

---

## E. Faz 2 Feature Backlog — Domain (60)

Faz 2 product roadmap.

### Identity (8)

| # | Konu |
|---|---|
| 99 | TOTP setup + recovery codes UI |
| 100 | Hardware key (FIDO2) |
| 101 | Trusted device list management |
| 102 | Session map / active devices view |
| 104 | Email change OTP step (replace 24h cron flow) |
| 105 | Account merge (duplicate email + phone) |
| 106 | Risk-based auth (login anomaly score) |
| 107 | Password breach check (HIBP) |

### Accounts (10)

| # | Konu |
|---|---|
| 108 | Seller storefront customization |
| 109 | Featured seller badges |
| 110 | Multi-farm bulk actions |
| 111 | Health record OCR import |
| 114 | Vet appointment scheduling |
| 115 | Vet specialty taxonomy |
| 117 | Follow feed (followed sellers' new listings) |
| 118 | Review reply (seller responds) |
| 119 | Review media attachments |
| 120 | Seller analytics dashboard |

### Catalog (5)

| # | Konu |
|---|---|
| 122 | Per-region breed availability map |
| 124 | Brand verified badge + admin approval |
| 126 | Category attribute strict validation Faz 2 |
| 127 | Location polygon GIS (PostGIS) ileri sorgu |
| 128 | Seasonal category boost (festival/term) |

### Listings (8)

| # | Konu |
|---|---|
| 129 | Bulk listing CSV import |
| 130 | Listing template (recurring inventory) |
| 131 | Auto-renew listing on expiry |
| 133 | Listing analytics (views/saved/contacted) |
| 134 | Saved search alerts (email + push) |
| 135 | Listing comparison view |
| 136 | Cross-listing breed match recommendation |
| 137 | Listing video upload + thumbnail |

### Marketplace (10)

| # | Konu |
|---|---|
| 138 | Escrow real money flow (Stripe Connect) |
| 139 | Dispute arbitrator role + workflow |
| 140 | Buyer protection — refund policy templating |
| 143 | Group buy / consortium offers |
| 144 | Counter offer comparison view |
| 145 | Deal contract PDF generation |
| 146 | DealCompleted automatic invoice |
| 149 | Marketplace tax engine (VAT/KDV calculation) |
| 150 | Recurring offer (subscription buying) |
| 151 | Auction mode (bid until deadline) |

### Carrier (5)

| # | Konu |
|---|---|
| 152 | Real-time tracking map for buyer |
| 153 | Driver mobile app integration |
| 154 | Multi-stop route optimization |
| 155 | Carrier rate comparison engine |
| 156 | Insurance bundling at checkout |

### Subscription (5)

| # | Konu |
|---|---|
| 157 | Annual billing discount |
| 158 | Plan downgrade/upgrade prorate |
| 159 | Team plan (multi-user single seller) |
| 160 | Coupon / promo code engine |
| 161 | Referral program credits |

### Messaging (4)

| # | Konu |
|---|---|
| 162 | Voice message support |
| 163 | Translated message (AI auto-translate UI toggle) |
| 164 | Message search across conversations |
| 165 | Pinned messages |

### Notifications (5)

| # | Konu |
|---|---|
| 147 | Per-user timezone digest run |
| 148 | Notification rate limit (flood guard) |
| 166 | Quiet hours per user |
| 167 | Notification category subscribe/unsubscribe granular |
| 168 | Push notification A/B test framework |

---

## F. Frontend Backlog (28)

Frontend repo'ya taşınacak — burada referans.

| # | Konu | Repo |
|---|---|---|
| 169 | @hey-api/openapi-ts client generation pipeline | web |
| 170 | TanStack Query setup + per-module query keys | web |
| 171 | MSW handler factory | web |
| 172 | i18n locale resolution UI (6 fallback) | web |
| 173 | Locale chip header (manual override) | web |
| 174 | Currency display per user preference | web |
| 175 | Timezone-aware date rendering | web |
| 176 | Cursor pagination infinite scroll component | web |
| 177 | Map component (Mapbox/Leaflet decision) | web |
| 178 | GeoJSON polygon editor | web |
| 179 | Image upload with pre-signed URL | web |
| 180 | SignalR client wrapper (auto-reconnect + heartbeat) | web + mobile |
| 181 | Typing indicator debounce (300ms) | web + mobile |
| 182 | Read receipt opt-out toggle | web + mobile |
| 183 | Push token registration (web push + APNS/FCM) | mobile + web |
| 184 | KVKK consent banner + version tracking | web |
| 185 | GDPR data export request UI | web |
| 186 | Form field error mapping (useApiFormErrors) | web |
| 187 | Global error boundary discriminated union | web |
| 188 | Rate-limited toast with countdown | web |
| 189 | Admin impersonation UI + warning banner | web |
| 190 | Feature flag client provider | web + mobile |
| 191 | Offline-first mobile listing draft | mobile |
| 192 | Mobile camera capture for listing images | mobile |
| 193 | Mobile deep link (universal links) | mobile |
| 194 | iOS App Store + Google Play submission | mobile |
| 195 | A/B test framework (PostHog/GrowthBook) | web + mobile |
| 196 | Lighthouse perf budget enforcement | web |

---

## G. DX + Tooling (14)

| # | Konu |
|---|---|
| 197 | setup-dev.sh end-to-end |
| 198 | new-endpoint scaffolding script (PowerShell) |
| 199 | regen-openapi.sh per module |
| 200 | regen-frontend-client.sh integration |
| 201 | Mkdocs documentation site |
| 202 | ADR (Architecture Decision Record) template |
| 203 | CLAUDE.md rewrite for new stack (Wave 1) |
| 204 | EditorConfig + dotnet format pre-commit hook |
| 205 | Conventional Commits enforcement |
| 206 | Husky + lint-staged for frontend |
| 207 | Local Docker compose dashboard (Lazydocker) |
| 208 | Test data seeder for dev (fixtures) |
| 209 | Postman → .http migration |
| 210 | Onboarding checklist tracking |

---

## H. Anti-Abuse + Security Hardening (16)

| # | Konu | Tetikleyici |
|---|---|---|
| 211 | Listing spam detection (similarity hash) | Listing volume |
| 212 | Image NSFW filter | ML pipeline |
| 213 | Phone number verification fraud signal | OTP abuse |
| 214 | Account takeover detection (login geo anomaly) | Risk-based auth |
| 215 | Brute force IP throttling beyond rate limit | Cloudflare WAF rules |
| 216 | Bot detection (Cloudflare Turnstile / hCaptcha) | Listing form |
| 217 | Fake review detection (graph analysis) | Review volume Faz 2 |
| 218 | Dispute fraud pattern detection | Dispute volume |
| 219 | Self-purchase prevention (same payment method) | Marketplace check |
| 220 | Coupon abuse detection | Subscription Faz 2 |
| 221 | Email enumeration prevention (consistent response) | Identity hardening |
| 222 | Tarpit for failed auth (intentional slow response) | Auth abuse |
| 223 | CSRF double-submit cookie | Web only |
| 224 | Content Security Policy (CSP) tightening | Web hardening |
| 225 | Subresource Integrity (SRI) for CDN assets | Web hardening |
| 226 | Audit log retention REVOKE policy | Compliance |

---

## Backlog Yönetim Kuralları

1. **Yeni item eklerken:** kategori (A-H), modül, tetikleyici, target wave/faz alanlarını doldur
2. **Closed yaparken:** çözüm referansı (modül doc / karar bölümü / commit SHA)
3. **Karar 6/7 kapanış:** ilgili karar doc'unda detay; burada sadece pointer
4. **Numara sırası:** kalıcı — closed item'lar boş bırakılır, yeni item maksimum + 1
5. **Re-open:** closed item yeniden açılırsa "REOPENED" notu + neden

---

## Sonraki Adım

Wave 0 ile beraber bu backlog'a yeni item eklenmeye başlayacak. Wave/karar bazlı groomings haftalık olacak (Karar 7 / Grup C.2 — PR + ops cadence). Her wave kapanışında "Karar X boyunca eklenen + kapanan" özet bu doc'a düşülür.
