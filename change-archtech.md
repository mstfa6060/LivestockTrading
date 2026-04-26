Canlıda olmaması büyük avantaj — backward-compat endişesi yok, temiz bir iskelet kurabiliriz. İşte sıfırdan önerim:

## Hedef mimari (özet)

**Vertical Slice + Modüler Monolit**. Her özellik kendi dikey dilimi; modüller arası sadece `Contracts` (event) paketleri konuşur, DB erişimi paylaşılmaz.

## Solution yapısı

```
LivestockTrading.slnx
├── src/
│   ├── Shared/                              ← shared kernel (küçük tut)
│   │   ├── Shared.Abstractions/             ← IEndpoint, Result<T>, Error, IUserContext
│   │   ├── Shared.Infrastructure/           ← Serilog, OTel, Auth, Cache, Nats, EF base
│   │   └── Shared.Contracts/                ← cross-module integration events
│   │
│   ├── Modules/
│   │   ├── Iam/
│   │   │   ├── Iam.Api/                     ← endpoint'ler + DI composition
│   │   │   ├── Iam.Features/                ← Features/Auth/Login/, Features/Users/Create/
│   │   │   ├── Iam.Domain/                  ← entity + domain event
│   │   │   └── Iam.Persistence/             ← IamDbContext + migrations
│   │   │
│   │   ├── Files/                           ← aynı yapı
│   │   └── Livestock/                       ← aynı yapı
│   │
│   ├── Bootstrapper/
│   │   └── Livestock.Host/                  ← tek Program.cs, tüm modülleri monte eder
│   │
│   └── Workers/
│       ├── Workers.Notifications/           ← NATS consumer
│       ├── Workers.Email/
│       └── Workers.Sms/
│
├── tests/
│   ├── Iam.Tests/
│   ├── Livestock.Tests/
│   └── Architecture.Tests/                  ← NetArchTest ile bağımlılık kuralları
│
├── deploy/
│   └── docker/
│
└── Directory.Packages.props                 ← central package management
```

**Neden tek host?** Şu an 3 ayrı API + gateway var. Modüler monolit'te tek `Host` projesi tüm modülleri mount eder; istediğin zaman mikroservise bölebilirsin. Gateway/Ocelot tamamen gider.

## Dikey dilim örneği

```
Iam.Features/
└── Auth/
    └── Login/
        ├── LoginEndpoint.cs        ← FastEndpoints Endpoint<Req,Res>
        ├── LoginHandler.cs         ← iş mantığı (static ya da küçük class)
        ├── LoginModels.cs          ← Request + Response + (gerekiyorsa) mini DTO'lar
        └── LoginValidator.cs       ← FluentValidation
```

Yani **4 dosya**, Mapper inline, Verificator yerine endpoint'te `Roles()`/`Policies()` + Validator domain kontrolü. DataAccess slice dışında (DbContext doğrudan inject) — vertical slice'ın fikri de budur.

## Teknoloji seçimleri

| Alan | Seçim | Neden |
|---|---|---|
| **.NET** | 10.0 | Zaten oradasın |
| **Solution** | `.slnx` | İstediğin gibi |
| **Endpoint framework** | **FastEndpoints** | REPR, convention-routing, Swagger/auth entegre, DI hafif |
| **Validation** | FluentValidation (FastEndpoints'e entegre) | |
| **Mapping** | **Manuel** (`Mapperly` opsiyonel) | AutoMapper artık ücretli, manuel mapping vertical slice'la iyi gider |
| **ORM** | EF Core 10 | NetTopologySuite zaten var |
| **DB migration** | EF Core tooling + modül başına ayrı DbContext | |
| **Messaging** | **NATS JetStream** (`NATS.Net` 2.x) | Queue group + durable consumer = worker pattern |
| **Cache** | Redis + `FusionCache` | L1+L2'yi kendin yazmak yerine hazır kütüphane; mevcut custom kod atılabilir |
| **Auth** | `Microsoft.AspNetCore.Authentication.JwtBearer` + `JwtService` (token üretir) | Her API kendi token'ını doğrular |
| **Logging** | Serilog + `Serilog.Sinks.OpenTelemetry` | |
| **Observability** | **OpenTelemetry** (traces + metrics + logs) → OTLP | ASP.NET Core, EF Core, Redis, NATS, HttpClient instrumentation |
| **Background jobs** | Hangfire ya da `Coravel` (daha hafif) | Mevcut Hangfire kalabilir |
| **File storage** | MinIO/S3 + `Minio` SDK | Mevcut zaten |
| **Tests** | xUnit + `Testcontainers` (Postgres/Redis/NATS) + `WebApplicationFactory` + NetArchTest | |
| **Code quality** | `Directory.Build.props` → `Nullable=enable`, `TreatWarningsAsErrors`, `.editorconfig` | |

## Neleri taşıyacağız, neleri atacağız

**Tut:**
- `Domain/Entities/*` — entity'lerin kendisi (modül başına ayrılır)
- `SeedData/countries.json` ve CountrySeeder mantığı
- `JwtService` (token üretim mantığı)
- EF migration geçmişi (yeni DbContext'lere split edilir)
- `.http` test dosyaları — regresyon için
- Serilog konfigürasyonu, Redis cache anahtar düzeni
- `PermissionService` / `RoleConstants` mantığı (endpoint'e `Roles()` olarak taşınır)

**At:**
- Tüm ArfBlocks bağımlılıkları + 7 csproj referansı
- `RequestHandlers/**` (Handler/Models/DataAccess/Mapper/Validator/Verificator 6-dosya iskeleti) → yeni `Features/**` yapısına yeniden yazılır
- `ApplicationDependencyProvider` sınıfları → her modülün kendi `IServiceCollection` extension'ı (`AddIamModule()` vb.)
- `DbVerificationService` / `DbValidationService` → Validator içine ya da küçük domain servislerine iner
- Ocelot API Gateway + `Gateways/Api` projesi → tek host'ta gerek yok
- RabbitMQ tüm kod + docker servisi → NATS
- Custom 2-tier cache kodu → FusionCache
- 3 ayrı Api projesi + 2 ayrı Worker proje grubu → tek Host + tek Workers klasörü

## Önerdiğim başlangıç planı

1. **PR #1 — İskelet**: `LivestockTrading.slnx`, `Shared/Shared.Abstractions`, `Shared/Shared.Infrastructure` (Serilog+OTel+Auth+Cache+Nats extension'ları), bir tane "hello" FastEndpoints endpoint'i, docker-compose (Postgres/Redis/NATS/Jaeger/Grafana). **Çalışır, boş iskelet.**
2. **PR #2 — IAM modülü**: Auth (Login/Register/Refresh/Logout) + Users CRUD + Roles. Testlerle.
3. **PR #3 — Files modülü**: Upload/Download/Delete + MinIO integration.
4. **PR #4 — Livestock Core**: Category/Brand/Product + FavoriteProducts.
5. **PR #5 — Livestock Messaging**: Conversations/Messages + SignalR Hub + NATS event'ler.
6. **PR #6 — Workers**: Notification/Email/Sms consumer'ları.
7. **PR #7 — Admin/Moderation**: Approve/Reject/Verify/Suspend endpoint'leri.

Her PR **başlı başına çalışır**, önceki PR'ların üstüne inşa eder. İlk PR'da framework kararları taşa kazınır, sonrası tekrar.

## Onayına ihtiyacım olan kararlar

1. **SQL Server mı, Postgres mı?** Greenfield ise Postgres öneririm (OSS, Docker ile trivial, NetTopologySuite Postgres'te daha iyi).
2. **Tek host + modüler monolit** tamam mı, yoksa üç ayrı API kalsın mı?
3. **FastEndpoints** kesinleşsin mi?
4. **FusionCache** (mevcut custom cache atılır) tamam mı?
5. **Mapperly** mı, **tamamen manuel mapping** mi?
6. **Eski kod**: Mevcut `src/`'yi yeni branch'te olduğu gibi bırakıp `src-new/` altında mı kuralım, yoksa branch'te eski kodu silip yeniden mi başlayalım?

Bu 6 soruyu cevapladığında **PR #1'i (iskelet) hazırlarım** — hiçbir şey yazmadan önce senin onayını alacağım.

İşte benim kararlarım — her biri gerekçesiyle:

## 1. Postgres

**SQL Server yerine PostgreSQL.** Greenfield + canlıda değil + OSS avantajı + Docker'da dakikada kalkar + NetTopologySuite'in (spatial) Postgres tarafı çok olgun + lisans maliyeti yok. EF Core provider'ı (`Npgsql.EntityFrameworkCore.PostgreSQL`) production-grade. Mevcut migration'ları silip sıfırdan üretiriz (zaten canlı veri yok).

## 2. Tek host + modüler monolit

**Tek `Livestock.Host` projesi, üç modül (Iam, Files, Livestock) mount edilir.**

- Gateway tamamen gider, Ocelot/YARP gereksiz.
- Deploy tek container, ops yükü azalır.
- Modüller arası sınır *kodda* (proje referansı yok, sadece `Shared.Contracts` üzerinden event) — dolayısıyla yarın birini ayırmak gerekirse saatler içinde mikroservise çıkar.
- SignalR Hub aynı host'ta kaldığından connection sticky-session dertleri de yok.

Ayrı API'leri savunabilmek için "farklı deploy frekansı / farklı ölçeklendirme" gerekçeniz olmalı — şu an yok.

## 3. FastEndpoints

**Evet, FastEndpoints.** ArfBlocks'un convention-routing ruhuna en yakın olan bu. Avantajları:
- REPR (Request-Endpoint-Response) → vertical slice ile native uyum.
- FluentValidation, Swagger, JWT, rate limiting, versioning tek paketten.
- Minimal API'den daha yapılandırılmış, MediatR'dan daha az boilerplate.
- Kaynak kod üretici (source generator) ile reflection'sız route discovery.

MediatR'ı **şimdilik almayalım** — FastEndpoints handler + endpoint birleşik yapısında MediatR katmanı gerçekten boş yere dolaylılık ekler. Domain event'leri için ileride küçük bir `INotificationPublisher` yazarız, tam MediatR'a gerek yok.

## 4. FusionCache

**Evet, FusionCache.** Mevcut custom L1+L2 kodu atılır. Sebep:
- Hazır L1 (memory) + L2 (Redis) + `SafeMode` + `Fail-Safe` + `Background Refresh` — hepsi out-of-the-box.
- OpenTelemetry desteği dahili.
- Cache stampede koruması var (mevcut kodda muhtemelen yok).
- Aktif geliştiriliyor, tek kişilik `ZiggyCreatures` değil, Microsoft'un Hybrid Cache API'si de bundan ilham aldı.

## 5. Manuel mapping, ama projection için `LinqToDB`/`Mapperly` yok

**Manuel extension method'lar.** `ToResponse()` / `ToEntity()` şeklinde. Sebep:
- Mapper dosyaları 327 handler × ortalama 2 method'luk bir boilerplate — vertical slice'da `Models.cs` içine inline edilirse dosya sayısı düşer.
- Mapperly source generator iyi ama *bir bağımlılık daha* demek; 4 dosyalık slice yapısında getirisi düşük.
- EF projection'ları zaten `Select(x => new ResponseModel { ... })` ile DB tarafına itilir — en performanslı yol.

Karar: **Mapper dosyası yok.** `Models.cs` içinde `static class RequestModelMappings { public static Entity ToEntity(this Request r) ... }`.

## 6. `src-new/` altında paralel kurulum, mevcut kod bir PR'da silinir

**Öneri:** 
- Yeni iskeleti `src/` altında **temiz** kuralım (eski klasörleri silmeyelim henüz).
- Eski kod `legacy/` klasörüne taşınır — referans olarak kalır, build'e girmez.
- Her modül migrasyonu tamamlandığında, o modülün `legacy/` karşılığı silinir.
- **Son PR:** `legacy/` tamamen silinir, ArfBlocks paketleri sonsuza dek gider.

Bu şekilde:
- Eski `.http` dosyaları, entity tanımları, seed data, JwtService kod referansı elinizin altında kalır.
- Yeni kod git history'de temiz başlar, eski kirli commit'ler karışmaz.
- Geri dönüş noktası her zaman var.

## Ek olarak benim eklediklerim

7. **Central Package Management** (`Directory.Packages.props`) — 7 csproj'da sürüm tutarsızlığı bir daha olmasın.
8. **`.editorconfig` + `Directory.Build.props`** — `Nullable=enable`, `TreatWarningsAsErrors=true`, `ImplicitUsings=enable`, `InvariantGlobalization=true`.
9. **Docker Compose'da dev stack**: Postgres + Redis + NATS + Jaeger + Grafana + Seq (Serilog için dev UI). Jenkins tarafına dokunmayalım şimdilik.
10. **Testcontainers** — integration testlerde gerçek Postgres/Redis/NATS. Fake/InMemory provider bug kaçırıyor.
11. **NetArchTest ile bir `Architecture.Tests` projesi** — "Livestock modülü Iam'in internal'ına ref veremez", "Features Domain'e ref verebilir ama tersi olamaz" gibi kurallar test olarak yazılır. Bu kurallar olmazsa modüler monolit 6 ay sonra yine spagetti olur.
12. **OpenAPI schema'yı build'e bağla**: FastEndpoints'in Swagger output'u CI'da `openapi.json`'a dump edilir, frontend client bundan üretilir (arf-cli yerine `NSwag` veya `Kiota`).

## Başlamaya hazırım

Eğer bu 12 maddede hemfikirsek, **PR #1 - İskelet** için yapacağım iş:

1. `LivestockTrading.slnx` oluşturulur (eski `.sln` silinmez, yanında durur).
2. `src/Shared/Shared.Abstractions` + `src/Shared/Shared.Infrastructure` + `src/Shared/Shared.Contracts` projeleri.
3. `src/Bootstrapper/Livestock.Host` — Program.cs, FastEndpoints + Serilog + OTel + JwtBearer + FusionCache + NATS + Npgsql kayıtlı, tek `/health` endpoint'i.
4. `Directory.Packages.props`, `Directory.Build.props`, `.editorconfig`.
5. `deploy/docker/compose/docker-compose.dev.yml` — Postgres/Redis/NATS/Jaeger/Seq.
6. `tests/Architecture.Tests` — temel bağımlılık kuralları, boş Iam modülü üstünde test.
7. `legacy/` klasörüne eski `BaseModules/`, `BusinessModules/`, `Common/`, `Gateways/`, `Jobs/`, `Workers/` taşınır.
8. CLAUDE.md güncellenir (eski ArfBlocks bölümü `legacy/` için ayrılır, yeni pattern'ler eklenir).

**"devam et" dersen** `claude/refactor-remove-hardware-dependency-W9Hgh` branch'inde PR #1'i açarım. Yoksa hangi maddede fikrin farklıysa söyle, önce onu konuşalım.