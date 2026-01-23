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
cp env/.env.example .env.dev
docker compose -f docker-compose.yml -f docker-compose.dev.yml --env-file .env.dev up -d
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
```

### Request Handler Pattern (ArfBlocks Framework)

Each feature uses a consistent 6-file folder structure under `Application/RequestHandlers/{Feature}/{Commands|Queries}/{OperationName}/`:

- **Handler.cs** - Implements `IRequestHandler`, receives `IRequestModel payload`, returns `ArfBlocksRequestResult`
- **Models.cs** - `RequestModel` (IRequestModel) and `ResponseModel` (IResponseModel) with nested DTOs
- **DataAccess.cs** - Implements `IDataAccess`, receives `ArfBlocksDependencyProvider` to resolve DbContext
- **Mapper.cs** - Entity to response DTO mapping
- **Validator.cs** - FluentValidation rules for request validation
- **Verificator.cs** - Authorization checks via `AuthorizationService`

Handler code pattern:
```csharp
public class Handler : IRequestHandler
{
    public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken ct)
    {
        var request = (RequestModel)payload;
        var dataAccess = new DataAccess(context.DependencyProvider);
        // ... business logic ...
        return ArfBlocksResults.Success(responseModel);
    }
}
```

DataAccess pattern:
```csharp
public class DataAccess : IDataAccess
{
    private readonly ModuleDbContext _dbContext;
    public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
    {
        _dbContext = dependencyProvider.GetInstance<ModuleDbContext>();
    }
}
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
- LivestockTrading tables are prefixed with `LivestockTrading_` to avoid collisions

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

### API Routing

The Ocelot gateway (`Gateways/Api/ocelot.json`) routes requests:
- `/iam/*` → IAM API (port 5000 locally)
- `/fileprovider/*` → FileProvider API
- `/livestocktrading/*` → LivestockTrading API

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
