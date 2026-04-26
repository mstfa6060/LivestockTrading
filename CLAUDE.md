# CLAUDE.md

Bu dosya Claude Code'a bu repository'de çalışırken rehberlik eder.

> **AKTİF MİMARİ:** `src/` altında Vertical Slice + Modüler Monolit (`LivestockTrading.slnx`).
> `legacy/` klasörü eski ArfBlocks kodunu içerir — **SADECE REFERANS**, dokunma, build'e girmez.

---

## 1. Proje Genel Bilgisi

**livestock-trading.com** — canlı hayvan alım satım platformu. Çiftçiler, satıcılar, alıcılar ve nakliyecileri bir araya getirir; 50+ dil ve çok ülke desteği vardır.

- Mimari: **Modüler Monolit** + **Vertical Slice** pattern
- Runtime: .NET 10, tek `Program.cs` (`src/Bootstrapper/Livestock.Host`)
- Tüm modüller tek process içinde çalışır; modüller arası iletişim NATS event'leri üzerinden

---

## 2. Teknoloji Stack

| Alan | Kütüphane |
|------|-----------|
| Endpoint framework | FastEndpoints 5.x |
| Validation | FluentValidation (FastEndpoints entegre) |
| ORM | EF Core 10 + Npgsql (PostgreSQL 17) |
| Cache | FusionCache (L1 memory + L2 Redis 8) |
| Messaging | NATS JetStream (NATS.Net 2.x) |
| Logging | Serilog → Seq + OTLP |
| Observability | OpenTelemetry (traces + metrics) → Jaeger |
| File Storage | MinIO (S3-uyumlu, Minio SDK) |
| Real-time | SignalR (mesajlaşma hub'ı) |
| Tests | xUnit + NetArchTest.Rules |

---

## 3. Solution Yapısı

```
src/
  Shared/
    Shared.Abstractions/     ← IEndpoint, Result<T>, Error, IUserContext
                               SADECE .NET runtime bağımlılığı — EF/NATS/FastEndpoints YASAK
    Shared.Contracts/        ← IIntegrationEvent + tüm cross-module event'ler
                               Events/Iam/  →  EmailOtpRequestedEvent, UserRegisteredEvent ...
                               Events/Livestock/  →  ProductCreatedEvent, MessageSentEvent ...
    Shared.Infrastructure/   ← FastEndpoints, EF Core, FusionCache, NATS, Serilog, OTel extension'ları
                               Messaging/IEventPublisher, NatsEventPublisher, NatsConsumerBase<T>

  Modules/
    Iam/
      Iam.Domain/            ← User, Role, RefreshToken entity'leri + hata sabitleri
      Iam.Features/          ← Auth/ Users/ Admin/ Consumers/ Services/
      Iam.Persistence/       ← IamDbContext + EF migrations (Npgsql)
    Files/
      Files.Domain/          ← MediaBucket, FileRecord entity'leri
      Files.Features/        ← Buckets/ Files/ Services/
      Files.Persistence/     ← FilesDbContext + migrations
    Livestock/
      Livestock.Domain/      ← Tüm ticaret entity'leri + enum'lar + hata sabitleri
      Livestock.Features/    ← Her entity için slice klasörü (aşağıda liste)
      Livestock.Persistence/ ← LivestockDbContext + migrations

  Bootstrapper/
    Livestock.Host/          ← TEK Program.cs: tüm modülleri mount eder, /health endpoint'i

  Workers/
    Livestock.Workers/       ← NatsConsumerBase<T> tabanlı worker'lar
                               Consumers/  →  Email, SMS, Push notification consumer'ları
                               Services/   →  EmailService, SmsService, PushService, PriceConversionService

tests/
  Architecture.Tests/        ← NetArchTest bağımlılık kuralları (6 test)

deploy/docker/
  Dockerfile.host            ← Livestock.Host image (multi-stage, aspnet runtime)
  Dockerfile.workers         ← Livestock.Workers image
  compose/
    docker-compose.dev.yml       ← Postgres 17, Redis 8, NATS 2.11, MinIO, Seq
                                   (Jaeger profile: tracing — opt-in)
    docker-compose.dev.app.yml   ← host + workers overlay (build veya pull)
    docker-compose.dev.server.yml ← server-side overlay: portları daraltır

Jenkinsfile.dev              ← dev branch push → image build + push + deploy + smoke
```

---

## 4. Modül Sınırları

### IAM Modülü (`src/Modules/Iam/`)
Auth, kullanıcılar ve kimlik yönetimi:
- **Auth**: Login, Register, Logout, RefreshToken, ForgotPassword, ResetPassword, SendOtp, VerifyOtp, SendEmailOtp, VerifyEmailOtp
- **Users**: GetCurrentUser, UpdateProfile
- **Admin/Users**: kullanıcı yönetimi (admin)
- **Consumers**: event consumer'ları (Iam eventlerini dinler)

### Files Modülü (`src/Modules/Files/`)
MinIO üzerinden dosya depolama:
- **Buckets**: Create, Delete (yönetici)
- **Files**: Upload (multipart), Delete, GetPresignedUrl, Reorder, SetCover

### Livestock Modülü (`src/Modules/Livestock/`)
Tüm ticaret domain'i. Feature klasörleri:

| Klasör | Açıklama |
|--------|----------|
| `Products` | İlan yönetimi (search, detail, create, update, delete, slug) |
| `Categories` | Kategori ağacı (çok dil desteği) |
| `Brands` | Marka yönetimi |
| `Sellers` | Satıcı profili, nearby, oto-kayıt |
| `Transporters` | Nakliyeci profili |
| `Farms` | Çiftlik yönetimi |
| `Offers` | Teklif sistemi (create, accept, reject) |
| `Deals` | Tamamlanmış anlaşmalar |
| `Conversations` | Mesajlaşma konuşmaları |
| `Notifications` | Bildirim yönetimi |
| `Reviews` | Ürün/satıcı değerlendirmeleri |
| `Subscriptions` | Abonelik planları (limit kontrolü) |
| `Transport` | Nakliye talepleri |
| `Locations` | Konum bilgileri |
| `Favorites` | Favori ürünler |
| `AnimalInfos` | Hayvan detay bilgisi |
| `HealthRecords` | Sağlık kayıtları |
| `Vaccinations` | Aşı kayıtları |
| `ChemicalInfos` / `FeedInfos` / `SeedInfos` / `MachineryInfos` / `VeterinaryInfos` | Tarım bilgi tipleri |
| `ProductVariants` | Ürün varyantları |
| `ProductViewHistories` | Görüntüleme geçmişi |
| `SearchHistories` | Arama geçmişi |
| `UserPreferences` | Kullanıcı tercihleri |
| `Dashboard` | Satıcı dashboard istatistikleri |
| **`Admin/`** | Yönetici endpoint'leri |
| `Admin/Products` | Ürün onay/red |
| `Admin/Sellers` | Satıcı doğrula/askıya al |
| `Admin/Transporters` | Nakliyeci doğrula/askıya al |
| `Admin/Currencies` | Para birimi yönetimi |
| `Admin/SubscriptionPlans` | Abonelik planı yönetimi |
| `Admin/BoostPackages` | Boost paketi yönetimi |
| `Admin/AppVersions` | Uygulama versiyon yönetimi |
| `Admin/Reports` | Rapor endpoint'leri |

---

## 5. Yeni Endpoint Ekleme Rehberi

### Adımlar

1. **Domain entity** (gerekiyorsa) → `Livestock.Domain/Entities/`
2. **DbSet** ekle → `LivestockDbContext.cs`
3. **Entity configuration** → `Livestock.Persistence/Configurations/`
4. **EF migration** ekle:
   ```bash
   cd src/Modules/Livestock/Livestock.Persistence
   dotnet ef migrations add AddMyEntity --startup-project ../../Bootstrapper/Livestock.Host
   ```
5. **Feature slice** oluştur → `Livestock.Features/MyEntity/`
6. **Endpoint, Models, Validators** dosyalarını yaz (aşağıda örnek)
7. **Architecture testleri** geçiyor mu kontrol et:
   ```bash
   dotnet test tests/Architecture.Tests
   ```
8. **TypeScript client'ı yeniden üret** (web + mobile için):
   ```bash
   bash scripts/generate-api-client.sh
   ```
   Bu komut `Livestock.Host`'u build eder, `generated/openapi.json` ve
   `generated/api-client.ts` üretir, ardından client'ı
   `livestock-frontend/src/api/generated/` ve
   `livestock-mobile/src/api/generated/` altına kopyalar. Frontend dev
   yeni endpoint'i IDE autocomplete'inde tipli olarak görür.

### Endpoint Dosya Yapısı (Vertical Slice)

```
Livestock.Features/MyEntity/
  _Shared/
    Models.cs                 ← shared DTOs (kullanan endpoint sayısı >= 2 ise)
  Create/
    Endpoint.cs               ← tek FastEndpoints Endpoint<Req, Res>
    Models.cs                 ← Request + Response record
    Validator.cs              ← FluentValidation AbstractValidator<TRequest>
  Update/
    Endpoint.cs
    Models.cs
    Validator.cs
  Delete/ ...
  _Legacy/                    ← eski route alias endpoint'leri (geçiş süresince)
```

- Her `Endpoint.cs` **tek bir** `Endpoint<TReq, TRes>` class içerir
- Namespace: `Livestock.Features.{Entity}.{UseCase}` (örn. `Livestock.Features.Categories.Create`)
- Shared DTO sadece **>= 2 endpoint** tarafından kullanılıyorsa `_Shared/` altına alınır
- Legacy route alias endpoint'leri `_Legacy/` klasöründe tutulur
- `tests/Architecture.Tests/VerticalSliceTests` bu kuralı derleme zamanı koruyucusu olarak doğrular (whitelist'e ekli refactor edilmiş slice'lar için)

### Örnek Endpoint

```csharp
// Models.cs
public record CreateCategoryRequest(string Name, string? Slug, Guid? ParentId);
public record CreateCategoryResponse(Guid Id, string Name, string Slug);

// Validator.cs
public class CreateCategoryValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}

// Endpoint.cs
public class CreateCategoryEndpoint(LivestockDbContext db) 
    : Endpoint<CreateCategoryRequest, CreateCategoryResponse>
{
    public override void Configure()
    {
        Post("/livestocktrading/Categories/Create");  // module prefix zorunlu
        Roles("Admin", "Moderator");      // JWT role-based auth
        Tags("Categories");
    }

    public override async Task HandleAsync(CreateCategoryRequest req, CancellationToken ct)
    {
        var entity = new Category { Id = Guid.NewGuid(), Name = req.Name, ... };
        db.Categories.Add(entity);
        await db.SaveChangesAsync(ct);
        await SendAsync(new CreateCategoryResponse(entity.Id, entity.Name, entity.Slug), cancellation: ct);
    }
}
```

### Route Convention

`POST /{modulePrefix}/{Entity}/{Action}` — legacy Ocelot gateway uyumluluğu
için tüm endpoint'ler POST + module prefix kullanır. Tek prefix:
- `iam` — IAM modülü endpoint'leri
- `fileprovider` — Files modülü endpoint'leri
- `livestocktrading` — Livestock modülü endpoint'leri (en yoğun, tüm ticaret)

Örnekler:
- `POST /iam/Auth/Login` — kullanıcı girişi
- `POST /iam/Push/RegisterToken` — mobil push token kaydı
- `POST /fileprovider/Files/Upload` — dosya yükleme
- `POST /livestocktrading/Products/All` — ürün listele/filtrele
- `POST /livestocktrading/Products/Detail` — tekil ürün
- `POST /livestocktrading/Products/Create` — oluştur
- `POST /livestocktrading/Products/Update` — güncelle (id body'de)
- `POST /livestocktrading/Products/Delete` — soft delete (id body'de)
- `POST /livestocktrading/Sellers/Nearby` — Haversine geo-search
- `POST /livestocktrading/Admin/Products/Approve` — admin onayı

`tests/Architecture.Tests/RouteConventionTests` her endpoint'in bu kalıpta
olduğunu derleme zamanı koruyucusu olarak doğrular.

---

## 6. Kurallar

### Kesin Kurallar

- Her endpoint **FastEndpoints** `Endpoint<TReq, TRes>` kullanmalı — controller yok
- Request doğrulama **FluentValidation** ile (`AbstractValidator<T>`)
- Silme işlemleri **soft delete**: `entity.IsDeleted = true; entity.DeletedAt = DateTime.UtcNow`
- Architecture testleri her değişiklikte geçmeli: `dotnet test tests/Architecture.Tests`
- Eski ArfBlocks koduna ihtiyacın olursa `main` branch'ine bak (bu branch'te yok)

### Build ve Test Komutları

```bash
# Solution build
dotnet build LivestockTrading.slnx

# Sadece architecture testleri
dotnet test tests/Architecture.Tests

# Tüm testler
dotnet test LivestockTrading.slnx

# Host'u çalıştır
dotnet run --project src/Bootstrapper/Livestock.Host

# Dev stack (Postgres, Redis, NATS, Jaeger, Seq, MinIO)
cd deploy/docker/compose
docker compose -f docker-compose.dev.yml up -d

# TypeScript client üret + web ve mobile'a kopyala
bash scripts/generate-api-client.sh

# EF migration ekle (örn: Iam modülü)
cd src/Modules/Iam/Iam.Persistence
dotnet ef migrations add MyMigration --startup-project ../../../Bootstrapper/Livestock.Host
```

### Configuration (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=livestock;Username=postgres;Password=...",
    "Redis": "localhost:6379",
    "Nats": "nats://localhost:4222"
  },
  "Jwt": { "SigningKey": "..." },
  "Seq": { "ServerUrl": "http://localhost:5341" },
  "OpenTelemetry": { "OtlpEndpoint": "http://localhost:4317" },
  "Minio": { "Endpoint": "localhost:9000", "AccessKey": "...", "SecretKey": "..." }
}
```

### Soft Delete Pattern

Tüm entity'lerde `BaseEntity`'den gelen:
```csharp
public bool IsDeleted { get; set; }
public DateTime? DeletedAt { get; set; }
```

Sorgularda her zaman filtrele: `.Where(e => !e.IsDeleted)`

### Multi-Country / Multi-Language

- Ürünler `Location.CountryCode` (ISO 3166-1 alpha-2) üzerinden ülkeye bağlı
- Kategoriler `NameTranslations` (JSON) ile çoklu dil destekler: `{"en":"...","tr":"...","de":"..."}`
- `countryCode` boş ise global görünüm (tüm ürünler)

---

## 7. Architecture Test Kuralları

`tests/Architecture.Tests/ModuleDependencyTests.cs` içindeki 5 kural (hepsi geçmeli):

1. **Domain projeleri infrastructure'a bağımlı olamaz** — `Iam.Domain`, `Files.Domain`, `Livestock.Domain` içinde EF Core / FastEndpoints / NATS kullanılamaz
2. **`Iam.Domain`** → `Files.Domain` veya `Livestock.Domain` referans alamaz
3. **`Files.Domain`** → `Iam.Domain` veya `Livestock.Domain` referans alamaz
4. **`Livestock.Domain`** → `Iam.Domain` veya `Files.Domain` referans alamaz
5. **`Shared.Abstractions`** → sadece .NET runtime (FastEndpoints, EF, NATS, Serilog YASAK)

Modüller arası iletişim: `Shared.Contracts` üzerinden `IIntegrationEvent` ile.

---

## 8. Event / Messaging Pattern

### Event Publish (Features katmanında)

```csharp
// Constructor injection
public class CreateProductEndpoint(LivestockDbContext db, IEventPublisher events) 
    : Endpoint<CreateProductRequest, CreateProductResponse>
{
    public override async Task HandleAsync(CreateProductRequest req, CancellationToken ct)
    {
        // ... entity oluştur, kaydet ...

        await events.PublishAsync("livestock.product.created", new ProductCreatedEvent
        {
            ProductId = entity.Id,
            SellerId = entity.SellerId,
            Title = entity.Title
        }, ct);
    }
}
```

### Event Subject Convention

`{module}.{entity}.{action}` formatı:

| Subject | Event |
|---------|-------|
| `iam.user.registered` | `UserRegisteredEvent` |
| `iam.otp.email-requested` | `EmailOtpRequestedEvent` |
| `iam.otp.sms-requested` | `OtpSmsRequestedEvent` |
| `livestock.product.created` | `ProductCreatedEvent` |
| `livestock.product.approved` | `ProductApprovedEvent` |
| `livestock.product.rejected` | `ProductRejectedEvent` |
| `livestock.offer.created` | `OfferCreatedEvent` |
| `livestock.offer.accepted` | `OfferAcceptedEvent` |
| `livestock.message.sent` | `MessageSentEvent` |
| `livestock.conversation.created` | `ConversationCreatedEvent` |
| `livestock.seller.registered` | `SellerRegisteredEvent` |
| `livestock.seller.verified` | `SellerVerifiedEvent` |
| `livestock.transporter.registered` | `TransporterRegisteredEvent` |
| `livestock.transporter.verified` | `TransporterVerifiedEvent` |

### Consumer Yazma (Workers katmanında)

```csharp
// src/Workers/Livestock.Workers/Consumers/
public class ProductCreatedConsumer(IEmailService email) 
    : NatsConsumerBase<ProductCreatedEvent>(nats)
{
    protected override string Subject => "livestock.product.created";

    protected override async Task HandleAsync(ProductCreatedEvent msg, CancellationToken ct)
    {
        await email.SendProductCreatedNotificationAsync(msg.SellerId, msg.Title, ct);
    }
}
```

---

## 9. Mevcut Durum

### Tamamlanan PR'lar (10+ adet)

| PR | İçerik |
|----|--------|
| #1 | Solution iskeleti — Shared katmanı, Bootstrapper, Architecture testleri |
| #2 | IAM modülü — Auth (Login/Register/Refresh/Logout/OTP), Users, Admin/Users |
| #3 | Files modülü — Upload/Download/Delete/Presigned URL/Reorder/SetCover + MinIO |
| #4 | Livestock Core — Categories, Brands, Products, Sellers, Farms, Offers, Deals, Favorites |
| #5 | Livestock Messaging — Conversations, Messages, SignalR Hub, NATS event'ler |
| #6 | Workers — Email/SMS/Push notification consumer'ları, PriceConversionService |
| #7 | Admin/Moderation — Approve/Reject/Verify/Suspend + tüm admin endpoint'leri |
| #8 | **Route refactor** — tüm 209 endpoint `POST /{prefix}/{Entity}/{Action}` kalıbına; RouteConventionTests guard testi eklendi |
| #9 | **P0 düzeltmeleri** — Subscribe IAP receipt + duplicate guard, Sellers/Transporters IsActive/IsVerified flag'leri (NotMapped computed), `/iam/Push/RegisterToken+RevokeToken`, Login UserName + Google/Apple OAuth, Register admin email whitelist, SignalR `/hubs/chat` (FusionCache presence) |
| #10 | **P1 düzeltmeleri** — `/livestocktrading/Conversations/UnreadCount`, `/livestocktrading/Sellers/Nearby` (Haversine), Reviews CRUD (Update + Delete + TransporterReview Create), SkiaSharp image processing pipeline (WebP + thumbnail), Shipping domain (Carriers + Zones + Rates, 3 entity + 15 endpoint) |
| #11 | **EF migrations** — `Iam.Persistence`, `Files.Persistence`, `Livestock.Persistence` için `InitialCreate` baseline migration'ları + `IDesignTimeDbContextFactory`'ler |
| #12 | **NSwag pipeline** — `nswag.json` + `scripts/generate-api-client.sh` + `RouteBasedOperationIdProcessor` + `NoopEventPublisher` (codegen guard); 39 typed client class + 209 method `generated/api-client.ts`'de, web ve mobile'a otomatik kopyalanır |

### Business Logic Düzeltmeleri (Son merge)

- **Auto-seller**: Ürün oluşturan kullanıcıya otomatik Seller rolü atanır
- **Subscription limits**: Ürün oluşturmadan önce aktif abonelik limiti kontrol edilir
- **Multi-currency**: Fiyatlar kullanıcının para birimine dönüştürülür (PriceConversionService)
- **Timezone expiry**: Abonelik ve token süreleri UTC yerine kullanıcı timezone'una göre hesaplanır
- **Role assignment**: Yeni kayıtta Buyer rolü otomatik atanır
- **Typing indicator**: SignalR üzerinden doğru gönderilir
- **Unread count**: Conversation listesinde doğru hesaplanır
- **Slug detail**: Ürün slug'a göre de sorgulanabilir
- **Nearby sellers**: Coğrafi yakınlık filtrelemesi
- **Soft-delete**: Tüm entity'lerde tutarlı uygulandı

### Konuşma / Mesajlaşma (SignalR)

Hub endpoint: `/hubs/chat` — JWT auth gerekli

Client → Server: `JoinConversation`, `LeaveConversation`, `SendTypingIndicator`, `MarkMessageAsRead`, `GetOnlineUsers`

Server → Client: `ReceiveMessage`, `TypingIndicator`, `MessageRead`, `UserOnline`, `UserOffline`

---

## 10. Bilinen Limitasyonlar / TODO

- **Production deploy**: Dev sunucu (`dev-api.livestock-trading.com`) yeni mimaride çalışıyor (Jenkinsfile.dev → otomatik deploy). Production için ayrı `Jenkinsfile.prod` + production compose overlay henüz yok.
- **E2E testler**: Henüz yok. Architecture testleri var (`tests/Architecture.Tests` — 6 test: 5 dependency + 1 route convention), integration/E2E altyapısı eksik.
- **IAP receipt verification**: Apple App Store / Google Play receipt'i Subscribe endpoint'inde sadece kayıt + duplicate check; gerçek doğrulama API çağrısı henüz yok (P2'de planlandı).
- **OAuth provider verification**: Login Google/Apple `ExternalProviderUserId`'yi doğrulanmış kabul ediyor — gerçek JWT verification eklenmeli.

## 11. Branch Stratejisi

- **`main`** — `ab0b0d1` commit'inde (eski ArfBlocks mimarisi, canlıdaki stack'in karşılığı). Yeni geliştirme için ASLA kullanılmaz.
- **`dev`** — `331253c` ve sonrası (Vertical Slice + FastEndpoints + tüm yeni geliştirme). Tüm feature work burada.
- Yeni feature için: `git checkout dev && git checkout -b feat/X` → PR `dev`'e açılır.
- Hazır olunca `dev → main` merge tek bir büyük review oturumunda yapılır.

Aynı strateji `livestock-frontend` ve `livestock-mobile` repo'larında geçerli — üç repo da `dev` branch'inde paralel ilerler.

## 12. Migration Gaps Raporu

`_doc/MIGRATION_GAPS.md` ESKİ ArfBlocks (artık `main` branch'te durur — bu repo'da silindi)
ile yeni FastEndpoints arasındaki endpoint kapsamı + davranış delta'sını listeler.
Her yeni feature başlamadan önce bu raporu kontrol et — P2 listesinde hâlâ açık
maddeler var (Banners, FAQs, Languages, PaymentMethods, TaxRates, ContactForms,
GeoIp/Geolocation, Audit log altyapısı, FusionCache referans veriler için).

## 13. CI/CD

- **`Jenkinsfile.dev`** (repo root) — `dev` branch'e push tetikler:
  1. `mstfaock/livestocktrading-backend:dev-latest` (host) ve
     `mstfaock/livestocktrading-backend-workers:dev-latest` (workers)
     image'larını build edip Docker Hub'a push eder
  2. Compose dosyalarını sunucuda `/opt/livestocktrading/dev-app/`'e scp eder
  3. `docker compose pull + up` ile uygulama container'larını yeniden başlatır
     (infra container'larına dokunmaz)
  4. `/health` ve `/swagger/v1/swagger.json` smoke test
- Gerekli Jenkins credential'ları:
  `dockerhub-credentials`, `maden-server-key`, `dev-server-host`.
- Production için ayrı `Jenkinsfile.prod` henüz yok.

## Eski Mimari Notu

ArfBlocks tabanlı eski kod (`BaseModules/`, `BusinessModules/`, `Common/`,
`Gateways/`, `Jobs/`, eski `_devops/`) bu branch'ten temizlendi. Tarihsel
referans için `main` branch'inde (`ab0b0d1`) saf eski hali korunuyor —
SQL Server, RabbitMQ, Ocelot Gateway, ArfBlocks CQRS, Redis, Serilog stack'i.
