---
paths:
  - "**/Program.cs"
  - "**/*DbContext*"
  - "**/ApplicationDependencyProvider*"
  - "**/Worker*"
  - "**/appsettings*"
  - "**/MigrationJob/**"
---
# Program.cs Middleware Order

```csharp
builder.AddSerilogLogging("ModuleName.Api");
builder.Services.AddMadenCaching(builder.Configuration);
builder.Services.AddArfBlocks(options => { ... });

app.UseCorrelationId();
app.UseSerilogRequestLogging();
app.UseArfBlocksRequestHandlers(); // Must be last
```

# DbContext Pattern (Multi-Module)

```
DefinitionDbContext (Common.Definitions.Infrastructure)  <- Base: User, Role, Module, Country, AuditLog
    ├── IamDbContext (extends DefinitionDbContext)       <- AppRefreshToken, etc.
    └── LivestockTradingModuleDbContext                  <- Business entities
```

- `ApplicationDbContext` in MigrationJob aggregates all module contexts for migration generation
- `CommonModelBuilder.Build(ModelBuilder)` configures shared entity relationships
- `LivestockTradingModelBuilder.Build(ModelBuilder)` configures business entity relationships
- Country entity is managed by `DefinitionDbContext` - use `modelBuilder.Ignore<Country>()` in module builders

# Dependency Injection (ApplicationDependencyProvider)

Each module has `ApplicationDependencyProvider` extending `ArfBlocksDependencyProvider`:

```csharp
// Located at {Module}.Application/Configuration/ApplicationDependencyProvider.cs
// Registers: DbContext, IJwtService, ICacheService, AuthorizationService, RabbitMqPublisher
```

# Caching Strategy

Two-tier caching via `Common.Services.Caching`:
1. L1 (Memory): Fast, in-process, short TTL (~5 min)
2. L2 (Redis): Distributed, longer TTL (configurable)

```csharp
await _cacheService.GetOrCreateAsync(cacheKey, factoryFunc, timespan);
await _cacheService.RemoveByPatternAsync("pattern:*");
```

Use `ICacheService` with `CacheKeys` class for standardized key management.

# Workers (Background Services)

IHostedService pattern consuming RabbitMQ messages:
- `BaseModules.IAM.Workers.MailSender` - Sends email via Brevo SMTP
- `BaseModules.IAM.Workers.SmsSender` - Sends SMS via NetGSM
- `LivestockTrading.Workers.NotificationSender` - Push notifications & real-time events

Workers consume from RabbitMQ exchanges (e.g., `iam.notification.sms`, `iam.notification.email`, `livestocktrading.notification.push`).

# Configuration

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

# Seed Data

- Country data: `Jobs/RelationalDB/MigrationJob/SeedData/countries.json` (196 countries with currency info)
- `CountrySeeder` handles insert (new DB) or update (existing data with `--force-country-reseed`)
- Seed runs automatically after migrations in `Program.cs`
