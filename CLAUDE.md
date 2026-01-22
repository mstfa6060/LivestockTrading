# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Development Commands

```bash
# Build the solution
dotnet build LivestockTrading.sln

# Run tests
dotnet test LivestockTrading.sln

# Run database migrations (from Jobs/RelationalDB/MigrationJob directory)
dotnet run development

# Run specific API locally
dotnet run --project BaseModules/IAM/BaseModules.IAM.Api
dotnet run --project BusinessModules/LivestockTrading/LivestockTrading.Api
dotnet run --project BaseModules/FileProvider/BaseModules.FileProvider.Api
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
  ├── Definitions/    # Base entities, DbContext
  ├── Services/       # Auth, Caching, Logging, Messaging
  └── Contracts/      # Event/Queue contracts

Gateways/
  └── Api/            # Ocelot API Gateway (routes /iam/*, /fileprovider/*, /livestocktrading/*)

Jobs/
  ├── RelationalDB/MigrationJob/  # EF Core migrations
  └── SpecialPurpose/ResourceSeeder/  # Data seeding
```

### Request Handler Pattern (ArfBlocks Framework)

Each feature uses a consistent folder structure under `Application/RequestHandlers/{Feature}/{Commands|Queries}/{OperationName}/`:

- **Handler.cs** - Main request handler implementing `IRequestHandler`
- **Models.cs** - Request/Response DTOs
- **DataAccess.cs** - Database operations (DbContext usage)
- **Mapper.cs** - Entity ↔ DTO mapping
- **Validator.cs** - FluentValidation rules
- **Verificator.cs** - Authorization checks

Example path: `BusinessModules/LivestockTrading/LivestockTrading.Application/RequestHandlers/Products/Commands/Create/Handler.cs`

### Key Technologies

- **Arfware.ArfBlocks.Core** - Custom CQRS-like framework (replaces MediatR/traditional controllers)
- **Ocelot** - API Gateway routing
- **Entity Framework Core 8.0** - ORM with SQL Server
- **Redis** - Distributed cache (L2), with in-memory L1 cache
- **RabbitMQ** - Message queue for async operations
- **FluentValidation** - Request validation
- **Serilog** - Structured logging

### Authentication

- JWT-based authentication with refresh tokens
- Multi-provider support: Native (email/password), Google OAuth, Apple Sign-In
- User management in IAM module, shared across all modules
- Roles stored per module in `UserRole` entity

### API Routing

The Ocelot gateway (`Gateways/Api/ocelot.json`) routes requests:
- `/iam/*` → IAM API
- `/fileprovider/*` → FileProvider API
- `/livestocktrading/*` → LivestockTrading API

No traditional controllers - ArfBlocks middleware discovers handlers based on request path.

### Caching Strategy

Two-tier caching via `Common.Services.Caching`:
1. L1 (Memory): Fast, in-process, short TTL (~5 min)
2. L2 (Redis): Distributed, longer TTL (configurable)

Use `ICacheService` with `CacheKeys` class for standardized key management.

### Workers

Background services for async processing:
- `Workers.MailSender` - Email dispatch
- `Workers.SmsSender` - SMS dispatch
- `Workers.NotificationSender` - Push notifications
- `Jobs.BackgroundJobs.HangfireScheduler` - Scheduled tasks

### Configuration

- `appsettings.json` / `appsettings.Development.json` - Standard config
- `appsettings.local.json` - Local overrides (gitignored)
- Environment variables for Docker deployment (see `_devops/docker/env/.env.example`)
