# Technical Architecture Review - GlobalLivestock Backend

**Review Date:** 2026-02-11
**Codebase:** GlobalLivestock Backend (LivestockTrading.sln)
**Platform:** .NET 8.0 Modular Monolith
**Reviewer:** Technical Architecture Agent

---

## 1. Architecture Overview Assessment

### 1.1 Module Structure

The backend follows a **modular monolith** pattern with Clean Architecture principles. The solution contains:

| Layer | Projects | Purpose |
|-------|----------|---------|
| **BaseModules** | IAM (4 projects), FileProvider (4 projects) | Platform infrastructure |
| **BusinessModules** | LivestockTrading (4 projects: Api, Application, Domain, Infrastructure) | Core trading domain |
| **Common** | Definitions, Services (8), Contracts (3), Connectors, Helpers | Shared code |
| **Gateways** | API Gateway (Ocelot) | Request routing |
| **Jobs** | MigrationJob, HangfireScheduler | Background tasks |
| **Workers** | IAM.Workers (Mail, SMS), LivestockTrading.Workers (Notification) | Message consumers |

**Assessment:** The module separation is clean. Each business module follows a consistent 4-project structure (Api, Application, Domain, Infrastructure) which enforces dependency direction. The Common layer is well-factored into granular service packages.

**Concern:** The `LivestockTradingModuleDbContext` inherits from `DefinitionDbContext`, which means the business module has direct access to IAM tables (Users, Roles). This creates a coupling that would make future module extraction into separate services more complex.

### 1.2 ArfBlocks Framework (CQRS-like Pipeline)

The ArfBlocks framework provides a convention-based request handler pipeline:

```
HTTP Request -> ArfBlocks Middleware -> Verificator -> Validator -> Handler -> Response
```

Each endpoint consists of 6 files:
- `Handler.cs` - Business logic (IRequestHandler)
- `Models.cs` - Request/Response DTOs
- `DataAccess.cs` - Database operations (IDataAccess)
- `Mapper.cs` - Entity-DTO mapping
- `Validator.cs` - FluentValidation + domain validation
- `Verificator.cs` - Authorization + existence checks

**Strengths:**
- Convention-based discovery eliminates route registration boilerplate
- Consistent 6-file pattern makes endpoints predictable and auditable
- Clean separation of concerns: auth (Verificator) -> validation (Validator) -> logic (Handler)
- `AsNoTracking()` consistently used for read operations

**Weaknesses:**
- The framework is proprietary (Arfware), which creates vendor lock-in
- No visible unit test infrastructure -- the framework's DI model (`ArfBlocksDependencyProvider`) makes mocking difficult
- The `object dataAccess` parameter in Handler constructors loses type safety
- Handler constructors create `new Mapper()` instances directly, preventing mapper injection/testing

### 1.3 Endpoint Coverage

The LivestockTrading module contains approximately **140+ handler files** across 30+ entities. Major entity groups:

- **Core Commerce:** Products (All, Detail, DetailBySlug, Search, Approve, Reject, Create, Update, Delete), Categories, Brands
- **Users & Sellers:** Sellers (Create, Update, Verify, Suspend, All, Detail, Pick), Farms
- **Messaging:** Conversations, Messages, Offers
- **Transport:** Transporters, TransportRequests, TransportOffers, TransportTracking
- **Supporting:** Currencies, Languages, PaymentMethods, ShippingZones, ShippingRates, TaxRates
- **User Activity:** FavoriteProducts, ProductReviews, SellerReviews, TransporterReviews, Notifications, SearchHistory, ProductViewHistory, UserPreferences
- **Content:** Banners, FAQs
- **Trading:** Deals
- **Product Types:** AnimalInfos, ChemicalInfos, MachineryInfos, SeedInfos, FeedInfos, VeterinaryInfos, HealthRecords, Vaccinations
- **Dashboard:** Stats

---

## 2. Database & Data Access Layer

### 2.1 DbContext Hierarchy

```
DbContext (EF Core)
  -> DefinitionDbContext (Common) -- Users, Roles, Countries, Provinces, Districts, Neighborhoods
      -> IamDbContext (IAM) -- AppRefreshTokens
      -> LivestockTradingModuleDbContext (Business) -- All business entities (40+ DbSets)
```

The `LivestockTradingModuleDbContext` contains **40+ DbSet properties**, covering:
- 7 product-specific info tables (AnimalInfo, ChemicalInfo, MachineryInfo, SeedInfo, FeedInfo, VeterinaryInfo, HealthRecord)
- 3 messaging tables (Conversation, Message, Offer)
- 6 transport tables (Transporter, TransportRequest, TransportOffer, TransportTracking, TransporterReview)
- 8 helper/config tables (Currency, Language, PaymentMethod, ShippingCarrier, ShippingZone, ShippingRate, TaxRate, FAQ)
- Plus all remaining entities

**Concern: DbContext Size.** A single DbContext with 40+ DbSets will have a large model. EF Core's `OnModelCreating` compiles this model once and caches it, but the initial startup cost is non-trivial. For a modular monolith, consider using `IEntityTypeConfiguration<T>` classes to break up `LivestockTradingModelBuilder`.

### 2.2 Entity Design

**BaseEntity Pattern:**
```csharp
public abstract class BaseEntity
{
    public Guid Id { get; set; }       // Client-generated GUIDs
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } // Soft delete
    public DateTime? DeletedAt { get; set; }
}
```

**Observations:**
- **Soft Delete:** All entities use `IsDeleted` flag. Every query must include `.Where(e => !e.IsDeleted)`. This is error-prone -- a global query filter (`HasQueryFilter`) would be safer and eliminate repetition.
- **GUIDs as PKs:** Client-generated `Guid.NewGuid()` in constructors. This creates fragmented clustered indexes in SQL Server. Sequential GUIDs or `NEWSEQUENTIALID()` would improve insert performance.
- **No RowVersion/Concurrency Token:** No concurrency control is visible. For a trading platform where offers and deals involve multiple parties, optimistic concurrency (`[Timestamp]` / `rowversion`) is critical.
- **JSON Columns:** Multiple entities store JSON as `string` (Attributes, BusinessHours, FleetInfo, ServiceRegions, Specializations, NameTranslations). Consider using EF Core 8's `OwnsMany`/`OwnsOne` with JSON column support for type-safe querying.

### 2.3 Indexing Analysis

The `LivestockTradingModelBuilder` defines indexes:

| Entity | Indexes | Assessment |
|--------|---------|------------|
| Category | `Slug` (unique) | Good |
| Product | `Slug` (unique) | Good, but missing: `SellerId`, `CategoryId`, `Status`, `CreatedAt` |
| Brand | `Name` | Good |
| Seller | `UserId` | Good, but missing: `Status`, `IsVerified` |
| Location | None | **Missing:** `CountryCode` (used in country filtering), `UserId` |
| Conversation | `ParticipantUserId1`, `ParticipantUserId2` | Good, but missing: composite `(ParticipantUserId1, ParticipantUserId2)` |
| Message | `SenderUserId`, `RecipientUserId` | Missing: `ConversationId` + `SentAt` composite for chat pagination |
| Deal | `BuyerId`, `Status`, `DealNumber` (unique) | Good |
| TransportRequest | `BuyerId`, `Status`, `IsInPool` | Good |
| Transporter | `UserId` (unique), `IsVerified`, `IsActive` | Good |

**Critical Missing Indexes:**
1. `Product.Status` -- Product listing queries filter by status constantly
2. `Product.CategoryId` -- Category-based product listing
3. `Product.SellerId` -- Seller's product listing
4. `Product.CreatedAt` -- Default sort order for all listings
5. `Location.CountryCode` -- Multi-country filtering (used in every product listing)
6. `Message.ConversationId + SentAt` -- Chat message pagination
7. `FavoriteProduct.(UserId, ProductId)` -- Composite unique index to prevent duplicates

### 2.4 N+1 Query Risk

The Products/All `DataAccess.cs` includes `.Include(p => p.Location)` but does **not** include Category, Brand, or Seller. If the Mapper accesses `product.Category?.Name`, this triggers lazy loading (or returns null if lazy loading is disabled).

**Pattern found in DataAccess.cs:**
```csharp
var query = _dbContext.Products
    .AsNoTracking()
    .Include(p => p.Location)
    .Where(p => !p.IsDeleted);
```

**Missing Includes:** `Category`, `Brand`, `Seller` -- these are commonly needed in list views for display names.

### 2.5 Timestamp Management

The `DefinitionDbContext.AddTimestamps()` method correctly handles `CreatedAt`, `UpdatedAt`, and `DeletedAt` via `ChangeTracker` intercept. Uses `DateTime.UtcNow` consistently.

**Good:** Single UTC timestamp per SaveChanges call ensures consistency.
**Concern:** `RowNumber` property modification exclusion logic is fragile -- it relies on property name string matching.

---

## 3. Caching Strategy Analysis

### 3.1 Two-Tier Cache Architecture

```
L1: MemoryCache (in-process, 5-min TTL cap)
L2: Redis (distributed, configurable TTL)
```

**CacheService Implementation (`Common.Services.Caching/CacheService.cs`):**

**Strengths:**
- Graceful Redis degradation -- continues with memory cache if Redis is unavailable
- Pattern-based cache invalidation (`RemoveByPatternAsync`)
- Proper serialization with `ReferenceLoopHandling.Ignore`

**Critical Issues:**

1. **Cache Stampede Vulnerability:** `GetOrCreateAsync` has no locking. Under high load, if the cache expires, multiple concurrent requests will all hit the database simultaneously and all try to repopulate the cache. Solution: Use a distributed lock (e.g., `SemaphoreSlim` for L1 or Redis `SETNX` for distributed).

2. **L1/L2 Coherency Problem:** When one API instance invalidates L2 (Redis), other instances' L1 (memory) caches still hold stale data for up to 5 minutes. In a multi-instance deployment, this means data inconsistency. Solution: Use Redis Pub/Sub to notify other instances to invalidate L1.

3. **`server.Keys()` in Production:** `RemoveByPatternAsync` uses `server.Keys()` which executes `KEYS` on Redis. This is O(N) and **blocks the Redis server**. For production with large key spaces, this will cause latency spikes. Solution: Use `SCAN` via `server.Keys()` with `pageSize` parameter (which StackExchange.Redis does by default), but still avoid calling this frequently.

4. **No Cache-Aside Pattern for Entities:** No evidence of entity-level caching in handlers. Categories, Brands, and other slowly-changing reference data should be cached aggressively.

5. **Constructor-based Redis Connection:** `CacheService` creates its own `ConnectionMultiplexer` in the constructor rather than using DI-registered singleton. This could create multiple Redis connections per service instance.

### 3.2 Cache Key Management

No centralized `CacheKeys` class was found in the LivestockTrading module. The ChatHub uses inline key patterns (`"chat:online:{userId}"`, `"chat:connections:{userId}"`). Duplicated between `ChatHub` and `PresenceService`.

**Recommendation:** Create a shared `CacheKeys` static class to standardize key patterns and prevent typo-related bugs.

---

## 4. Messaging & Real-Time Communication

### 4.1 SignalR ChatHub

**File:** `LivestockTrading.Api/Hubs/ChatHub.cs`

**Architecture:**
- JWT-authenticated (`[Authorize]` attribute)
- Group-based messaging per conversation
- Redis backplane configured for multi-instance scaling
- Online presence tracked via Redis cache

**Strengths:**
- Multi-connection support per user (handles multiple tabs/devices)
- Redis backplane ensures SignalR scales across instances
- Graceful handling of user ID claim extraction with fallback (`userId` -> `sub`)

**Issues:**

1. **Broadcast Spam:** `SetUserOnline/SetUserOffline` calls `Clients.All.SendAsync("UserOnline"/"UserOffline")`. In a platform with thousands of users, every connect/disconnect broadcasts to ALL connected clients. This is extremely costly at scale. Solution: Only notify users who have that user in their contacts or active conversations.

2. **Race Condition in Presence:** `SetUserOffline` does a Read-Modify-Write on the connections HashSet without locking:
   ```csharp
   var connections = await _cacheService.GetAsync<HashSet<string>>(connectionsKey);
   connections.Remove(Context.ConnectionId);
   if (connections.Count == 0) { /* mark offline */ }
   else { await _cacheService.SetAsync(connectionsKey, connections, ...); }
   ```
   Two simultaneous disconnects could both read the set, each remove their connection, and both see `Count > 0`, leaving a stale connection entry. Solution: Use Redis `SREM` (set remove) atomic operation instead of serialize/deserialize a HashSet.

3. **Duplicate Presence Logic:** Both `ChatHub` and `PresenceService` implement the same presence tracking with the same Redis keys. This violates DRY and risks inconsistency. The ChatHub should delegate to `PresenceService`.

4. **No Conversation Membership Validation:** `JoinConversation` does not verify that the requesting user is actually a participant in the conversation. Any authenticated user can join any conversation group by passing the conversation ID. This is a **security vulnerability**.

### 4.2 RabbitMQ Message Bus

**File:** `Common.Services.Messaging/RabbitMqPublisher.cs`

**Exchange Topology:** All exchanges are `fanout` with `durable: true`. Messages use `DeliveryMode = 2` (persistent).

**Strengths:**
- Automatic recovery enabled (`AutomaticRecoveryEnabled`, `TopologyRecoveryEnabled`)
- Heartbeat configured (30s)
- Graceful failure -- silently continues if RabbitMQ is unavailable

**Issues:**

1. **Single Channel, Single Connection:** The publisher uses one connection and one channel for all publishing. While thread-safety is handled via `lock(_sync)`, this creates a bottleneck under high throughput. RabbitMQ recommends one channel per thread.

2. **Fire-and-Forget Publishing:** `PublishAsync` returns immediately without waiting for broker acknowledgment (`ConfirmSelect` / `WaitForConfirms` not used). Messages could be lost if the broker fails before persisting. For critical events (message delivery, deal creation), publisher confirms should be enabled.

3. **No Dead Letter Queue:** No DLQ configuration visible. Failed message processing in workers will result in message loss or infinite retry loops depending on worker implementation.

4. **Singleton Publisher as `new RabbitMqPublisher()`:** In `ApplicationDependencyProvider`, the publisher is instantiated directly:
   ```csharp
   base.Add<IRabbitMqPublisher>(new RabbitMqPublisher());
   ```
   This means each `ArfBlocksDependencyProvider` resolution creates a new publisher. Since the DI provider may be scoped per request, this could create many RabbitMQ connections. The publisher should be registered as a true singleton.

### 4.3 Domain Events

**File:** `LivestockTrading.Domain/Events/MessagingEvents.cs`

Events defined: `MessageCreatedEvent`, `MessageReadEvent`, `TypingIndicatorEvent`, `ConversationCreatedEvent`, `UserOnlineEvent`, `UserOfflineEvent`.

**Issue:** Events are published directly to RabbitMQ from handlers. There is no outbox pattern, which means if the database transaction succeeds but RabbitMQ publish fails (or vice versa), data will be inconsistent. For a trading platform, this is a critical reliability concern.

---

## 5. Security Analysis

### 5.1 JWT Authentication - CRITICAL

**API Gateway (`Gateways.ApiGateway.Api/Startup.cs`):**

```csharp
string secretKey = "Rj!7dXrQ*5z9@Lb^PqKf!M2&gUw#AeZx7Vp$ChN+36YT@tW%";
```

**CRITICAL SECURITY ISSUE: JWT Secret Key Hardcoded in Source Code**

The JWT signing key is:
1. **Hardcoded** in `Startup.cs` (line 24) -- committed to version control
2. **Used directly as the Ocelot AuthenticationProviderKey** in `ocelot.json` (lines 103, 134, 151)
3. **Visible in plain text** in the configuration file

This is a **P0 security vulnerability**. Anyone with repository access can forge valid JWT tokens for any user, including admin accounts. The key must be:
- Stored in environment variables or a secrets manager (Azure Key Vault, AWS Secrets Manager)
- Different per environment (dev/staging/prod)
- Rotatable without code deployment

### 5.2 CORS Configuration

**API Gateway:**
```csharp
builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
```

**LivestockTrading API:**
```csharp
builder.AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed(_ => true).AllowCredentials();
```

Both configurations are **wide open**. The LivestockTrading API is especially problematic -- it allows credentials from any origin. In production, CORS should restrict origins to known frontend domains.

### 5.3 Authorization Model

The `PermissionService` provides role-based access control with 5 roles: Admin, Moderator, Seller, Transporter, Buyer.

**Strengths:**
- Hierarchical role checking (Admin can do everything Moderator can, etc.)
- Permission enum system allows fine-grained control
- `RequireX()` methods throw `ArfBlocksVerificationException` for clear authorization failures

**Weaknesses:**

1. **No Resource-Level Authorization:** The system checks roles but not resource ownership. For example, `RequireSeller()` verifies the user is a seller, but does not verify they own the specific product being updated. A seller could modify another seller's products. The Verificator pattern should include ownership checks.

2. **No Rate Limiting:** No visible rate limiting on any endpoint. The API is vulnerable to:
   - Brute force attacks on auth endpoints
   - Enumeration attacks on product/user data
   - Message flooding via the messaging system

3. **OTP Code Storage:** `User.LastOtpCode` stores the OTP in plain text. While time-limited via `LastOtpSentAt`, it should be hashed.

### 5.4 Input Validation

FluentValidation is used consistently in Validator.cs files. The validation pipeline ensures:
1. Request model structure validation (FluentValidation)
2. Domain validation (unique slug checks, existence checks)

**Gap:** No visible input sanitization for HTML/XSS in free-text fields (Description, Content, Comments). The `Message.Content`, `Product.Description`, and `Offer.Message` fields accept arbitrary strings.

### 5.5 API Gateway Route Security

The `ocelot.json` defines public routes without `AuthenticationOptions`:
- `/iam/auth/*` - Login/Register (correct)
- `/iam/Users/Create` - Registration (correct)
- `/iam/Countries/All` - Country list (correct)
- `/fileprovider/Buckets/Detail` - **Public file access** (needs review)
- `/livestocktrading/hubs/*` - SignalR hub (has its own `[Authorize]`)

**Concern:** The catch-all route `"/"` maps to LivestockTrading API without authentication. This could expose unintended endpoints.

---

## 6. Scalability Concerns

### 6.1 Database Scalability

1. **Single Database:** All modules share one SQL Server database. At scale, this becomes a bottleneck. The modular monolith design should support eventual database-per-module extraction.

2. **No Connection Pooling Configuration:** SQL Server connection string doesn't specify `Max Pool Size`. Default is 100, which may be insufficient under load.

3. **Retry Policy:** `EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: 3s)` is configured. Good for transient failure handling.

4. **No Read Replica Support:** All queries hit the primary database. For a marketplace with heavy read traffic, read replicas would significantly improve performance.

5. **Soft Delete Table Growth:** Soft-deleted records remain in tables forever. Large tables (Messages, ProductViewHistory, SearchHistory, Notifications) will grow unbounded. An archival strategy is needed.

### 6.2 Product Listing Performance

The Products/All endpoint performs:
```csharp
_dbContext.Products
    .AsNoTracking()
    .Include(p => p.Location)
    .Where(p => !p.IsDeleted)
    .Where(p => p.Location.CountryCode == countryCode)
    .Sort(sorting)
    .Filter(filters)
    .Paginate(page)
```

**Issues:**
- No caching of product listing results
- Missing composite index on `(IsDeleted, Status, CountryCode)` for the most common query pattern
- `Include(p => p.Location)` loads full Location entity when only `CountryCode` and `City` might be needed
- No eager loading of Category/Brand/Seller for display names

### 6.3 Message Table Growth

The Messages table will grow linearly with platform usage. With no archival strategy, queries like "load conversation messages" will degrade over time. Recommended:
- Composite index on `(ConversationId, SentAt DESC)` for efficient pagination
- Archive old messages to cold storage after 1+ years

### 6.4 SignalR Scaling

Redis backplane is correctly configured for multi-instance SignalR:
```csharp
builder.Services.AddSignalR()
    .AddStackExchangeRedis(redisConnectionString, options =>
    {
        options.Configuration.ChannelPrefix = "LivestockTrading";
    });
```

However, the `Clients.All.SendAsync()` calls in presence tracking will not scale. With 10,000 concurrent users, every connect/disconnect sends a message to all 10,000 clients.

### 6.5 ViewCount/FavoriteCount Race Conditions

`Product.ViewCount` and `Product.FavoriteCount` are updated directly on the entity. Under concurrent access, these will lose increments. Solution: Use SQL `UPDATE SET ViewCount = ViewCount + 1` or track in Redis and periodically sync.

---

## 7. DevOps & Deployment

### 7.1 Docker Architecture

**Strengths:**
- Clean service separation with health checks for all infrastructure services
- YAML anchors for shared environment configuration
- Production overlay with resource limits
- Network aliases match Ocelot route expectations

**Resource Limits (Production):**

| Service | CPU | Memory |
|---------|-----|--------|
| API Gateway | 1 | 512M |
| LivestockTrading API | 2 | 2G |
| IAM API | 1 | 1G |
| FileProvider API | 1 | 1G |
| Workers (each) | 0.5 | 512M |
| SQL Server | 2 | 4G |
| Redis | - | 1G (maxmemory) |

**Concerns:**
- SQL Server with 4G memory for a trading platform may be insufficient at scale
- No horizontal scaling configuration (replicas)
- No Nginx/reverse proxy in the Docker compose -- the README mentions Nginx but it's not defined

### 7.2 Jenkins CI/CD

Two pipelines:
- `Jenkinsfile.dev` - Auto-deploy on `dev` branch
- `Jenkinsfile.prod` - Deploy on `main` with immutable image tags

**Good practices:**
- Immutable tags for production (`prod-{BUILD_ID}-{COMMIT}`)
- Separate environment files
- Automatic migration in dev, explicit in prod

### 7.3 Missing Infrastructure

1. **No Health Check Endpoints:** The APIs don't expose `/health` or `/ready` endpoints for container orchestration
2. **No Prometheus/Metrics:** No metrics endpoint for monitoring
3. **No Log Aggregation Config:** Serilog is configured but no sink for centralized logging (ELK, Seq) in Docker compose
4. **No Backup Strategy:** SQL Server data is in a Docker volume with no backup configuration

---

## 8. Critical Issues (Ranked by Severity)

### P0 - Must Fix Immediately

| # | Issue | Location | Risk |
|---|-------|----------|------|
| 1 | **JWT secret key hardcoded in source code** | `Gateways.ApiGateway.Api/Startup.cs:24`, `ocelot.json:103,134,151` | Any repository reader can forge admin JWT tokens |
| 2 | **No conversation membership validation in SignalR** | `ChatHub.cs:JoinConversation` | Any user can eavesdrop on any conversation |
| 3 | **Wide-open CORS with credentials** | `LivestockTrading.Api/Program.cs:19-29` | Cross-site request forgery possible from any domain |

### P1 - Fix Before Production

| # | Issue | Location | Risk |
|---|-------|----------|------|
| 4 | No resource ownership checks in authorization | All Verificators | Sellers can modify other sellers' products |
| 5 | No optimistic concurrency control | All entities (missing `RowVersion`) | Lost updates on concurrent modifications (especially Deals, Offers) |
| 6 | `Clients.All.SendAsync` for presence broadcasts | `ChatHub.cs:156,175` | O(N) message fan-out on every connect/disconnect |
| 7 | Cache stampede vulnerability | `CacheService.GetOrCreateAsync` | Database overload when cache expires under high traffic |
| 8 | No publisher confirms for RabbitMQ | `RabbitMqPublisher.cs` | Message loss for critical events (deals, payments) |
| 9 | OTP stored in plain text | `User.LastOtpCode` | OTP disclosure if database is compromised |
| 10 | Missing critical database indexes | `LivestockTradingModelBuilder.cs` | Slow queries on Product.Status, Location.CountryCode |

### P2 - Fix in Next Sprint

| # | Issue | Location | Risk |
|---|-------|----------|------|
| 11 | Client-generated GUIDs as clustered PKs | All entities via `BaseEntity` | Index fragmentation, degraded insert performance |
| 12 | Duplicate presence logic (ChatHub + PresenceService) | `ChatHub.cs`, `PresenceService.cs` | Logic divergence, maintenance burden |
| 13 | No global query filter for soft delete | `LivestockTradingModelBuilder.cs` | Risk of accidentally querying deleted records |
| 14 | No rate limiting on any endpoint | API Gateway / APIs | Brute force, enumeration, flooding attacks |
| 15 | RabbitMQ publisher per-request instantiation | `ApplicationDependencyProvider.cs:40` | Connection leak under high throughput |
| 16 | `server.Keys()` usage for cache pattern deletion | `CacheService.cs:176` | Redis blocking under large key count |
| 17 | No outbox pattern for event publishing | All handlers that publish events | Data/event inconsistency if one fails |
| 18 | No input sanitization for HTML/XSS | Message.Content, Product.Description | Stored XSS vulnerability |

### P3 - Technical Debt

| # | Issue | Location | Risk |
|---|-------|----------|------|
| 19 | `new Mapper()` in handlers (not injected) | All Handler.cs files | Untestable mapping logic |
| 20 | DbValidationService and DbVerificationService duplicate methods | Both infrastructure services | Maintenance overhead, risk of divergence |
| 21 | No health check endpoints | All API projects | Poor observability in container orchestration |
| 22 | No centralized CacheKeys management | Various files | Key collision risk, typo-prone |
| 23 | JSON string columns for structured data | Multiple entities | No type safety, no queryability |
| 24 | Npgsql switch set despite using SQL Server | `DefinitionDbContext.cs:41` | Dead code, potential confusion |
| 25 | `StudentEvents.cs` in LivestockTrading domain | `Domain/Events/StudentEvents.cs` | Leftover from template/scaffold |

---

## 9. Improvement Recommendations

### 9.1 Immediate Actions (Week 1)

1. **Move JWT secret to environment variable / secrets manager.** Remove from source code entirely. Use `IConfiguration` to read at runtime.

2. **Add conversation membership validation** in `ChatHub.JoinConversation()` -- query the database to verify `userId` is `ParticipantUserId1` or `ParticipantUserId2`.

3. **Restrict CORS origins** to known frontend domains in production. Use environment-specific CORS configuration.

4. **Add critical indexes:**
   ```csharp
   entity.HasIndex(e => e.Status);
   entity.HasIndex(e => e.CategoryId);
   entity.HasIndex(e => e.SellerId);
   entity.HasIndex(e => new { e.IsDeleted, e.Status, e.CreatedAt });
   // Location
   entity.HasIndex(e => e.CountryCode);
   // Message
   entity.HasIndex(e => new { e.ConversationId, e.SentAt });
   ```

### 9.2 Short-Term (Sprint 1-2)

5. **Add resource ownership verification** in Verificators:
   ```csharp
   public async Task VerificateDomain(IRequestModel payload, ...)
   {
       var product = await _dbContext.Products.FindAsync(request.Id);
       if (product.SellerId != _currentUser.GetSellerId())
           throw new ArfBlocksVerificationException("Not your product");
   }
   ```

6. **Implement global query filters** for soft delete:
   ```csharp
   modelBuilder.Entity<Product>().HasQueryFilter(e => !e.IsDeleted);
   ```

7. **Add optimistic concurrency** to critical entities (Deal, Offer, Product):
   ```csharp
   [Timestamp]
   public byte[] RowVersion { get; set; }
   ```

8. **Fix presence broadcasting** -- replace `Clients.All` with targeted notifications to relevant users only.

9. **Register RabbitMqPublisher as singleton** and use `IServiceProvider` to resolve it properly.

### 9.3 Medium-Term (Quarter)

10. **Implement outbox pattern** for reliable event publishing. Store events in a database table within the same transaction, then publish asynchronously.

11. **Add caching layer for reference data** (Categories, Brands, Currencies, Languages) with 5-10 minute TTL.

12. **Implement rate limiting** via ASP.NET Core rate limiting middleware or API Gateway level.

13. **Add health check endpoints** (`/health`, `/ready`) for all services.

14. **Set up log aggregation** (Seq, ELK, or Azure Application Insights) for centralized monitoring.

15. **Implement Redis atomic operations** for presence tracking instead of serialize/deserialize patterns.

### 9.4 Long-Term (Roadmap)

16. **Database-per-module preparation** -- introduce explicit contracts (interfaces) for cross-module data access instead of shared DbContext inheritance.

17. **CQRS read model** -- for high-traffic queries (product listings, search), consider materialized views or a separate read-optimized store.

18. **Event-driven architecture** -- replace synchronous cross-module calls with domain events for loose coupling.

19. **Automated testing infrastructure** -- the current architecture makes unit testing very difficult due to the ArfBlocks DI model. Consider integration test suite.

20. **Data archival strategy** -- implement for high-volume tables (Messages, ViewHistory, SearchHistory, Notifications).

---

## 10. Summary

The GlobalLivestock backend demonstrates a well-organized modular monolith with consistent patterns and a comprehensive domain model covering the full livestock trading workflow. The ArfBlocks framework provides a productive development experience with its convention-based approach.

The **most critical concern** is the hardcoded JWT secret key (P0), which must be addressed immediately. Beyond that, the main areas requiring attention are:

1. **Security hardening** -- CORS, ownership checks, rate limiting, input sanitization
2. **Database optimization** -- missing indexes, no concurrency control, GUID fragmentation
3. **Messaging reliability** -- cache coherency, RabbitMQ publisher confirms, outbox pattern
4. **Scalability preparation** -- SignalR broadcast optimization, caching strategy, read replicas

The architecture is solid for current scale and provides a good foundation for growth with the recommended improvements.
