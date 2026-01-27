# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Development Commands

```bash
# Build the solution
dotnet build LivestockTrading.sln

# Run tests
dotnet test LivestockTrading.sln

# Run specific API locally
dotnet run --project BaseModules/IAM/BaseModules.IAM.Api
dotnet run --project BusinessModules/LivestockTrading/LivestockTrading.Api
dotnet run --project BaseModules/FileProvider/BaseModules.FileProvider.Api
```

### Database Migrations

```bash
# Run migrations (from Jobs/RelationalDB/MigrationJob directory)
cd Jobs/RelationalDB/MigrationJob
dotnet run development

# Force re-seed country data (updates existing records without deleting)
dotnet run development --force-country-reseed

# Add a new migration
dotnet ef migrations add <MigrationName>
```

### Docker Local Development
```bash
cd _devops/docker/compose
cp ../env/.env.example .env.dev
# Edit .env.dev with your local settings

# Start all services
docker compose -f docker-compose.yml -f docker-compose.dev.yml --env-file .env.dev up -d

# View logs
docker compose -p livestock_dev logs -f api-gateway

# Restart specific service
docker compose -p livestock_dev restart iam-api
```

## Architecture Overview

This is a **modular monolith** .NET 8.0 backend for an agricultural livestock trading platform using Clean Architecture principles.

### Module Structure

```
BaseModules/           # Platform infrastructure
  ├── IAM/            # Identity & Access Management (auth, users, roles)
  └── FileProvider/   # File storage service (MinIO/S3)

BusinessModules/
  └── LivestockTrading/  # Core trading domain

Common/               # Shared code
  ├── Definitions/    # Base entities, DbContext, Seed data configurations
  ├── Services/       # Auth, Caching, Logging, Messaging, Environment, Notification
  ├── Contracts/      # Event/Queue/PubSub contracts
  ├── Connectors/     # External service connectors
  └── Helpers/        # Utility functions

Gateways/
  └── Api/            # Ocelot API Gateway (routes /iam/*, /fileprovider/*, /livestocktrading/*)

Jobs/
  ├── RelationalDB/MigrationJob/  # EF Core migrations & seed data
  └── BackgroundJobs/HangfireScheduler/  # Scheduled tasks

Workers/              # Background services (per module)
  ├── BaseModules.IAM.Workers/          # MailSender, SmsSender
  └── BusinessModules.LivestockTrading.Workers/  # MailSender, SmsSender, NotificationSender

_devops/              # CI/CD & Infrastructure
  ├── docker/         # Dockerfiles, compose files, env templates
  └── jenkins/        # Jenkinsfile.dev, Jenkinsfile.prod
```

### Request Handler Pattern (ArfBlocks Framework)

**KURAL: Her endpoint MUTLAKA 6 dosya/class icerir. Eksik dosya OLAMAZ.**

`Application/RequestHandlers/{Feature}/{Commands|Queries}/{OperationName}/` altinda:

```
Handler.cs       → Is mantigi (IRequestHandler)
Models.cs        → Request/Response DTO'lari
DataAccess.cs    → Veritabani islemleri (IDataAccess)
Mapper.cs        → Entity ↔ DTO donusumleri
Validator.cs     → FluentValidation + DbValidationService (IRequestValidator)
Verificator.cs   → Yetkilendirme + DbVerificationService (IRequestVerificator)
```

Detaylar:
- **Handler.cs** - Implements `IRequestHandler`, constructor takes `(ArfBlocksDependencyProvider, object dataAccess)`
- **Models.cs** - `RequestModel` (IRequestModel) and `ResponseModel` (IResponseModel or IResponseModel<Array>)
- **DataAccess.cs** - Implements `IDataAccess`, receives `ArfBlocksDependencyProvider` to resolve DbContext
- **Mapper.cs** - Entity ↔ DTO mapping (enum ↔ int cast'leri burada yapilir)
- **Validator.cs** - FluentValidation (ValidateRequestModel) + domain validation (ValidateDomain) using `LivestockTradingModuleDbValidationService`
- **Verificator.cs** - Authorization (VerificateActor) + domain verification (VerificateDomain) using `LivestockTradingModuleDbVerificationService`

> Yeni bir endpoint olusturulurken bu 6 dosyanin hepsi olusturulmalidir. Hicbiri atlanamaz.

### Standard CRUD Endpoint Patterns

Her entity icin asagidaki 6 endpoint olusturulmalidir (Create, Update, Delete, All, Detail, Pick):

#### Handler Constructor (tum handler'larda ayni):
```csharp
public class Handler : IRequestHandler
{
    private readonly DataAccess _dataAccessLayer;

    public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
    {
        _dataAccessLayer = (DataAccess)dataAccess;
    }
}
```

#### 1. ALL (Queries/All) - Listeleme (Sayfalama, Siralama, Filtreleme)
```csharp
// Models.cs
public class RequestModel : IRequestModel
{
    public XSorting Sorting { get; set; }
    public List<XFilterItem> Filters { get; set; }
    public XPageRequest PageRequest { get; set; }
}

public class ResponseModel : IResponseModel<Array>
{
    public Guid Id { get; set; }
    // ... entity alanlari ...
    public DateTime CreatedAt { get; set; }
}

// Handler.cs - return ile page bilgisi dondurulur
var (items, page) = await _dataAccessLayer.All(req.Sorting, req.Filters, req.PageRequest, cancellationToken);
var response = mapper.MapToResponse(items);
return ArfBlocksResults.Success(response, page);

// DataAccess.cs - Sort, Filter, Paginate kullanilir
public async Task<(List<Entity> Items, XPageResponse Page)> All(
    XSorting sorting, List<XFilterItem> filters, XPageRequest pageRequest, CancellationToken ct)
{
    var query = _dbContext.Entities
        .AsNoTracking()
        .Where(e => !e.IsDeleted)
        .Sort(sorting)
        .Filter(filters);

    if (sorting == null)
        query = query.OrderByDescending(e => e.CreatedAt);

    var page = query.GetPage(pageRequest);
    var items = await query.Paginate(page).ToListAsync(ct);
    return (items, page);
}

// Mapper.cs - List<ResponseModel> dondurur
public List<ResponseModel> MapToResponse(List<Entity> items) { ... }
```

#### 2. DETAIL (Queries/Detail) - Tekil Kayit
```csharp
// Models.cs
public class RequestModel : IRequestModel
{
    public Guid Id { get; set; }
}

// Handler.cs
var entity = await _dataAccessLayer.GetById(req.Id, cancellationToken);
if (entity == null)
    throw new ArfBlocksValidationException(
        ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));
return ArfBlocksResults.Success(mapper.MapToResponse(entity));

// DataAccess.cs - AsNoTracking ile
public async Task<Entity> GetById(Guid id, CancellationToken ct)
{
    return await _dbContext.Entities
        .AsNoTracking()
        .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, ct);
}
```

#### 3. PICK (Queries/Pick) - Dropdown/Select Listesi
```csharp
// Models.cs
public class RequestModel : IRequestModel
{
    public List<Guid> SelectedIds { get; set; }
    public string Keyword { get; set; }
    public int Limit { get; set; } = 10;
}

public class ResponseModel : IResponseModel<Array>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    // minimal alanlar
}

// DataAccess.cs - Oncelik: SelectedIds > Keyword > default
public async Task<List<Entity>> Pick(List<Guid> selectedIds, string keyword, int limit, CancellationToken ct)
{
    var query = _dbContext.Entities.AsNoTracking().Where(e => !e.IsDeleted && e.IsActive);

    if (selectedIds != null && selectedIds.Any())
        return await query.Where(e => selectedIds.Contains(e.Id))
            .OrderByDescending(e => e.CreatedAt).ToListAsync(ct);

    if (!string.IsNullOrWhiteSpace(keyword))
    {
        var lowerKeyword = keyword.ToLower();
        query = query.Where(e => e.Name.ToLower().Contains(lowerKeyword));
    }

    return await query.OrderByDescending(e => e.CreatedAt)
        .Take(limit > 0 ? limit : 10).ToListAsync(ct);
}
```

#### 4. CREATE (Commands/Create)
```csharp
// Models.cs - RequestModel: entity alanlari, ResponseModel: olusturulan entity bilgileri
// Handler.cs
var entity = mapper.MapToEntity(request);
await _dataAccessLayer.Add(entity);
return ArfBlocksResults.Success(mapper.MapToResponse(entity));
```

#### 5. UPDATE (Commands/Update)
```csharp
// Models.cs - RequestModel: Guid Id + guncellenecek alanlar
// Handler.cs
var entity = await _dataAccessLayer.GetById(request.Id);
if (entity == null) throw new ArfBlocksValidationException(
    ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));
mapper.MapToEntity(request, entity);
await _dataAccessLayer.SaveChanges();
return ArfBlocksResults.Success(mapper.MapToResponse(entity));
```

#### 6. DELETE (Commands/Delete) - Soft Delete
```csharp
// Models.cs - RequestModel: Guid Id, ResponseModel: bool Success
// Handler.cs
var entity = await _dataAccessLayer.GetById(request.Id);
if (entity == null) throw new ArfBlocksValidationException(
    ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));
entity.IsDeleted = true;
entity.DeletedAt = DateTime.UtcNow;
await _dataAccessLayer.SaveChanges();
return ArfBlocksResults.Success(new ResponseModel { Success = true });
```

### Verificator & Validator Pattern

Her endpoint'te Verificator.cs ve Validator.cs dosyalari bulunur:

#### Verificator.cs - Yetkilendirme ve varlik kontrolu
```csharp
using {Module}.Infrastructure.Services;

public class Verificator : IRequestVerificator
{
    private readonly AuthorizationService _authorizationService;
    private readonly LivestockTradingModuleDbVerificationService _dbVerification;

    public Verificator(ArfBlocksDependencyProvider dependencyProvider)
    {
        _authorizationService = dependencyProvider.GetInstance<AuthorizationService>();
        _dbVerification = dependencyProvider.GetInstance<LivestockTradingModuleDbVerificationService>();
    }

    public async Task VerificateActor(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
    {
        await _authorizationService
            .ForResource(typeof(Verificator).Namespace)
            .VerifyActor()
            .Assert();
    }

    public async Task VerificateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
    {
        var request = (RequestModel)payload;
        // Varlik kontrolu (entity mevcut mu?)
        await _dbVerification.ValidateCategoryExists(request.Id, cancellationToken);
    }
}
```

- **Commands (Create/Update/Delete)**: VerificateDomain icinde entity varlik kontrolu yapilir
- **Queries (All/Detail/Pick)**: VerificateDomain genellikle `await Task.CompletedTask;` olarak birakilir

#### Validator.cs - Is kurallari ve FluentValidation
```csharp
using FluentValidation;
using {Module}.Domain.Errors;
using {Module}.Infrastructure.Services;
using Common.Services.ErrorCodeGenerator;

public class Validator : IRequestValidator
{
    private readonly LivestockTradingModuleDbValidationService _dbValidator;

    public Validator(ArfBlocksDependencyProvider dependencyProvider)
    {
        _dbValidator = dependencyProvider.GetInstance<LivestockTradingModuleDbValidationService>();
    }

    public void ValidateRequestModel(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
    {
        var request = (RequestModel)payload;
        var result = new RequestModel_Validator().Validate(request);
        if (!result.IsValid)
            throw new ArfBlocksValidationException(result.ToString("~"));
    }

    public async Task ValidateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
    {
        var request = (RequestModel)payload;
        await _dbValidator.ValidateCategoryExists(request.Id, cancellationToken);
        await _dbValidator.ValidateCategorySlugUnique(request.Slug, request.Id, cancellationToken);
    }
}

public class RequestModel_Validator : AbstractValidator<RequestModel>
{
    public RequestModel_Validator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CategoryErrors.NameRequired));
    }
}
```

#### DbVerificationService vs DbValidationService

| | DbVerificationService | DbValidationService |
|---|---|---|
| Kullanim yeri | Verificator.VerificateDomain | Validator.ValidateDomain |
| Amac | Yetki + hizli varlik kontrolu | Is kurallari + state kontrolu |
| Constructor | `(ModuleDbContext dbContext)` | `(ArfBlocksDependencyProvider dp)` |
| Base class | `DefinitionDbValidationService` | `DefinitionDbValidationService` |
| Konum | `Infrastructure/Services/` | `Infrastructure/Services/` |

#### DomainErrors Pattern

**KRITIK KURAL: Tum error property isimleri BENZERSIZ olmalidir!**

ErrorCodeExporter, tum hata property'lerini tek bir TypeScript dosyasina export eder. Ayni isimli property'ler TypeScript'te "duplicate key" hatasina neden olur.

**Naming Convention:**
- Her property ismi entity prefix'i ile baslamalidir
- Format: `{EntityName}{ErrorType}` (ornek: `CategoryNameRequired`, `BrandSlugAlreadyExists`)
- Generic isimler KULLANILMAZ: ~~`NameRequired`~~, ~~`SlugRequired`~~, ~~`ProductRequired`~~

```csharp
// Domain/Errors/DomainErrors.cs
public class LivestockTradingDomainErrors
{
    public static class CommonErrors { ... }

    public static class CategoryErrors
    {
        public static string CategoryNotFound { get; set; } = "Kategori bulunamadi.";
        public static string CategorySlugAlreadyExists { get; set; } = "Bu slug zaten kullaniliyor.";  // DOGRU
        public static string CategoryNameRequired { get; set; } = "Kategori adi zorunludur.";         // DOGRU
        // public static string SlugAlreadyExists { get; set; } = "...";  // YANLIS - duplicate olur!
        // public static string NameRequired { get; set; } = "...";       // YANLIS - duplicate olur!
    }

    public static class BrandErrors
    {
        public static string BrandNotFound { get; set; } = "Marka bulunamadi.";
        public static string BrandSlugAlreadyExists { get; set; } = "Bu slug zaten kullaniliyor.";    // DOGRU
        public static string BrandNameRequired { get; set; } = "Marka adi zorunludur.";               // DOGRU
    }

    public static class ProductPriceErrors
    {
        public static string PriceNotFound { get; set; } = "Fiyat bilgisi bulunamadi.";
        public static string PriceProductRequired { get; set; } = "Urun zorunludur.";                 // DOGRU
        public static string PriceAmountRequired { get; set; } = "Fiyat zorunludur.";                 // DOGRU
    }
}
```

**Yeni hata eklerken kontrol listesi:**
1. Ayni isimde baska bir property var mi? (`Ctrl+F` ile ara)
2. Property ismi entity prefix'i ile basliyor mu?
3. Generic isim kullanilmamis mi? (`NameRequired`, `SlugRequired` gibi)

#### DI Kaydi (ApplicationDependencyProvider)
```csharp
// Services
base.Add<LivestockTradingModuleDbVerificationService>();
base.Add<LivestockTradingModuleDbValidationService>();
```

### Mapper.cs - Enum Type Casting Kurali

Entity'lerdeki enum property'leri, Models.cs'te `int` olarak tanimlanir. Mapper'da explicit cast yapilmalidir:

```csharp
// Entity → Response (enum → int)
Status = (int)entity.Status,

// Request → Entity (int → enum)
Status = (ConversationStatus)request.Status,
```

Enum'lar Entity dosyalarinda tanimlidir (`Domain/Entities/`), ayri bir Enums namespace'i YOKTUR:
- `ConversationStatus` → `Entities/Messaging.cs`
- `OfferStatus` → `Entities/Messaging.cs`
- `ProductStatus`, `ProductCondition` → `Entities/Product.cs`
- `SellerStatus` → `Entities/Seller.cs`
- `BannerPosition` → `Entities/Banner.cs`
- `TaxType` → `Entities/TaxRate.cs`
- `OrderStatus`, `PaymentStatus` → `Entities/Order.cs`

> **ONEMLI**: `using LivestockTrading.Domain.Enums;` seklinde bir namespace YOKTUR. Enum'lar `using LivestockTrading.Domain.Entities;` ile gelir.

### Entity Property Referansi

Bazi entity'lerde property isimleri beklenenden farkli olabilir:
- **Seller**: `BusinessName` (~~Name~~ degil)
- **Category**: `Name`, `Slug`, `ParentCategoryId`
- **Brand**: `Name`, `Slug`
- **Product**: `Title`, `Slug`, `SellerId`, `CategoryId`, `BrandId`

Navigation property'ler Mapper'da null-safe kullanilir:
```csharp
CategoryName = entity.Category?.Name,
BrandName = entity.Brand?.Name,
SellerName = entity.Seller?.BusinessName,
```

### Çok Ülkeli Filtreleme (Multi-Country Filtering)

Platform 50+ dil ve çok sayıda ülkeyi destekler. Kullanıcılar default ülkelerinin ilanlarını görür veya frontend'den başka bir ülke seçebilir.

**Temel Kurallar:**
1. Ürünler `Location.CountryCode` (ISO 3166-1 alpha-2) üzerinden ülkeye bağlıdır
2. Frontend `countryCode` parametresi gönderirse sadece o ülkenin ürünleri döner
3. `countryCode` boş ise tüm ürünler döner (global görünüm)

**Listeleme Endpoint'lerinde Kullanım:**
```csharp
// Models.cs
public class RequestModel : IRequestModel
{
    /// <summary>Ülke kodu filtresi (ISO 3166-1 alpha-2, örn: "TR", "US", "DE")</summary>
    public string CountryCode { get; set; }
    public XSorting Sorting { get; set; }
    public List<XFilterItem> Filters { get; set; }
    public XPageRequest PageRequest { get; set; }
}

// DataAccess.cs - Include ile Location yüklenir
var query = _dbContext.Products
    .AsNoTracking()
    .Include(p => p.Location)
    .Where(p => !p.IsDeleted);

// Ülke filtresi uygulanır
if (!string.IsNullOrWhiteSpace(countryCode))
{
    query = query.Where(p => p.Location != null && p.Location.CountryCode == countryCode);
}
```

**ResponseModel'de Ülke Bilgisi:**
```csharp
public class ResponseModel : IResponseModel<Array>
{
    // ... diğer alanlar ...
    public Guid LocationId { get; set; }
    public string LocationCountryCode { get; set; }
    public string LocationCity { get; set; }
}

// Mapper'da
LocationCountryCode = entity.Location?.CountryCode,
LocationCity = entity.Location?.City,
```

**Kullanıcı Ülke Tercihi:**
- `User.CountryId`: Kullanıcının varsayılan ülkesi
- `User.LastViewingCountryId`: Son görüntülediği ülke (frontend güncelleyebilir)
- Frontend ülke değiştirdiğinde `countryCode` parametresini request body'de gönderir

### .http Test Dosyasi Kurali

Her entity icin CRUD endpoint'leri gelistirildiginde, `_doc/Http/BusinessModules/LivestockTrading/{EntityName}.http` dosyasi olusturulmalidir. Ornek: `_doc/Http/BusinessModules/LivestockTrading/Categories.http`

```http
@BaseUrl = http://localhost:5221

### Create
POST {{BaseUrl}}/{Entity}/Create

### All (sayfalama, siralama)
POST {{BaseUrl}}/{Entity}/All

### Detail
POST {{BaseUrl}}/{Entity}/Detail
{ "id": "..." }

### Pick (dropdown icin)
POST {{BaseUrl}}/{Entity}/Pick
{ "keyword": "...", "limit": 10 }

### Update
POST {{BaseUrl}}/{Entity}/Update
{ "id": "...", ... }

### Delete
POST {{BaseUrl}}/{Entity}/Delete
{ "id": "..." }
```

### Handler Registration & Discovery

Handlers are auto-discovered by ArfBlocks based on namespace convention. No manual route registration needed.

```csharp
// In Program.cs
builder.Services.AddArfBlocks(options => {
    options.ApplicationProjectNamespace = "BaseModules.IAM.Application";
});

// Middleware (must be last)
app.UseArfBlocksRequestHandlers();
```

ArfBlocks maps HTTP paths to handlers: `POST /Auth/Login` → `RequestHandlers/Auth/Commands/Login/Handler.cs`

### Program.cs Middleware Order

```csharp
builder.AddSerilogLogging("ModuleName.Api");
builder.Services.AddMadenCaching(builder.Configuration);
builder.Services.AddArfBlocks(options => { ... });

app.UseCorrelationId();
app.UseSerilogRequestLogging();
app.UseArfBlocksRequestHandlers(); // Must be last
```

### DbContext Pattern (Multi-Module)

```
DefinitionDbContext (Common.Definitions.Infrastructure)  ← Base: User, Role, Module, Country, AuditLog
    ├── IamDbContext (extends DefinitionDbContext)       ← AppRefreshToken, etc.
    └── LivestockTradingModuleDbContext                  ← Business entities
```

- `ApplicationDbContext` in MigrationJob aggregates all module contexts for migration generation
- `CommonModelBuilder.Build(ModelBuilder)` configures shared entity relationships
- `LivestockTradingModelBuilder.Build(ModelBuilder)` configures business entity relationships
- Country entity is managed by `DefinitionDbContext` - use `modelBuilder.Ignore<Country>()` in module builders

### Dependency Injection (ApplicationDependencyProvider)

Each module has `ApplicationDependencyProvider` extending `ArfBlocksDependencyProvider`:

```csharp
// Located at {Module}.Application/Configuration/ApplicationDependencyProvider.cs
// Registers: DbContext, IJwtService, ICacheService, AuthorizationService, RabbitMqPublisher
```

### Key Technologies

- **Arfware.ArfBlocks.Core** - Custom CQRS-like framework (replaces MediatR/traditional controllers)
- **Ocelot** - API Gateway routing
- **Entity Framework Core 8.0** - ORM with SQL Server
- **Redis** - Distributed cache (L2), with in-memory L1 cache
- **RabbitMQ** - Message queue for async operations (workers consume via exchanges)
- **FluentValidation** - Request validation
- **Serilog** - Structured logging
- **NetTopologySuite** - Spatial data support

### Authentication & JWT

- JWT-based with refresh tokens stored in `AppRefreshTokens` table
- Multi-provider: Native (email/password), Google OAuth, Apple Sign-In
- Roles stored per module in `UserRole` entity with `ModuleId`
- JWT role claims format: `"ModuleName.RoleName"` (e.g., `"LivestockTrading.Seller"`)
- Platform tracking: Web=0, Android=1, iOS=2

### Error Handling

```csharp
// Throw validation errors
throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.InvalidCredentials));

// Return success
return ArfBlocksResults.Success(responseModel);
```

### API Gateway & Routing

**Production/Dev Architecture:**
```
Internet → Nginx (443) → API Gateway (Ocelot) → Backend APIs
                              ↓
                    ┌─────────┼─────────┐
                    ↓         ↓         ↓
               IAM API   FileProvider  LivestockTrading
```

**Ocelot Gateway** (`Gateways/Api/Gateways.ApiGateway.Api/ocelot.json`):
- Routes requests based on URL prefix
- Handles JWT authentication centrally
- Public endpoints (no auth): `/iam/Auth/*`, `/iam/Users/Create`, `/iam/Countries/All`
- Protected endpoints: All others require valid JWT

**Route Mappings:**
- `/iam/*` → `iam-api-container:8080`
- `/fileprovider/*` → `fileprovider-api-container:8080`
- `/livestocktrading/*` → `livestocktrading-api-container:8080`

**Port Configuration (Docker):**
| Service | Default Port | Container Port |
|---------|-------------|----------------|
| API Gateway | 5000 (GATEWAY_PORT) | 8080 |
| LivestockTrading API | 5001 (GLOBALLIVESTOCK_API_PORT) | 8080 |
| IAM API | 5002 (IAM_API_PORT) | 8080 |
| FileProvider API | 5003 (FILEPROVIDER_API_PORT) | 8080 |

No traditional controllers - ArfBlocks middleware discovers handlers based on request path.

### Caching Strategy

Two-tier caching via `Common.Services.Caching`:
1. L1 (Memory): Fast, in-process, short TTL (~5 min)
2. L2 (Redis): Distributed, longer TTL (configurable)

```csharp
await _cacheService.GetOrCreateAsync(cacheKey, factoryFunc, timespan);
await _cacheService.RemoveByPatternAsync("pattern:*");
```

Use `ICacheService` with `CacheKeys` class for standardized key management.

### Workers (Background Services)

IHostedService pattern consuming RabbitMQ messages:
- `BaseModules.IAM.Workers.MailSender` - Sends email via Brevo SMTP
- `BaseModules.IAM.Workers.SmsSender` - Sends SMS via NetGSM
- `LivestockTrading.Workers.NotificationSender` - Push notifications

Workers consume from RabbitMQ exchanges (e.g., `iam.notification.sms`, `iam.notification.email`).

### Configuration

- `appsettings.json` / `appsettings.Development.json` - Standard config
- `appsettings.local.json` - Local overrides (gitignored)
- Environment variables for Docker deployment (see `_devops/docker/env/.env.example`)

Key config sections:
```json
{
  "ProjectConfigurations": {
    "RelationalDbConfiguration": { "SqlConnectionString": "..." },
    "EnvironmentConfiguration": { "EnvironmentName": "...", "ApiUrl": "..." },
    "ExternalAuth": { "Google": {...}, "Apple": {...} }
  },
  "Caching": {
    "Redis": { "ConnectionString": "...", "InstanceName": "globallivestock:" },
    "Memory": { "SizeLimitMB": 1024 }
  }
}
```

### Seed Data

- Country data: `Jobs/RelationalDB/MigrationJob/SeedData/countries.json` (196 countries with currency info)
- `CountrySeeder` handles insert (new DB) or update (existing data with `--force-country-reseed`)
- Seed runs automatically after migrations in `Program.cs`

## DevOps & Deployment

### Docker Structure

```
_devops/
├── docker/
│   ├── compose/
│   │   ├── docker-compose.yml          # Base services
│   │   ├── docker-compose.dev.yml      # Dev overrides
│   │   └── docker-compose.prod.yml     # Prod overrides (resource limits)
│   ├── env/
│   │   └── .env.example                # Environment template
│   ├── Dockerfile.api-gateway
│   ├── Dockerfile.livestocktrading-api
│   ├── Dockerfile.iam-api
│   ├── Dockerfile.fileprovider-api
│   ├── Dockerfile.iam-mail-worker
│   ├── Dockerfile.iam-sms-worker
│   └── Dockerfile.resource-seeder
└── jenkins/
    ├── Jenkinsfile.dev                 # Dev pipeline (dev branch)
    └── Jenkinsfile.prod                # Prod pipeline (main branch)
```

### Docker Compose Usage

```bash
# Development
cd _devops/docker/compose
docker compose -f docker-compose.yml -f docker-compose.dev.yml --env-file .env.dev up -d

# Production
docker compose -f docker-compose.yml -f docker-compose.prod.yml --env-file .env.prod up -d
```

### Jenkins CI/CD

**Dev Pipeline (`Jenkinsfile.dev`):**
- Triggers on `dev` branch
- Builds all images with `dev-latest` tag
- Deploys to dev server
- Runs migrations/seeders automatically

**Prod Pipeline (`Jenkinsfile.prod`):**
- Triggers on `main` branch
- Builds with `latest` + immutable tag (`prod-{BUILD_ID}-{COMMIT}`)
- Deploys to production server
- Migrations require explicit parameter

**Build Modules:**
```
api-gateway, livestocktrading-api, iam-api, fileprovider-api,
iam-mail-worker, iam-sms-worker, resource-seeder
```

### Server Paths

```
/opt/livestocktrading/
├── repo/                    # Git repository
├── .env.dev                 # Dev environment variables
└── .env.prod                # Prod environment variables
```

### Adding New Route to Gateway

1. Edit `Gateways/Api/Gateways.ApiGateway.Api/ocelot.json`
2. Add public route BEFORE catch-all routes:
```json
{
    "DownstreamPathTemplate": "/NewEndpoint/Action",
    "DownstreamScheme": "http",
    "DownstreamHostAndPorts": [{ "Host": "iam-api-container", "Port": 8080 }],
    "UpstreamPathTemplate": "/iam/NewEndpoint/Action",
    "UpstreamHttpMethod": ["POST"]
}
```
3. For authenticated endpoints, add `AuthenticationOptions` with the secret key
4. Rebuild and deploy API Gateway

## Frontend Entegrasyonu

Frontend projesi (`D:\Projects\GlobalLivestock\web`) backend API'lerini `arf-cli` ile olusturulan TypeScript client uzerinden tuketir.

### Frontend API Dokumantasyonu

Frontend gelisitiriciler icin API kullanim dokumani: `D:\Projects\GlobalLivestock\web\common\API-INTEGRATION.md`

**Bu dokuman asagidaki konulari kapsar:**
- Kimlik dogrulama (Login, Register, Logout, Token yonetimi)
- JWT ve Refresh Token akislari
- API cagirma kaliplari (All, Detail, Pick, Create, Update, Delete)
- Hata yonetimi
- Dosya yukleme
- React ornekleri (AuthContext, ProtectedRoute)

### Frontend'i Etkileyen Degisiklikler

Asagidaki degisiklikler yapildiginda `API-INTEGRATION.md` dosyasi MUTLAKA guncellenmelidir:

| Degisiklik Tipi | Guncellenmesi Gereken Bolum |
|-----------------|----------------------------|
| Yeni Auth endpoint (OTP, sosyal login, vb.) | Kimlik Dogrulama bolumu |
| JWT claim yapisi degisikligi | Token Yonetimi bolumu |
| Token suresi degisikligi | Token Yonetimi bolumu |
| Yeni public endpoint eklenmesi | Public/Protected Endpoint listesi |
| Response model formati degisikligi | Ilgili API kullanim ornekleri |
| Yeni hata kodlari | Hata Yonetimi bolumu |
| Dosya yukleme API degisikligi | Dosya Yukleme bolumu |
| Platform enum degisikligi | Platform Degerleri bolumu |

### arf-cli ile API Client Guncelleme

Backend'de endpoint degisikligi yapildiginda frontend API client'i guncellenmelidir:

```bash
# arf-cli ile TypeScript client olustur
arf-cli generate --output D:\Projects\GlobalLivestock\web\common\livestock-api
```

Bu komut asagidaki dosyalari gunceller:
- `src/api/base_modules/iam/index.ts`
- `src/api/base_modules/FileProvider/index.ts`
- `src/api/business_modules/livestocktrading/index.ts`
- `src/errors/locales/modules/backend/*/tr.ts`
