# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Current Priority: TASKS.md (Birlesik Gorev Listesi)

When starting a new chat without a specific task, refer to `../TASKS.md` in the parent directory (`d:\Projects\GlobalLivestock\TASKS.md`). This is the **unified task list** across all three projects (web, mobil, backend). Work through the unchecked items in priority order.

Additional project-specific reference:
- `CHANGELOG.md` — All features, fixes, and improvements log (58 commits in revenue strategy)

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

### Key Technologies

- **Arfware.ArfBlocks.Core** - Custom CQRS-like framework (replaces MediatR/traditional controllers)
- **Ocelot** - API Gateway routing
- **Entity Framework Core 8.0** - ORM with SQL Server
- **Redis** - Distributed cache (L2), with in-memory L1 cache
- **RabbitMQ** - Message queue for async operations (workers consume via exchanges)
- **FluentValidation** - Request validation
- **Serilog** - Structured logging
- **NetTopologySuite** - Spatial data support

### DomainErrors Pattern

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
}
```

**Yeni hata eklerken kontrol listesi:**
1. Ayni isimde baska bir property var mi? (`Ctrl+F` ile ara)
2. Property ismi entity prefix'i ile basliyor mu?
3. Generic isim kullanilmamis mi? (`NameRequired`, `SlugRequired` gibi)

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

### Error Handling

```csharp
// Throw validation errors
throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.InvalidCredentials));

// Return success
return ArfBlocksResults.Success(responseModel);
```

No traditional controllers - ArfBlocks middleware discovers handlers based on request path.

## Frontend Entegrasyonu

Frontend projesi (`D:\Projects\GlobalLivestock\web`) backend API'lerini `arf-cli` ile olusturulan TypeScript client uzerinden tuketir.

## Mobil Uygulama

Mobil uygulama projesi: `D:\Projects\GlobalLivestock\mobil`

### Frontend API Dokumantasyonu

Frontend gelisitiriciler icin API kullanim dokumani: `D:\Projects\GlobalLivestock\web\common\API-INTEGRATION.md`

**Bu dokuman asagidaki konulari kapsar:**
- Kimlik dogrulama (Login, Register, Logout, Token yonetimi)
- JWT ve Refresh Token akislari
- API cagirma kaliplari (All, Detail, Pick, Create, Update, Delete)
- Hata yonetimi
- Dosya yukleme
- Real-Time Mesajlasma (SignalR Hub baglantisi, event dinleme, typing indicator)
- React ornekleri (AuthContext, ProtectedRoute, useChat hook)

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
| SignalR Hub event degisikligi | Real-Time Mesajlasma bolumu |
| Yeni messaging endpoint'i | Real-Time Mesajlasma bolumu |
| Domain event yapisi degisikligi | Real-Time Mesajlasma bolumu |

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

### Frontend'den Gelen Talepler

Frontend ekibinden gelen backend gelistirme talepleri asagidaki dosyada dokumante edilir:

**Dosya:** `D:\Projects\GlobalLivestock\web\common\API-INTEGRATION.md`

Bu dosyanin "Backend Gelistirme Talimatlari" bolumunde:
- Kritik buglar ve cozum onerileri
- Yeni endpoint talepleri
- Mevcut endpoint iyilestirme onerileri
- Oncelik tablosu

**"Frontend'den gelen taleplere bak"** denildiginde bu dosya incelenmelidir.
