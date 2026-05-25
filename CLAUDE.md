# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Test Commands

```bash
# Restore, build, format-check
dotnet restore LivestockTrading.slnx
dotnet build LivestockTrading.slnx --no-restore -c Release
dotnet format LivestockTrading.slnx --verify-no-changes --no-restore

# Run all tests (only Catalog.Application.Tests exists today)
dotnet test LivestockTrading.slnx

# Run a single test project
dotnet test src/Modules/Catalog/Catalog.Application.Tests/Catalog.Application.Tests.csproj

# Run a single test by name filter
dotnet test src/Modules/Catalog/Catalog.Application.Tests/ --filter "FullyQualifiedName~CreateCategoryHandlerTests"

# Run the API host (Catalog module only until Wave 4)
dotnet run --project src/LivestockTrading.Api

# Database migrations & seeding (Wave 1+)
dotnet run --project Tools/SeedRunner -- migrate
dotnet run --project Tools/SeedRunner -- seed --scope=all
```

`TreatWarningsAsErrors=true` is set globally — zero warnings allowed.

## Architecture Overview

**Modular monolith** on .NET 10, PostgreSQL 17, Redis, RabbitMQ, MinIO.  
Solution file: `LivestockTrading.slnx`

### Solution Layout

```
src/
  LivestockTrading.Api/          # Single host — wires all 10 modules via DI
  Modules/
    Accounts / Admin / Carrier / Catalog / Identity /
    Listings / Marketplace / Messaging / Notifications / Subscription
  Shared/
    LivestockTrading.Shared.Kernel        # Base types: Entity, AggregateRoot, Result<T>, Error, DomainEvent
    LivestockTrading.Shared.Contracts     # Cross-module DTOs and interface contracts
    LivestockTrading.Shared.Infrastructure
Tools/
  SeedRunner        # Migration runner + seed data (executable)
  AdminBootstrap    # Admin user init
  OpenApiGen        # OpenAPI schema generation
_devops/            # Docker Compose, Jenkins Jenkinsfile.ci, DB init scripts
_docs/decisions/    # Architecture decision records (planning docs, Turkish)
```

Each module has exactly three projects: `{Module}.Domain`, `{Module}.Application`, `{Module}.Infrastructure`. Only `LivestockTrading.Api` references any `.Infrastructure` project (for DI registration).

### CQRS — MassTransit Mediator

Commands/queries are `IConsumer<TMessage>` implementations dispatched via `IScopedMediator`.  
Handlers respond with `Result<T>` or `Result` (non-generic for validation failures).

Endpoint → Handler call pattern (Catalog example):

```
MapXxxEndpoints() → XxxEndpoint.Handle()
  → mediator.CreateRequestClient<XxxCommand>()
  → client.GetResponse<Result<TValue>, Result>(command, ct)
  → XxxHandler : IConsumer<XxxCommand>  (responds via context.RespondAsync)
```

Two pipeline filters run automatically per handler via MassTransit:
- **ValidationFilter** — runs FluentValidation before the handler; responds `Result` on failure
- **UnitOfWorkFilter** — calls `SaveChangesAsync()` after the handler succeeds

### DDD Patterns (Shared.Kernel)

- `AggregateRoot` → `Entity`: AR collects domain events via `Raise(IDomainEvent)`. Infrastructure dispatches and clears them in `DomainEventDispatchInterceptor` (EF Core `ISaveChangesInterceptor`).
- Cross-AR references use **ID only** — no navigation properties across aggregate boundaries.
- `Result<T>` / `Result`: railway-oriented; `Error` carries `(code, message)`. Use `result.ToApiResult()` to map to HTTP responses.
- `DomainException` is the base for rule violations; handlers catch it and return `Failure`.

### Database

- **Schema-per-module**: each module's `DbContext` uses its own PostgreSQL schema (e.g., `catalog`). Migrations history table: `catalog.__EFMigrationsHistory`.
- Connection string key pattern: `ConnectionStrings:{Module}Db` (e.g., `CatalogDb`).
- Runtime user `livestock_app` (DML only); migrations run as `livestock_migrator`.
- `UseSnakeCaseNamingConvention()` + `UseNetTopologySuite()` applied to all DbContexts.
- JSONB columns for `Translations` value objects (multi-language support).

### Module DI Registration

Each module exposes one extension method in its `.Infrastructure` project:

```csharp
builder.Services.AddCatalogApplication();
builder.Services.AddCatalogInfrastructure(builder.Configuration);
```

Catalog infrastructure wires: `DomainEventDispatchInterceptor` → repositories → cache (`ICacheService`, Memory or Redis via `Catalog:CacheProvider` config) → `ICatalogReadService` decorated by `CachedCatalogReadService` (Scrutor `Decorate`) → 3-tier currency rate providers (TCMB → ECB → Fawazahmed0, each as typed `HttpClient` + `AddStandardResilienceHandler`) → Quartz `CurrencyRateUpdateJob` (cron `0 0 13 * * ?`, daily 13:00 UTC) → `ICurrencyRateRefresher`.

### Caching

Config key `Catalog:CacheProvider`: `"Memory"` (default, dev) or `"Redis"` (prod, requires `ConnectionStrings:Redis`). Implemented via `ICacheService` singleton; read service wrapped at DI registration time (Scrutor `Decorate`, Scoped lifetime preserved).

## Branch Strategy

Active rebuild uses `rebuild/v2` as the base. Feature work goes on `feature/wave-N-*` branches targeting `rebuild/v2`. **Do not touch `main`** during the rebuild period (main = old production code, invariant commit `44416138`).

All planning documents are in `c:\workspace\livestock-trading-planning\` (separate repo, 21 docs / ~13k lines). Implementation must not deviate from those documents without recording the deviation in `_docs/deviations.md`.

## Wave Rollout Status

Modules are implemented in waves. As of Wave 3:
- **Complete**: `Catalog.Domain`, `Catalog.Application` (154 tests), `Catalog.Infrastructure`, `LivestockTrading.Api` (Catalog-only host wire)
- **Wave 4+**: Identity, Accounts, and the remaining 8 modules

When adding code to a not-yet-implemented module, follow the existing Catalog layer structure as the canonical reference.
