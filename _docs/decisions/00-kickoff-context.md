# Livestock Trading — Backend Kickoff Context

## Proje Genel

**Domain:** livestock-trading.com
**Tip:** Global B2B tarımsal pazaryeri — büyükbaş, küçükbaş, kümes, tarımsal malzeme, damızlık, ihracat
**Hedef:** 50 dil + 180 ülke (gerçek hedef, "ileride bakarız" değil)
**Launch sırası:** TR → MENA (Azerbaycan, Kazakistan) → Orta Doğu → Batı pazarları
**Mevcut durum:** Eski sistem hâlâ canlıda (livestock-trading.com), ama dış kullanıcı yok. Yeni sistem hazır olunca eski tamamen kapatılacak. **Veri taşıma yok**, DB temizlenecek, sıfırdan.

---

## Stratejik Karar — Sıfırdan Rewrite

**Rewrite gerekçesi:**
1. ArfBlocks framework'ünden kurtulmak — eski MassTransit'e geçiş yarıda kalmıştı, sıfırdan başlamak temiz keser
2. Mevcut domain modeli yeni gereksinimleri (50 dil, 180 ülke, yeni persona'lar — alıcı/satıcı/taşıyıcı/admin) tam karşılamıyor
3. Veri taşıma maliyeti sıfır (kullanıcı yok), rewrite'ın en pahalı kısmı yok
4. En iyi modern stack ile başlama fırsatı

---

## Frontend Durumu (referans için)

Frontend ayrı repoda zaten yenilendi:

- **Repo:** `mstfa6060/livestock-frontend`
- **Branch:** `feat/v2-rewrite`
- **Stack:** Next.js 16 + React 19 + TypeScript + Tailwind v4 + shadcn/ui + Zustand + TanStack Query + next-intl
- **Yapı:** pnpm workspace + Turborepo monorepo (`apps/web`, `apps/mobile` planlı, `packages/*` paylaşımlı)
- **Durum:** 62 ekran port edilmiş, navigation wiring tamam, design tokens turunda
- **API ile bağlantı:** Şu an MSW (Mock Service Worker) kuruluyor — backend hazır olunca real API'ye geçecek

### Frontend backend'den ne bekliyor

- **Auth:** JWT + refresh token rotation. Mobile için secure storage'a token saklama yapılacak.
- **i18n:** Backend kullanıcı locale'i `Accept-Language` header'ından veya query param'dan okumalı. Hata mesajları lokalize gelmeli.
- **Currency:** Multi-currency. Backend kur conversion'ı yapacak (frontend değil), `?currency=USD` gibi query param'la veya user preference'tan.
- **Geo:** IP-based country detection backend'de. Response'a kullanıcının country code'u dönmeli.
- **Pagination:** Cursor-based önerilen (offset değil) — sonsuz scroll için.
- **Real-time:** Mesajlaşma için SignalR.
- **File upload:** Görseller MinIO'ya, response'ta URL.

---

## Tech Stack — Karar Verildi

**Tüm bileşenler ücretsiz / open source. Toplam lisans maliyeti: 0.**

### Çekirdek

| Katman | Seçim |
|---|---|
| Runtime | .NET 10 LTS (Kasım 2028'e kadar destek) |
| Web Framework | ASP.NET Core 10 (Minimal API) |
| Dil | C# 14 |

### Application Layer

| Katman | Seçim |
|---|---|
| Mediator / CQRS | MassTransit.Mediator |
| Validation | FluentValidation |
| Mapping | Mapster |
| API Docs | Scalar |

### Data Layer

| Katman | Seçim |
|---|---|
| ORM | EF Core 10 (+ Dapper hybrid for read-heavy) |
| Database | PostgreSQL 17 |
| GIS | PostGIS |
| Cache | Redis 7 |
| Search (Faz 1) | PostgreSQL FTS |
| Search (Faz 2) | Meilisearch |

### Auth & Security

| Katman | Seçim |
|---|---|
| Auth | OpenIddict (OAuth2/OIDC + JWT + passkey) |
| Password Hashing | BCrypt.Net-Next |
| Refresh Token | JWT + rotation pattern |

### Async / Messaging / Real-time

| Katman | Seçim |
|---|---|
| Message Queue | RabbitMQ |
| Real-time | SignalR |
| Background Jobs | MassTransit + Quartz.NET |

### Storage & Files

| Katman | Seçim |
|---|---|
| Object Storage | MinIO (S3 uyumlu, self-hosted) |
| Image Processing | SixLabors.ImageSharp (WebP, thumbnail) |

### Communication

| Katman | Seçim |
|---|---|
| Email Library | MailKit + MimeKit |
| Email Provider | SMTP veya Resend |
| SMS | Twilio |
| WhatsApp | WhatsApp Business API (faz 2) |

### Observability

| Katman | Seçim |
|---|---|
| Logs | Serilog → Loki |
| Metrics | OpenTelemetry → Prometheus |
| Traces | OpenTelemetry → Tempo |
| Dashboards | Grafana |
| Error Tracking | Sentry self-hosted |

### DevOps & Infrastructure

| Katman | Seçim |
|---|---|
| Container | Docker + Docker Compose (faz 1) |
| Orchestration (Faz 2) | K3s veya Kubernetes |
| Reverse Proxy | nginx |
| CI/CD | Jenkins + GitHub Actions |
| Container Registry | Private (localhost:5050) |

### Testing

| Katman | Seçim |
|---|---|
| Unit Test | xUnit |
| Assertions | FluentAssertions (v9 ücretsiz tier) |
| Mocking | NSubstitute |
| Integration Test | Testcontainers (real DB ile test) |

### Architecture Patterns

| Pattern | Seçim |
|---|---|
| Yapı | Modüler Monolith |
| Code Organization | Vertical Slice (use-case-per-folder) |
| API Style | REST |
| Domain Pattern | Lightweight DDD (aggregate roots, value objects, domain events) |

### Tek Bakışta Özet

```
Runtime:         .NET 10 LTS + C# 14
Web:             ASP.NET Core 10 (Minimal API)
Mediator:        MassTransit.Mediator
Validation:      FluentValidation
Mapping:         Mapster
API Docs:        Scalar

ORM:             EF Core 10 + Dapper (hybrid)
Database:        PostgreSQL 17 + PostGIS
Cache:           Redis 7
Search:          PostgreSQL FTS → Meilisearch (faz 2)
Storage:         MinIO
Image:           SixLabors.ImageSharp

Auth:            OpenIddict + JWT + passkey
Password:        BCrypt.Net-Next
Queue:           RabbitMQ
Real-time:       SignalR
Background:      Quartz.NET

Email:           MailKit
SMS:             Twilio
WhatsApp:        WhatsApp Business API (faz 2)

Logs:            Serilog → Loki
Metrics:         OpenTelemetry → Prometheus
Traces:          OpenTelemetry → Tempo
Dashboards:      Grafana
Errors:          Sentry self-hosted

Container:       Docker + Docker Compose
Reverse proxy:   nginx
CI/CD:           Jenkins + GitHub Actions
Testing:         xUnit + FluentAssertions + NSubstitute + Testcontainers

Yapı:            Modüler Monolith (Vertical Slice)
API:             REST
```

---

## Domain Model (frontend tasarımından çıkarılan)

Frontend'de "Otlak Web.html" 62 ekran var. Anlamlı entity'ler:

### Core

- **User** — alıcı + satıcı + taşıyıcı + admin, role-based
- **Seller** (User'a 1:1) — onboarding state, sertifikalar, abonelik, boost paketleri
- **Carrier** (User'a 1:1) — taşıma firması, hizmet bölgeleri, ücret tablosu
- **Listing** (İlan) — polymorphic content per category
  - Türe göre farklı alanlar (büyükbaş: ırk, yaş, kilo, sağlık raporu; malzeme: kategori, marka, model)
  - Çoklu görsel (MinIO)
  - Sertifikalar
  - Lokasyon (PostGIS point + country)
  - Fiyat + para birimi
  - Status (active, sold, paused, moderation)

### Reference Data (INT identity, seed-based)

- **Country**, **Currency**, **Language**, **Category**, **Breed**, **CertificationType**

### Marketplace

- **Favorite**, **Conversation** + **Message** (SignalR), **Offer**, **Agreement**, **Notification**, **Subscription** (filter-based bildirim)

### Operasyon

- **Onboarding** (state machine), **BoostPackage**, **AbonelikPaket**, **Moderation**, **Report**

### Hibrit ID Stratejisi

- **Business entities:** GUID (User, Listing, Conversation, Message, Offer, vs.)
- **Reference/lookup tables:** INT (Country, Currency, Category, Breed, vs.)

---

## Sıradaki Karar Noktaları

İlk session'da şu kararları sırayla al, **kod yazmadan önce**:

1. **Solution yapısı** — modüller ABP-tarzı ayrı projelere mi, tek projede klasör tabanlı mı, yoksa hybrid (modüller class library projesi olarak ama tek host)?
2. **Bounded Context'ler** — Identity, Listings, Marketplace, Messaging, Admin gibi ayrımlar nasıl olmalı?
3. **Aggregate Root'lar ve Domain Event'ler** — hangi entity aggregate root, hangileri ona bağlı?
4. **İlk migration planı** — hangi tablolar ilk wave'de oluşacak?
5. **Folder convention** — vertical slice nasıl yerleşecek? (`Modules/Listings/Features/CreateListing/...`)
6. **API contract'ı nasıl çıkacak?** — Frontend MSW handler'ları mı kaynak, sen mi domain-first çıkaracaksın?
7. **Migration timing** — Frontend tarafı 3-4 haftalık scope'ta. Backend ne zaman frontend'le bağlantıya hazır olur?

---

## Önemli Frontend Notları

Backend'in karşılaması gereken endpoint pattern'leri (frontend ekranlarından çıkıyor):

```
GET    /api/listings?category=...&country=...&breed=...&priceMin=...&priceMax=...&radius=...&cursor=...
GET    /api/listings/{id}
POST   /api/listings
PUT    /api/listings/{id}
DELETE /api/listings/{id}

GET    /api/categories                  # tree response
GET    /api/breeds?categoryId=

POST   /api/auth/register
POST   /api/auth/login
POST   /api/auth/refresh
POST   /api/auth/forgot-password
POST   /api/auth/reset-password

GET    /api/me                          # locale, currency, country, role
PUT    /api/me/preferences

GET    /api/sellers/{id}
GET    /api/sellers/me
POST   /api/sellers/onboarding
GET    /api/sellers/nearby?lat=&lng=&radius=

GET    /api/carriers/{id}
GET    /api/carriers/me
POST   /api/carriers/onboarding

GET    /api/conversations
GET    /api/conversations/{id}/messages
POST   /api/conversations/{id}/messages
WS     /api/hubs/chat                   # SignalR

POST   /api/offers
PUT    /api/offers/{id}/accept
PUT    /api/offers/{id}/reject

GET    /api/geo/detect                  # IP-based
GET    /api/currencies/rates

POST   /api/uploads/image
POST   /api/uploads/document

GET    /api/admin/users
GET    /api/admin/listings
PUT    /api/admin/moderation/{id}
...
```

---

## Eski Repo (Referans)

`mstfa6060/LivestockTrading` — eski .NET 8 backend kodu burada. **Tamamen yeni repo açılacak**, ama eski kodu **referans** olarak kullanabilirsin (özellikle: MassTransit handler pattern'leri, vertical slice yapısı, PostGIS query'leri, IAM modülü, SignalR hub yapısı). Eski domain modelinde işine yarayan parçalar var.

### Eski repo'da gözüne kestirebileceğin yerler

- `BusinessModules/Listings/` — vertical slice refactor başlamış (MST-75, MST-78)
- `BusinessModules/Iam/` — auth + user management
- `Common/` — paylaşımlı kod
- `_devops/` — Jenkins pipeline'ları
- `Jobs/` — background workers
- `Gateways/` — API gateway konfigürasyonu

---

## Geliştirme Yaklaşımı (Kullanıcı Tercihi)

- **Adım adım ilerleme** — tek mesajda bir adım, çıktı kontrol → sonraki adım
- **Toplu komut listesi karışıklık yaratıyor** — tek tek
- **Conventional Commits** formatı (feat:, fix:, chore:, docs:, refactor:, test:)
- Türkçe iletişim, kullanıcı yazılımcı (Mustafa)
- VS Code + Git Bash (MINGW64 / Windows)
- Türkçe karakter için heredoc'ta `<<'EOF'` (single-quoted)
- **Plan-first yaklaşım** — kod yazmadan önce architecture document, BRD, implementation plan

---

## Altyapı (Mevcut)

- **Sunucu:** 45.143.4.64 (Ubuntu, Docker)
- **Domain:** livestock-trading.com (Cloudflare DNS)
- **Hazır servisler:** Redis, RabbitMQ, MinIO, PostgreSQL, nginx, Grafana stack
- **CI/CD:** Jenkins (mevcut pipeline'lar), GitHub Actions (paralel)
- **Container Registry:** Private (localhost:5050)

---

## İlk Görev (yeni Claude session'a verilecek talimat)

Yeni Claude project'inde Claude'a şunu söyle:

```
Livestock Trading'in backend'ini sıfırdan kuruyoruz. Tüm stack
kararları alındı (BACKEND_KICKOFF_CONTEXT.md'deki "Tek bakışta özet"
bölümü). Eski sistemi tamamen kapatıp yenisini yayına alacağız,
veri taşıma yok.

Şimdi yapacağın:
1. Önce mimari karar alacağımız konuları listele:
   - Solution yapısı (modül bölünmesi)
   - Bounded context'ler
   - Aggregate root'lar ve domain events
   - İlk migration planı
   - Folder convention'ı (vertical slice nasıl yerleşecek)

2. Bu kararları sırayla, her birinde dur ve onay al.
3. KOD YAZMA, sadece architecture document üret.
4. Sonra tek tek modülleri planlarız.

İlk soru: Solution yapısı nasıl olsun? Modülleri ABP-tarzı ayrı
projeler halinde mi, yoksa tek projede klasör tabanlı mı?
```
