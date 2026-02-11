# QA Strategy & Testing Plan - GlobalLivestock Backend

## Table of Contents
1. [Current Test Coverage Assessment](#1-current-test-coverage-assessment)
2. [Critical User Journey Maps](#2-critical-user-journey-maps)
3. [Complete API Endpoint Inventory](#3-complete-api-endpoint-inventory)
4. [Risk Areas & Untested Paths](#4-risk-areas--untested-paths)
5. [Proposed Testing Strategy](#5-proposed-testing-strategy)
6. [Test Priority Matrix](#6-test-priority-matrix)
7. [Recommended Test Infrastructure](#7-recommended-test-infrastructure)
8. [Sample Test Scenarios](#8-sample-test-scenarios)

---

## 1. Current Test Coverage Assessment

### 1.1 Automated Tests: NONE

The solution contains **zero automated test projects**. No `*.Tests.csproj` files exist anywhere in the repository. The only testing mechanism in place is:

- **6 PowerShell scripts** (`comprehensive-crud-test.ps1` through `comprehensive-crud-test-6.ps1`) that perform manual HTTP-based CRUD testing against a running instance
- **44 `.http` files** in `_doc/Http/` for manual endpoint testing via IDE REST clients
- Both approaches require a running server with a valid JWT token

### 1.2 Manual Test Scripts Analysis

The PowerShell scripts test basic CRUD operations (Create x2, Update, Delete) for entities but have significant gaps:
- Hardcoded JWT token (expires, requires manual refresh)
- No validation error scenario testing
- No authorization/role-based testing
- No concurrent operation testing
- No cleanup or idempotent test runs

### 1.3 Coverage Summary

| Layer | Automated Coverage | Manual Coverage | Gap |
|-------|-------------------|-----------------|-----|
| Domain Entities (28 entities) | 0% | N/A | CRITICAL |
| Validators (FluentValidation) | 0% | Partial | HIGH |
| Verificators (Authorization) | 0% | None | CRITICAL |
| Mappers (Entity <-> DTO) | 0% | Implicit | HIGH |
| DataAccess (EF Core queries) | 0% | Implicit | HIGH |
| Handlers (Business logic) | 0% | Partial via HTTP | MEDIUM |
| TranslationHelper | 0% | None | MEDIUM |
| PermissionService (RBAC) | 0% | None | CRITICAL |
| ChatHub (SignalR) | 0% | None | HIGH |
| Workers (RabbitMQ consumers) | 0% | None | HIGH |
| API Gateway (Ocelot) | 0% | Manual | MEDIUM |

---

## 2. Critical User Journey Maps

### Journey 1: Registration & Authentication

```
[New User] --> POST /iam/Users/Create (public)
    |           - Email/password registration
    |           - Auto-assigns Buyer role for LivestockTrading module
    |           - Admin emails get Admin role instead
    |
    +--> POST /iam/auth/Login (public)
    |       - Provider: native | google | apple
    |       - Google/Apple: auto-creates user if not exists
    |       - Returns JWT + RefreshToken
    |       - JWT contains: userId, roles as "ModuleName.RoleName"
    |
    +--> POST /iam/auth/RefreshToken (public)
    |       - Extends session with new JWT
    |
    +--> POST /iam/auth/Logout (authenticated)
    |       - Revokes refresh token
    |
    +--> POST /iam/auth/SendOtp (public)
    |       - Phone/email verification
    |
    +--> POST /iam/auth/VerifyOtp (public)
            - Confirms OTP code
```

**Risk Points:**
- No email verification step before account activation
- Admin role assignment by hardcoded email list (not configurable)
- Google/Apple provider creates user without password (no native login fallback)
- No rate limiting on Login/OTP endpoints
- JWT token expiration: controlled by `_jwtService.GetExpirationDayCount()` -- no session revocation
- Refresh token stored in DB but no mechanism to limit active sessions per user

### Journey 2: Seller Onboarding

```
[Buyer User] --> POST /livestocktrading/Sellers/Create (authenticated)
    |               - Creates Seller profile (Status: PendingVerification)
    |               - Auto-assigns Seller role to user
    |               - Requires: BusinessName, UserId
    |
    +--> POST /livestocktrading/Farms/Create (Seller role)
    |       - Creates farm linked to seller
    |       - Requires: SellerId, LocationId, Name
    |
    +--> [Moderator/Admin] POST /livestocktrading/Sellers/Verify
    |       - Changes status: PendingVerification --> Active
    |       - Requires: Moderator or Admin role
    |
    +--> POST /livestocktrading/Locations/Create (authenticated)
            - Creates location for products/farms
            - Requires: Name, CountryCode
```

**Risk Points:**
- User can create multiple Seller profiles for same UserId (no uniqueness check in Validator)
- Seller role is auto-assigned even if seller profile creation partially fails
- No validation that Seller.UserId matches the current authenticated user
- Seller can be created with another user's UserId (IDOR vulnerability)
- Farm creation doesn't verify that SellerId belongs to current user

### Journey 3: Product Listing (Full Lifecycle)

```
[Seller] --> POST /livestocktrading/Locations/Create
    |           - Creates product location
    |
    +--> POST /livestocktrading/Products/Create (Seller or Admin)
    |       - Auto-creates Seller profile if none exists
    |       - Status defaults to Draft
    |       - Validates: Title, Slug (unique), CategoryId, LocationId, BasePrice > 0
    |       - SellerId optional (auto-resolves from current user)
    |
    +--> POST /livestocktrading/AnimalInfos/Create (for livestock)
    |   POST /livestocktrading/MachineryInfos/Create (for machinery)
    |   POST /livestocktrading/ChemicalInfos/Create (for chemicals)
    |   POST /livestocktrading/SeedInfos/Create (for seeds)
    |   POST /livestocktrading/FeedInfos/Create (for feed)
    |   POST /livestocktrading/VeterinaryInfos/Create (for vet products)
    |       - Category-specific details linked to ProductId
    |
    +--> POST /livestocktrading/ProductVariants/Create
    |       - Size/color/weight variations
    |
    +--> POST /livestocktrading/ProductPrices/Create
    |       - Multi-currency pricing
    |
    +--> [File Upload via FileProvider]
    |       - Upload images/videos, get MediaBucketId
    |       - Set CoverImageFileId on product
    |
    +--> POST /livestocktrading/Products/Update
    |       - Change status to PendingApproval
    |
    +--> [Moderator] POST /livestocktrading/Products/Approve
    |       - Status: PendingApproval --> Active
    |       - PublishedAt timestamp set
    |
    +--> [Moderator] POST /livestocktrading/Products/Reject
            - Status: PendingApproval --> Rejected
            - Reason required
```

**Risk Points:**
- Product.Status can be set directly via Create/Update (client can bypass PendingApproval)
- No validation that product's SellerId matches logged-in user on Update
- No limit on number of products per seller
- Product slug uniqueness check excludes soft-deleted products (reuse possible)
- MediaBucketId/CoverImageFileId are free-text strings (no FileProvider validation)
- No expiration enforcement (ExpiresAt field exists but not checked)

### Journey 4: Buyer Discovery & Purchase

```
[Buyer] --> POST /livestocktrading/Products/Search (authenticated)
    |           - Full-text search by Query across Title, Description, Slug
    |           - Filters: CategoryId, BrandId, MinPrice, MaxPrice, Condition
    |           - Location: CountryCode, City
    |           - Only shows Status=Active products
    |           - Pagination + Sorting
    |
    +--> POST /livestocktrading/Products/All (authenticated)
    |       - Generic listing with XSorting, XFilter, XPageRequest
    |       - CountryCode filter support
    |
    +--> POST /livestocktrading/Products/Detail (authenticated)
    |   POST /livestocktrading/Products/DetailBySlug (authenticated)
    |       - Full product details with Category, Brand, Seller, Location
    |
    +--> POST /livestocktrading/Products/MediaDetail (authenticated)
    |       - Media files for a product
    |
    +--> POST /livestocktrading/ProductViewHistories/Create (authenticated)
    |       - Track product views
    |
    +--> POST /livestocktrading/SearchHistories/Create (authenticated)
    |       - Track search queries
    |
    +--> POST /livestocktrading/FavoriteProducts/Create (Buyer or Seller)
    |       - Add to favorites
    |
    +--> POST /livestocktrading/Conversations/Create (authenticated)
    |       - Start conversation with seller about product
    |       - Publishes ConversationCreatedEvent via RabbitMQ
    |
    +--> POST /livestocktrading/Messages/Create (authenticated)
    |       - Send message in conversation
    |       - Publishes MessageCreatedEvent via RabbitMQ
    |       - Real-time delivery via SignalR ChatHub
    |
    +--> POST /livestocktrading/Offers/Create (authenticated)
    |       - Make price offer on product
    |       - Supports counter-offers (CounterOfferToId)
    |
    +--> POST /livestocktrading/Deals/Create (authenticated)
            - Record agreed deal
            - Links to Product, Seller, Buyer
```

**Risk Points:**
- Products/Search only filters Active status, but Products/All may return non-active products
- No mechanism to prevent buyer messaging themselves
- Conversation can be created without validating both participant IDs
- Offer has no validation that BuyerUserId matches current user
- Deal creation has no offer-acceptance workflow prerequisite
- No purchase/order entity exists yet -- Deal is an informal record

### Journey 5: Real-Time Messaging

```
[User] --> WebSocket: /livestocktrading/hubs/chat (JWT required)
    |           - OnConnected: SetUserOnline (Redis), broadcast UserOnline
    |
    +--> Client: JoinConversation(conversationId)
    |       - Adds to SignalR group: "conversation_{id}"
    |
    +--> Client: SendTypingIndicator(conversationId, isTyping)
    |       - Broadcasts to group (excluding sender)
    |
    +--> API: POST /Messages/Create
    |       - Saves to DB + publishes MessageCreatedEvent
    |       - NotificationSender worker -> SignalR ReceiveMessage
    |
    +--> Client: MarkMessageAsRead(messageId, conversationId)
    |       - Broadcasts MessageRead to group
    |
    +--> Client: GetOnlineUsers(userIds)
    |       - Checks Redis for each userId
    |
    +--> OnDisconnected:
            - Remove connection from Redis set
            - If no more connections: SetUserOffline, broadcast UserOffline
```

**Risk Points:**
- ChatHub.JoinConversation has NO authorization check (any authenticated user can join any conversation)
- MarkMessageAsRead only broadcasts -- doesn't update DB (IsRead field not synced)
- GetOnlineUsers iterates userIds sequentially (N+1 Redis calls, no batching)
- SetUserOnline broadcasts to ALL clients (privacy issue, should only notify contacts)
- Multi-device connections use HashSet<string> in Redis (potential race condition)
- No conversation membership validation before SendTypingIndicator

### Journey 6: Transport & Logistics

```
[After Deal] --> POST /livestocktrading/TransportRequests/Create
    |               - Creates transport request for a deal
    |               - Links Product, Seller, Buyer, PickupLocation, DeliveryLocation
    |               - Status: Pending
    |
    +--> POST /livestocktrading/TransportRequests/Update
    |       - Set IsInPool=true, Status=InPool
    |       - Visible to transporters
    |
    +--> POST /livestocktrading/TransportOffers/Create (Transporter)
    |       - Transporter bids on request
    |       - OfferedPrice, EstimatedDates, VehicleType
    |
    +--> POST /livestocktrading/TransportOffers/Update
    |       - Accept/Reject offer
    |       - On Accept: TransportRequest.AssignedTransporterId set
    |
    +--> POST /livestocktrading/TransportTrackings/Create
    |       - Status updates: PickedUp, InTransit, Delivered
    |       - GPS coordinates (Latitude, Longitude)
    |
    +--> POST /livestocktrading/TransporterReviews/Create
            - Review transporter after delivery
```

**Risk Points:**
- No state machine enforcement on TransportRequest status transitions
- Multiple offers can be accepted for same request (no exclusivity)
- TransportTracking has no validation that reporter is assigned transporter
- No notification when transport status changes

### Journey 7: Review & Rating System

```
[After Purchase] --> POST /livestocktrading/ProductReviews/Create
    |                   - Rating 1-5, Comment
    |                   - IsVerifiedPurchase (not enforced)
    |                   - IsApproved defaults to false
    |
    +--> POST /livestocktrading/SellerReviews/Create
    |       - Multi-dimensional: Overall, Communication, ShippingSpeed, ProductQuality
    |       - IsApproved defaults to false
    |
    +--> POST /livestocktrading/TransporterReviews/Create
            - Multi-dimensional: Overall, Timeliness, Communication, CarefulHandling, Professionalism
```

**Risk Points:**
- No review moderation workflow (IsApproved exists but no Approve/Reject endpoints)
- Rating range (1-5) not validated by FluentValidation
- User can review same product/seller/transporter multiple times
- No verified purchase enforcement (IsVerifiedPurchase always false)
- AverageRating on Product/Seller/Transporter not recalculated after new review

---

## 3. Complete API Endpoint Inventory

### 3.1 IAM Module (BaseModules.IAM) - 19 Endpoints

| Endpoint | Type | Auth | Description |
|----------|------|------|-------------|
| Auth/Login | Command | Public | User login (native/google/apple) |
| Auth/Logout | Command | Auth | Revoke session |
| Auth/RefreshToken | Command | Public | Refresh JWT |
| Auth/RevokeRefreshToken | Command | Auth | Revoke specific token |
| Auth/SendOtp | Command | Public | Send OTP code |
| Auth/VerifyOtp | Command | Public | Verify OTP code |
| Users/Create | Command | Public | Register new user |
| Users/ForgotPassword | Command | Public | Initiate password reset |
| Users/ResetPassword | Command | Public | Complete password reset |
| Users/Update | Command | Auth | Update user profile |
| Countries/All | Query | Public | List countries |
| Countries/Detail | Query | Auth | Country detail |
| Provinces/All | Query | Auth | List provinces |
| Districts/All | Query | Auth | List districts |
| Neighborhoods/All | Query | Auth | List neighborhoods |
| Role/* | CRUD | Auth | Role management |
| UserPermissions/* | CRUD | Auth | User permissions |
| MobilApplicationVersiyon/* | CRUD | Auth | Mobile version check |
| Push/* | Command | Auth | Push notification tokens |

### 3.2 LivestockTrading Module - 259 Endpoints (43 Entities x ~6 Operations)

**Core Commerce Entities (full CRUD + special operations):**

| Entity | Operations | Special Endpoints |
|--------|-----------|-------------------|
| Products | Create, Update, Delete, All, Detail, Pick | Approve, Reject, DetailBySlug, MediaDetail, Search |
| Sellers | Create, Update, Delete, All, Detail, Pick | Verify, Suspend, DetailByUserId |
| Transporters | Create, Update, Delete, All, Detail, Pick | Verify, Suspend |
| Categories | Create, Update, Delete, All, Detail, Pick | - |
| Brands | Create, Update, Delete, All, Detail, Pick | - |
| Farms | Create, Update, Delete, All, Detail, Pick | - |
| Locations | Create, Update, Delete, All, Detail, Pick | - |
| Conversations | Create, Update, Delete, All, Detail, Pick | - |
| Messages | Create, Update, Delete, All, Detail, Pick | SendTypingIndicator, UnreadCount |
| Offers | Create, Update, Delete, All, Detail, Pick | - |
| Deals | Create, Update, Delete, All, Detail, Pick | - |
| Dashboard | - | Stats |

**Product Detail Entities (linked to ProductId):**

| Entity | Operations |
|--------|-----------|
| AnimalInfos | Create, Update, Delete, All, Detail, Pick |
| ChemicalInfos | Create, Update, Delete, All, Detail, Pick |
| MachineryInfos | Create, Update, Delete, All, Detail, Pick |
| SeedInfos | Create, Update, Delete, All, Detail, Pick |
| FeedInfos | Create, Update, Delete, All, Detail, Pick |
| VeterinaryInfos | Create, Update, Delete, All, Detail, Pick |
| HealthRecords | Create, Update, Delete, All, Detail, Pick |
| Vaccinations | Create, Update, Delete, All, Detail, Pick |
| ProductVariants | Create, Update, Delete, All, Detail, Pick |
| ProductPrices | Create, Update, Delete, All, Detail, Pick |

**Transport & Logistics:**

| Entity | Operations |
|--------|-----------|
| TransportRequests | Create, Update, Delete, All, Detail, Pick |
| TransportOffers | Create, Update, Delete, All, Detail, Pick |
| TransportTrackings | Create, Update, Delete, All, Detail, Pick |

**Reviews & Ratings:**

| Entity | Operations |
|--------|-----------|
| ProductReviews | Create, Update, Delete, All, Detail, Pick |
| SellerReviews | Create, Update, Delete, All, Detail, Pick |
| TransporterReviews | Create, Update, Delete, All, Detail, Pick |

**User Experience:**

| Entity | Operations |
|--------|-----------|
| FavoriteProducts | Create, Update, Delete, All, Detail, Pick |
| Notifications | Create, Update, Delete, All, Detail, Pick |
| SearchHistories | Create, Update, Delete, All, Detail, Pick |
| ProductViewHistories | Create, Update, Delete, All, Detail, Pick |
| UserPreferences | Create, Update, Delete, All, Detail, Pick |

**Configuration/Reference:**

| Entity | Operations |
|--------|-----------|
| Currencies | Create, Update, Delete, All, Detail, Pick |
| Languages | Create, Update, Delete, All, Detail, Pick |
| PaymentMethods | Create, Update, Delete, All, Detail, Pick |
| ShippingCarriers | Create, Update, Delete, All, Detail, Pick |
| ShippingRates | Create, Update, Delete, All, Detail, Pick |
| ShippingZones | Create, Update, Delete, All, Detail, Pick |
| TaxRates | Create, Update, Delete, All, Detail, Pick |
| Banners | Create, Update, Delete, All, Detail, Pick |
| FAQs | Create, Update, Delete, All, Detail, Pick |

### 3.3 FileProvider Module

| Endpoint | Auth | Description |
|----------|------|-------------|
| Buckets/Detail | Public | Get bucket/media details |
| All other endpoints | Auth | File upload/management |

### 3.4 SignalR Hub

| Method | Direction | Description |
|--------|-----------|-------------|
| JoinConversation | Client->Server | Join conversation group |
| LeaveConversation | Client->Server | Leave conversation group |
| SendTypingIndicator | Client->Server | Typing indicator |
| MarkMessageAsRead | Client->Server | Mark message read |
| GetOnlineUsers | Client->Server | Check online status |
| ReceiveMessage | Server->Client | New message received |
| TypingIndicator | Server->Client | Typing indicator event |
| MessageRead | Server->Client | Message read event |
| UserOnline | Server->Client | User came online |
| UserOffline | Server->Client | User went offline |

---

## 4. Risk Areas & Untested Paths

### 4.1 CRITICAL Security Risks

| Risk | Location | Description |
|------|----------|-------------|
| **IDOR: Seller Creation** | Sellers/Create/Handler.cs | User can create seller profile with any UserId, not just their own |
| **IDOR: Product Ownership** | Products/Update, Products/Delete | No check that SellerId belongs to current user |
| **IDOR: Conversation Access** | ChatHub.JoinConversation | Any authenticated user can join any conversation |
| **IDOR: Message Access** | Messages/All | May return messages from conversations user is not part of |
| **Status Bypass** | Products/Create Models.cs:23 | Client can set Status field directly (e.g., set Active, skip approval) |
| **Hardcoded Admin List** | Auth/Login, Users/Create | Admin emails hardcoded in source code |
| **JWT Secret in ocelot.json** | ocelot.json:103 | Authentication key visible in source code |
| **No Rate Limiting** | All endpoints | No rate limiting middleware on any endpoint |
| **No Input Sanitization** | All string fields | XSS risk in Description, Comment, Content fields |

### 4.2 HIGH Business Logic Risks

| Risk | Description |
|------|-------------|
| **Duplicate Seller Profiles** | No unique constraint on Seller.UserId |
| **Duplicate Favorites** | User can favorite same product multiple times |
| **Duplicate Reviews** | User can review same product/seller/transporter multiple times |
| **No Rating Recalculation** | Product.AverageRating not updated when review created |
| **Offer Race Condition** | Multiple offers can be accepted simultaneously |
| **Transport Offer Race** | Multiple transport offers accepted for same request |
| **No State Machine** | Status transitions not enforced (e.g., Draft->Active without PendingApproval) |
| **Soft Delete Orphans** | Deleting seller doesn't cascade to products, farms |
| **No Product Expiration** | ExpiresAt exists but never checked/enforced |
| **Counter-Offer Chain** | No depth limit on counter-offer chains |

### 4.3 MEDIUM Data Integrity Risks

| Risk | Description |
|------|-------------|
| **Translation JSON Malformation** | TranslationHelper catches JsonException but no input validation on write |
| **Currency Mismatch** | Product.Currency is free-text, no validation against Currencies table |
| **Country Code Validation** | CountryCode is free-text, no validation against Countries table |
| **Decimal Precision** | Price/amount fields have no precision constraint |
| **Negative Prices** | BasePrice > 0 validated on Create, but not on Update |
| **Empty Slug** | Products/Update could set slug to empty |
| **Timestamp Consistency** | Some entities use DateTime.UtcNow in constructor, others rely on DB |
| **Enum Range** | Int-to-enum casting in Mappers has no range validation |

### 4.4 Concurrency & Performance Risks

| Risk | Description |
|------|-------------|
| **ViewCount Race Condition** | Product.ViewCount incremented without concurrency control |
| **FavoriteCount Race Condition** | Product.FavoriteCount not atomically updated |
| **Redis Connection Set** | ChatHub HashSet<string> operations not atomic |
| **N+1 Queries** | GetOnlineUsers calls Redis per userId (no pipelining) |
| **No Caching on Reads** | Products/All, Categories/All not cached |
| **Missing Indexes** | No explicit index declarations found for slug, countryCode, status |
| **Broadcast Storm** | UserOnline/UserOffline broadcast to ALL clients |

---

## 5. Proposed Testing Strategy

### 5.1 Testing Pyramid

```
          /  E2E Tests  \           <- 5% - Critical user journeys
         /  Integration  \          <- 25% - DataAccess + DB, SignalR
        /   Unit Tests    \         <- 70% - Validators, Mappers, Services
       /____________________\
```

### 5.2 Phase 1: Unit Tests (Priority: IMMEDIATE)

**Target: 70% of test effort**

Create project: `LivestockTrading.Tests.Unit`

| Component | Test Count (est.) | Priority |
|-----------|------------------|----------|
| FluentValidation rules (all Validator.cs) | ~130 tests | P0 |
| Mapper.cs (entity <-> DTO mapping) | ~90 tests | P0 |
| TranslationHelper | ~15 tests | P1 |
| PermissionService | ~25 tests | P0 |
| DomainErrors uniqueness | ~5 tests | P1 |
| Enum casting correctness | ~20 tests | P1 |

**What to test per Validator:**
- Required field validation (empty/null)
- String length limits
- Numeric range (prices > 0, ratings 1-5)
- Guid format validation
- Custom domain rules (slug uniqueness mock)

**What to test per Mapper:**
- All properties mapped correctly
- Enum <-> int casting correct
- Null navigation properties handled (Category?.Name)
- DateTime fields preserved
- Collection mappings

### 5.3 Phase 2: Integration Tests (Priority: HIGH)

**Target: 25% of test effort**

Create project: `LivestockTrading.Tests.Integration`

| Component | Test Count (est.) | Priority |
|-----------|------------------|----------|
| DataAccess layer (all entities) | ~120 tests | P0 |
| DbValidationService | ~50 tests | P0 |
| DbVerificationService | ~30 tests | P1 |
| Handler business logic | ~80 tests | P0 |
| ChatHub (in-memory SignalR) | ~15 tests | P1 |
| RabbitMQ event publishing | ~10 tests | P2 |

**Database Strategy:**
- Use EF Core In-Memory provider or SQLite in-memory for fast tests
- Use SQL Server TestContainers for full fidelity tests
- Seed minimal data per test scenario

**Key Integration Scenarios:**
1. Products/Create with auto-seller creation
2. Sellers/Create with auto-role assignment
3. Products/Approve state transition
4. Conversation/Message creation with event publishing
5. Soft-delete cascading behavior
6. Slug uniqueness across create/update
7. Pagination, sorting, filtering correctness

### 5.4 Phase 3: E2E / API Tests (Priority: MEDIUM)

**Target: 5% of test effort**

Create project: `LivestockTrading.Tests.E2E`

| Journey | Test Count (est.) | Priority |
|---------|------------------|----------|
| Auth flow (register -> login -> refresh -> logout) | ~8 tests | P0 |
| Seller onboarding (create seller -> verify -> create farm) | ~6 tests | P0 |
| Product lifecycle (create -> approve -> search -> detail) | ~10 tests | P0 |
| Messaging flow (conversation -> message -> read) | ~8 tests | P1 |
| Offer/Deal flow (offer -> counter -> accept -> deal) | ~6 tests | P1 |
| Transport flow (request -> offer -> assign -> track -> complete) | ~8 tests | P1 |
| Review flow (create review -> verify rating update) | ~5 tests | P2 |

### 5.5 Security Tests

| Test Type | Count | Priority |
|-----------|-------|----------|
| IDOR: Access other user's resources | ~15 tests | P0 |
| Role bypass: Buyer accessing Seller endpoints | ~10 tests | P0 |
| JWT: Expired/invalid/missing token | ~5 tests | P0 |
| Status bypass: Setting Active status directly | ~3 tests | P0 |
| Input injection: XSS in text fields | ~10 tests | P1 |
| SQL injection: Filter/Sort parameters | ~5 tests | P1 |

---

## 6. Test Priority Matrix

### Priority 0 (Must Have - Before Next Release)

| # | Test Area | Reason | Est. Tests |
|---|-----------|--------|------------|
| 1 | PermissionService + Verificators | Authorization is the security boundary | 35 |
| 2 | Products CRUD + Approve/Reject Validators | Core business flow | 25 |
| 3 | Auth Login/Register Handlers | Entry point to system | 15 |
| 4 | IDOR vulnerability tests | Critical security | 15 |
| 5 | Seller/Transporter Create + auto-role | Role escalation risk | 10 |
| 6 | Product Search DataAccess | Core user experience | 10 |
| **Total P0** | | | **110** |

### Priority 1 (Should Have - Within 2 Sprints)

| # | Test Area | Reason | Est. Tests |
|---|-----------|--------|------------|
| 7 | All Validator.cs FluentValidation rules | Data integrity | 80 |
| 8 | All Mapper.cs correctness | Data transformation | 60 |
| 9 | Conversation/Message handlers | Real-time messaging | 20 |
| 10 | ChatHub authorization | Security gap | 15 |
| 11 | TranslationHelper edge cases | Multi-language support | 15 |
| 12 | Offer/Deal state transitions | Business logic | 15 |
| **Total P1** | | | **205** |

### Priority 2 (Nice to Have - Ongoing)

| # | Test Area | Reason | Est. Tests |
|---|-----------|--------|------------|
| 13 | All DataAccess pagination/filtering | Query correctness | 60 |
| 14 | Soft delete cascading | Data integrity | 20 |
| 15 | Transport lifecycle E2E | Complex flow | 15 |
| 16 | Worker event handlers | Async reliability | 15 |
| 17 | Concurrency (ViewCount, Favorites) | Data consistency | 10 |
| 18 | API Gateway routing | Infrastructure | 10 |
| **Total P2** | | | **130** |

---

## 7. Recommended Test Infrastructure

### 7.1 Project Structure

```
Tests/
├── LivestockTrading.Tests.Unit/
│   ├── LivestockTrading.Tests.Unit.csproj
│   ├── Validators/
│   │   ├── Products/
│   │   │   ├── CreateValidatorTests.cs
│   │   │   ├── UpdateValidatorTests.cs
│   │   │   └── SearchValidatorTests.cs
│   │   ├── Sellers/
│   │   ├── Categories/
│   │   └── ... (one folder per entity)
│   ├── Mappers/
│   │   ├── ProductMapperTests.cs
│   │   ├── SellerMapperTests.cs
│   │   └── ...
│   ├── Services/
│   │   ├── PermissionServiceTests.cs
│   │   ├── TranslationHelperTests.cs
│   │   └── PresenceServiceTests.cs
│   └── Authorization/
│       └── RolePermissionTests.cs
│
├── LivestockTrading.Tests.Integration/
│   ├── LivestockTrading.Tests.Integration.csproj
│   ├── Infrastructure/
│   │   ├── TestDbContextFactory.cs
│   │   ├── TestDataSeeder.cs
│   │   └── IntegrationTestBase.cs
│   ├── DataAccess/
│   │   ├── ProductDataAccessTests.cs
│   │   ├── SellerDataAccessTests.cs
│   │   └── ...
│   ├── Handlers/
│   │   ├── ProductCreateHandlerTests.cs
│   │   ├── SellerCreateHandlerTests.cs
│   │   └── ...
│   ├── Services/
│   │   ├── DbValidationServiceTests.cs
│   │   └── DbVerificationServiceTests.cs
│   └── Hubs/
│       └── ChatHubTests.cs
│
├── LivestockTrading.Tests.E2E/
│   ├── LivestockTrading.Tests.E2E.csproj
│   ├── Infrastructure/
│   │   ├── TestWebApplicationFactory.cs
│   │   ├── AuthHelper.cs
│   │   └── E2ETestBase.cs
│   ├── Journeys/
│   │   ├── AuthenticationJourneyTests.cs
│   │   ├── SellerOnboardingJourneyTests.cs
│   │   ├── ProductLifecycleJourneyTests.cs
│   │   ├── MessagingJourneyTests.cs
│   │   └── TransportJourneyTests.cs
│   └── Security/
│       ├── IdorTests.cs
│       ├── RoleBypassTests.cs
│       └── TokenSecurityTests.cs
│
└── IAM.Tests.Unit/
    ├── IAM.Tests.Unit.csproj
    ├── Auth/
    │   ├── LoginHandlerTests.cs
    │   └── RegisterHandlerTests.cs
    └── ...
```

### 7.2 NuGet Packages

```xml
<!-- Unit Test Project -->
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.*" />
<PackageReference Include="xunit" Version="2.*" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.*" />
<PackageReference Include="FluentAssertions" Version="6.*" />
<PackageReference Include="NSubstitute" Version="5.*" />
<PackageReference Include="Bogus" Version="35.*" />
<PackageReference Include="AutoFixture" Version="4.*" />

<!-- Integration Test Project (additional) -->
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="8.*" />
<PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="8.*" />
<PackageReference Include="Testcontainers.MsSql" Version="3.*" />
<PackageReference Include="Microsoft.AspNetCore.SignalR.Client" Version="8.*" />

<!-- E2E Test Project (additional) -->
<PackageReference Include="WireMock.Net" Version="1.*" />
```

### 7.3 Test Configuration

```csharp
// IntegrationTestBase.cs
public abstract class IntegrationTestBase : IAsyncLifetime
{
    protected LivestockTradingModuleDbContext DbContext { get; private set; }

    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<LivestockTradingModuleDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        DbContext = new LivestockTradingModuleDbContext(options);
        await SeedTestData();
    }

    public async Task DisposeAsync()
    {
        await DbContext.DisposeAsync();
    }

    protected abstract Task SeedTestData();
}
```

### 7.4 CI/CD Integration

```yaml
# Add to Jenkinsfile
stage('Test') {
    steps {
        sh 'dotnet test LivestockTrading.sln --configuration Release --logger trx --results-directory TestResults'
        // Publish test results
        mstest testResultsFile: 'TestResults/**/*.trx'
    }
}
```

---

## 8. Sample Test Scenarios

### 8.1 Unit Test: Product Create Validator

```csharp
// Tests/LivestockTrading.Tests.Unit/Validators/Products/CreateValidatorTests.cs

public class ProductCreateValidatorTests
{
    private readonly RequestModel_Validator _validator = new();

    [Fact]
    public void Should_Fail_When_Title_Is_Empty()
    {
        var model = new RequestModel
        {
            Title = "",
            Slug = "test-product",
            CategoryId = Guid.NewGuid(),
            LocationId = Guid.NewGuid(),
            BasePrice = 100
        };

        var result = _validator.Validate(model);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Fact]
    public void Should_Fail_When_BasePrice_Is_Zero()
    {
        var model = new RequestModel
        {
            Title = "Test Product",
            Slug = "test-product",
            CategoryId = Guid.NewGuid(),
            LocationId = Guid.NewGuid(),
            BasePrice = 0
        };

        var result = _validator.Validate(model);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "BasePrice");
    }

    [Fact]
    public void Should_Fail_When_BasePrice_Is_Negative()
    {
        var model = new RequestModel
        {
            Title = "Test Product",
            Slug = "test-product",
            CategoryId = Guid.NewGuid(),
            LocationId = Guid.NewGuid(),
            BasePrice = -50
        };

        var result = _validator.Validate(model);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Should_Pass_When_SellerId_Is_Null()
    {
        // SellerId is optional - auto-created from current user
        var model = new RequestModel
        {
            Title = "Test Product",
            Slug = "test-product",
            CategoryId = Guid.NewGuid(),
            LocationId = Guid.NewGuid(),
            BasePrice = 100,
            SellerId = null
        };

        var result = _validator.Validate(model);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Fail_When_CategoryId_Is_Empty_Guid()
    {
        var model = new RequestModel
        {
            Title = "Test Product",
            Slug = "test-product",
            CategoryId = Guid.Empty,
            LocationId = Guid.NewGuid(),
            BasePrice = 100
        };

        var result = _validator.Validate(model);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CategoryId");
    }
}
```

### 8.2 Unit Test: TranslationHelper

```csharp
// Tests/LivestockTrading.Tests.Unit/Services/TranslationHelperTests.cs

public class TranslationHelperTests
{
    [Fact]
    public void Should_Return_Exact_Language_Match()
    {
        var json = "{\"en\":\"Livestock\",\"tr\":\"Hayvancilik\",\"de\":\"Viehzucht\"}";

        var result = TranslationHelper.GetTranslation(json, "tr", "Default");

        result.Should().Be("Hayvancilik");
    }

    [Fact]
    public void Should_Return_Case_Insensitive_Match()
    {
        var json = "{\"EN\":\"Livestock\",\"TR\":\"Hayvancilik\"}";

        var result = TranslationHelper.GetTranslation(json, "tr", "Default");

        result.Should().Be("Hayvancilik");
    }

    [Fact]
    public void Should_Fallback_To_English_When_Language_Not_Found()
    {
        var json = "{\"en\":\"Livestock\",\"tr\":\"Hayvancilik\"}";

        var result = TranslationHelper.GetTranslation(json, "fr", "Default");

        result.Should().Be("Livestock");
    }

    [Fact]
    public void Should_Return_Fallback_When_Null_Json()
    {
        var result = TranslationHelper.GetTranslation(null, "tr", "Fallback Value");

        result.Should().Be("Fallback Value");
    }

    [Fact]
    public void Should_Return_Fallback_When_Null_LanguageCode()
    {
        var json = "{\"en\":\"Livestock\"}";

        var result = TranslationHelper.GetTranslation(json, null, "Fallback");

        result.Should().Be("Fallback");
    }

    [Fact]
    public void Should_Return_Fallback_When_Invalid_Json()
    {
        var result = TranslationHelper.GetTranslation("not-json", "tr", "Fallback");

        result.Should().Be("Fallback");
    }

    [Fact]
    public void Should_Return_First_Available_When_No_English()
    {
        var json = "{\"de\":\"Viehzucht\",\"fr\":\"Betail\"}";

        var result = TranslationHelper.GetTranslation(json, "tr", "Default");

        result.Should().BeOneOf("Viehzucht", "Betail"); // First available
    }

    [Fact]
    public void Should_Handle_Empty_Translations_Object()
    {
        var json = "{}";

        var result = TranslationHelper.GetTranslation(json, "tr", "Default");

        result.Should().Be("Default");
    }
}
```

### 8.3 Unit Test: PermissionService

```csharp
// Tests/LivestockTrading.Tests.Unit/Authorization/PermissionServiceTests.cs

public class PermissionServiceTests
{
    private readonly PermissionService _sut;
    private readonly CurrentUserService _currentUserServiceMock;

    public PermissionServiceTests()
    {
        // Setup mock ArfBlocksDependencyProvider
        // that returns a CurrentUserService with configurable roles
        _currentUserServiceMock = Substitute.For<CurrentUserService>();
        var dp = CreateMockDependencyProvider(_currentUserServiceMock);
        _sut = new PermissionService(dp);
    }

    [Fact]
    public void RequireAdmin_Should_Throw_When_User_Is_Buyer()
    {
        SetUserRoles("Buyer");

        var act = () => _sut.RequireAdmin();

        act.Should().Throw<ArfBlocksVerificationException>();
    }

    [Fact]
    public void RequireAdmin_Should_Not_Throw_When_User_Is_Admin()
    {
        SetUserRoles("Admin");

        var act = () => _sut.RequireAdmin();

        act.Should().NotThrow();
    }

    [Fact]
    public void RequireModerator_Should_Pass_For_Admin()
    {
        SetUserRoles("Admin");

        var act = () => _sut.RequireModerator();

        act.Should().NotThrow();
    }

    [Fact]
    public void RequireModerator_Should_Pass_For_Moderator()
    {
        SetUserRoles("Moderator");

        var act = () => _sut.RequireModerator();

        act.Should().NotThrow();
    }

    [Fact]
    public void RequireModerator_Should_Fail_For_Seller()
    {
        SetUserRoles("Seller");

        var act = () => _sut.RequireModerator();

        act.Should().Throw<ArfBlocksVerificationException>();
    }

    [Fact]
    public void RequireAnyRole_Should_Pass_When_User_Has_One_Of_Roles()
    {
        SetUserRoles("Buyer", "Seller");

        var act = () => _sut.RequireAnyRole("Seller", "Admin");

        act.Should().NotThrow();
    }

    [Fact]
    public void RequireAnyRole_Should_Fail_When_User_Has_None_Of_Roles()
    {
        SetUserRoles("Buyer");

        var act = () => _sut.RequireAnyRole("Seller", "Admin");

        act.Should().Throw<ArfBlocksVerificationException>();
    }

    private void SetUserRoles(params string[] roles)
    {
        _currentUserServiceMock
            .GetUserRolesForModule("LivestockTrading")
            .Returns(roles.ToList());
    }
}
```

### 8.4 Integration Test: Product Create Handler

```csharp
// Tests/LivestockTrading.Tests.Integration/Handlers/ProductCreateHandlerTests.cs

public class ProductCreateHandlerTests : IntegrationTestBase
{
    [Fact]
    public async Task Should_Create_Product_With_Existing_Seller()
    {
        // Arrange
        var seller = await CreateTestSeller();
        var category = await CreateTestCategory();
        var location = await CreateTestLocation();

        var request = new RequestModel
        {
            Title = "Holstein Cow",
            Slug = "holstein-cow",
            CategoryId = category.Id,
            SellerId = seller.Id,
            LocationId = location.Id,
            BasePrice = 5000,
            Currency = "USD",
            StockQuantity = 10,
            IsInStock = true,
            Status = 0 // Draft
        };

        // Act
        var result = await ExecuteHandler<Handler, RequestModel, ResponseModel>(request);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Holstein Cow");
        result.SellerId.Should().Be(seller.Id);

        var dbProduct = await DbContext.Products.FindAsync(result.Id);
        dbProduct.Should().NotBeNull();
        dbProduct.Status.Should().Be(ProductStatus.Draft);
    }

    [Fact]
    public async Task Should_Auto_Create_Seller_When_SellerId_Is_Null()
    {
        // Arrange
        var category = await CreateTestCategory();
        var location = await CreateTestLocation();
        SetCurrentUser(Guid.NewGuid(), "John Doe");

        var request = new RequestModel
        {
            Title = "Angus Bull",
            Slug = "angus-bull",
            CategoryId = category.Id,
            SellerId = null, // Auto-create
            LocationId = location.Id,
            BasePrice = 8000,
            Currency = "USD"
        };

        // Act
        var result = await ExecuteHandler<Handler, RequestModel, ResponseModel>(request);

        // Assert
        result.SellerId.Should().NotBe(Guid.Empty);
        var seller = await DbContext.Sellers.FindAsync(result.SellerId);
        seller.Should().NotBeNull();
        seller.BusinessName.Should().Be("John Doe");
        seller.Status.Should().Be(SellerStatus.PendingVerification);
    }

    [Fact]
    public async Task Should_Reuse_Existing_Seller_For_Same_User()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var existingSeller = await CreateTestSeller(userId);
        var category = await CreateTestCategory();
        var location = await CreateTestLocation();
        SetCurrentUser(userId, "John Doe");

        var request = new RequestModel
        {
            Title = "Another Product",
            Slug = "another-product",
            CategoryId = category.Id,
            SellerId = null,
            LocationId = location.Id,
            BasePrice = 3000,
            Currency = "USD"
        };

        // Act
        var result = await ExecuteHandler<Handler, RequestModel, ResponseModel>(request);

        // Assert
        result.SellerId.Should().Be(existingSeller.Id);
    }

    protected override async Task SeedTestData()
    {
        // Minimal seed data if needed
        await Task.CompletedTask;
    }
}
```

### 8.5 Integration Test: DbValidationService

```csharp
// Tests/LivestockTrading.Tests.Integration/Services/DbValidationServiceTests.cs

public class DbValidationServiceTests : IntegrationTestBase
{
    private LivestockTradingModuleDbValidationService _sut;

    [Fact]
    public async Task ValidateProductSlugUnique_Should_Throw_When_Slug_Exists()
    {
        var product = new Product
        {
            Title = "Test",
            Slug = "existing-slug",
            CategoryId = Guid.NewGuid(),
            SellerId = Guid.NewGuid(),
            LocationId = Guid.NewGuid(),
            BasePrice = 100
        };
        DbContext.Products.Add(product);
        await DbContext.SaveChangesAsync();

        var act = () => _sut.ValidateProductSlugUnique("existing-slug", null, CancellationToken.None);

        await act.Should().ThrowAsync<ArfBlocksValidationException>();
    }

    [Fact]
    public async Task ValidateProductSlugUnique_Should_Pass_When_Excluding_Own_Id()
    {
        var product = new Product
        {
            Title = "Test",
            Slug = "my-slug",
            CategoryId = Guid.NewGuid(),
            SellerId = Guid.NewGuid(),
            LocationId = Guid.NewGuid(),
            BasePrice = 100
        };
        DbContext.Products.Add(product);
        await DbContext.SaveChangesAsync();

        var act = () => _sut.ValidateProductSlugUnique("my-slug", product.Id, CancellationToken.None);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ValidateProductSlugUnique_Should_Ignore_Soft_Deleted()
    {
        var product = new Product
        {
            Title = "Test",
            Slug = "deleted-slug",
            CategoryId = Guid.NewGuid(),
            SellerId = Guid.NewGuid(),
            LocationId = Guid.NewGuid(),
            BasePrice = 100,
            IsDeleted = true
        };
        DbContext.Products.Add(product);
        await DbContext.SaveChangesAsync();

        var act = () => _sut.ValidateProductSlugUnique("deleted-slug", null, CancellationToken.None);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ValidateSellerIsPendingVerification_Should_Throw_When_Already_Active()
    {
        var seller = new Seller
        {
            UserId = Guid.NewGuid(),
            BusinessName = "Test Farm",
            Status = SellerStatus.Active
        };
        DbContext.Sellers.Add(seller);
        await DbContext.SaveChangesAsync();

        var act = () => _sut.ValidateSellerIsPendingVerification(seller.Id, CancellationToken.None);

        await act.Should().ThrowAsync<ArfBlocksValidationException>();
    }

    [Fact]
    public async Task ValidateCategoryHasNoChildren_Should_Throw_When_Children_Exist()
    {
        var parent = new Category { Name = "Parent", Slug = "parent" };
        var child = new Category { Name = "Child", Slug = "child", ParentCategoryId = parent.Id };
        DbContext.Categories.AddRange(parent, child);
        await DbContext.SaveChangesAsync();

        var act = () => _sut.ValidateCategoryHasNoChildren(parent.Id, CancellationToken.None);

        await act.Should().ThrowAsync<ArfBlocksValidationException>();
    }
}
```

### 8.6 Security Test: IDOR Scenarios

```csharp
// Tests/LivestockTrading.Tests.E2E/Security/IdorTests.cs

public class IdorTests : E2ETestBase
{
    [Fact]
    public async Task Buyer_Should_Not_Access_Other_Users_Conversations()
    {
        // Arrange
        var userA = await CreateAndLoginUser("userA@test.com", "Buyer");
        var userB = await CreateAndLoginUser("userB@test.com", "Buyer");
        var seller = await CreateAndLoginUser("seller@test.com", "Seller");

        // User A creates conversation with seller
        var conversation = await CreateConversation(userA.Token, userA.Id, seller.Id);

        // Act: User B tries to access User A's messages
        var response = await Client.PostAsync("/livestocktrading/Messages/All",
            CreateJsonContent(new { conversationId = conversation.Id }, userB.Token));

        // Assert: Should not return User A's messages
        var messages = await ParseResponse<List<MessageResponse>>(response);
        messages.Should().BeEmpty();
    }

    [Fact]
    public async Task Seller_Should_Not_Update_Another_Sellers_Product()
    {
        // Arrange
        var sellerA = await CreateSeller("Seller A");
        var sellerB = await CreateSeller("Seller B");
        var productByA = await CreateProduct(sellerA.Token, sellerA.SellerId);

        // Act: Seller B tries to update Seller A's product
        var response = await Client.PostAsync("/livestocktrading/Products/Update",
            CreateJsonContent(new
            {
                id = productByA.Id,
                title = "Hijacked Product",
                slug = productByA.Slug,
                categoryId = productByA.CategoryId,
                locationId = productByA.LocationId,
                basePrice = 1
            }, sellerB.Token));

        // Assert: Should be rejected
        response.StatusCode.Should().NotBe(200);
    }

    [Fact]
    public async Task Buyer_Should_Not_Approve_Products()
    {
        // Arrange
        var buyer = await CreateAndLoginUser("buyer@test.com", "Buyer");
        var product = await CreatePendingProduct();

        // Act: Buyer tries to approve
        var response = await Client.PostAsync("/livestocktrading/Products/Approve",
            CreateJsonContent(new { id = product.Id }, buyer.Token));

        // Assert: Should fail with authorization error
        response.StatusCode.Should().Be(403);
    }
}
```

### 8.7 E2E Journey Test: Product Lifecycle

```csharp
// Tests/LivestockTrading.Tests.E2E/Journeys/ProductLifecycleJourneyTests.cs

public class ProductLifecycleJourneyTests : E2ETestBase
{
    [Fact]
    public async Task Complete_Product_Lifecycle()
    {
        // Step 1: Register seller
        var seller = await RegisterUser("seller@farm.com", "password123");
        var sellerToken = await Login("seller@farm.com", "password123");

        // Step 2: Create seller profile
        var sellerProfile = await Post<SellerResponse>("/livestocktrading/Sellers/Create", new
        {
            userId = seller.Id,
            businessName = "Green Valley Farm",
            businessType = "Bireysel",
            email = "seller@farm.com",
            phone = "+905551234567"
        }, sellerToken);
        sellerProfile.Status.Should().Be(0); // PendingVerification

        // Step 3: Admin verifies seller
        var adminToken = await Login("m.mustafaocak@gmail.com", "adminpass");
        await Post("/livestocktrading/Sellers/Verify", new
        {
            id = sellerProfile.Id
        }, adminToken);

        // Step 4: Create location
        var location = await Post<LocationResponse>("/livestocktrading/Locations/Create", new
        {
            name = "Farm Location",
            city = "Ankara",
            countryCode = "TR",
            addressLine1 = "Cankaya, Ankara"
        }, sellerToken);

        // Step 5: Create product
        var product = await Post<ProductResponse>("/livestocktrading/Products/Create", new
        {
            title = "Holstein Dairy Cow",
            slug = $"holstein-dairy-cow-{DateTime.UtcNow.Ticks}",
            description = "High-yield dairy cow, 3 years old",
            categoryId = TestData.LivestockCategoryId,
            sellerId = sellerProfile.Id,
            locationId = location.Id,
            basePrice = 25000,
            currency = "TRY",
            stockQuantity = 5,
            isInStock = true,
            condition = 0 // New
        }, sellerToken);
        product.Status.Should().Be(0); // Draft

        // Step 6: Submit for approval
        await Post("/livestocktrading/Products/Update", new
        {
            id = product.Id,
            title = product.Title,
            slug = product.Slug,
            categoryId = product.CategoryId,
            locationId = product.LocationId,
            basePrice = product.BasePrice,
            status = 1 // PendingApproval
        }, sellerToken);

        // Step 7: Admin approves
        await Post("/livestocktrading/Products/Approve", new
        {
            id = product.Id
        }, adminToken);

        // Step 8: Verify product is searchable
        var searchResults = await Post<PagedResponse<ProductSearchResponse>>(
            "/livestocktrading/Products/Search", new
        {
            query = "Holstein",
            countryCode = "TR",
            pageRequest = new { page = 1, pageSize = 10 }
        }, sellerToken);

        searchResults.Items.Should().Contain(p => p.Id == product.Id);

        // Step 9: Buyer views product
        var buyerToken = await CreateBuyerAndLogin();
        var detail = await Post<ProductDetailResponse>(
            "/livestocktrading/Products/Detail", new { id = product.Id }, buyerToken);
        detail.Title.Should().Be("Holstein Dairy Cow");

        // Step 10: Buyer favorites product
        await Post("/livestocktrading/FavoriteProducts/Create", new
        {
            productId = product.Id,
            userId = TestData.BuyerUserId
        }, buyerToken);
    }
}
```

### 8.8 ChatHub Test

```csharp
// Tests/LivestockTrading.Tests.Integration/Hubs/ChatHubTests.cs

public class ChatHubTests : IAsyncLifetime
{
    private HubConnection _connection1;
    private HubConnection _connection2;

    [Fact]
    public async Task Users_Should_Receive_Typing_Indicators()
    {
        // Arrange
        var conversationId = Guid.NewGuid();
        var receivedIndicator = new TaskCompletionSource<dynamic>();

        _connection2.On("TypingIndicator", (dynamic data) =>
        {
            receivedIndicator.SetResult(data);
        });

        await _connection1.InvokeAsync("JoinConversation", conversationId);
        await _connection2.InvokeAsync("JoinConversation", conversationId);

        // Act
        await _connection1.InvokeAsync("SendTypingIndicator", conversationId, true);

        // Assert
        var result = await receivedIndicator.Task.WaitAsync(TimeSpan.FromSeconds(5));
        // Connection2 should receive typing indicator from Connection1
    }

    [Fact]
    public async Task User_Should_Appear_Online_After_Connection()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var onlineReceived = new TaskCompletionSource<Guid>();

        _connection2.On<dynamic>("UserOnline", (data) =>
        {
            onlineReceived.SetResult(data.userId);
        });

        // Act: User1 connects (triggers OnConnectedAsync)
        // The connection was already established in InitializeAsync

        // Assert: Check via GetOnlineUsers
        var onlineUsers = await _connection2.InvokeAsync<List<OnlineUserInfo>>(
            "GetOnlineUsers", new List<Guid> { userId });

        // Note: This test demonstrates the pattern; actual setup requires
        // configuring JWT claims for the test connections
    }

    public async Task InitializeAsync()
    {
        // Setup SignalR test connections with mock JWT
        _connection1 = CreateTestHubConnection("user1-token");
        _connection2 = CreateTestHubConnection("user2-token");
        await _connection1.StartAsync();
        await _connection2.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _connection1.DisposeAsync();
        await _connection2.DisposeAsync();
    }
}
```

### 8.9 Domain Error Uniqueness Test

```csharp
// Tests/LivestockTrading.Tests.Unit/Domain/DomainErrorUniquenessTests.cs

public class DomainErrorUniquenessTests
{
    [Fact]
    public void All_Error_Properties_Should_Have_Unique_Names()
    {
        var errorType = typeof(LivestockTradingDomainErrors);
        var nestedTypes = errorType.GetNestedTypes();

        var allPropertyNames = nestedTypes
            .SelectMany(t => t.GetProperties(BindingFlags.Public | BindingFlags.Static))
            .Select(p => p.Name)
            .ToList();

        var duplicates = allPropertyNames
            .GroupBy(n => n)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        duplicates.Should().BeEmpty(
            $"Duplicate error property names found: {string.Join(", ", duplicates)}. " +
            "This will cause TypeScript export errors.");
    }

    [Fact]
    public void All_Error_Properties_Should_Start_With_Entity_Prefix()
    {
        var errorType = typeof(LivestockTradingDomainErrors);
        var nestedTypes = errorType.GetNestedTypes()
            .Where(t => t.Name != "CommonErrors" && t.Name != "AuthorizationErrors");

        foreach (var type in nestedTypes)
        {
            var entityPrefix = type.Name.Replace("Errors", "");
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Static);

            foreach (var prop in properties)
            {
                // Allow known exceptions like "SuspensionReasonRequired"
                prop.Name.Should().StartWith(entityPrefix,
                    $"Property '{type.Name}.{prop.Name}' should start with '{entityPrefix}'");
            }
        }
    }
}
```

### 8.10 Mapper Enum Casting Test

```csharp
// Tests/LivestockTrading.Tests.Unit/Mappers/ProductMapperTests.cs

public class ProductMapperTests
{
    private readonly Mapper _sut = new();

    [Theory]
    [InlineData(0, ProductStatus.Draft)]
    [InlineData(1, ProductStatus.PendingApproval)]
    [InlineData(2, ProductStatus.Active)]
    [InlineData(7, ProductStatus.Rejected)]
    public void MapToEntity_Should_Cast_Status_Int_To_Enum(int input, ProductStatus expected)
    {
        var request = CreateValidRequest();
        request.Status = input;

        var entity = _sut.MapToEntity(request);

        entity.Status.Should().Be(expected);
    }

    [Theory]
    [InlineData(ProductStatus.Draft, 0)]
    [InlineData(ProductStatus.Active, 2)]
    [InlineData(ProductStatus.Rejected, 7)]
    public void MapToResponse_Should_Cast_Enum_To_Int(ProductStatus input, int expected)
    {
        var entity = CreateValidEntity();
        entity.Status = input;

        var response = _sut.MapToResponse(entity);

        response.Status.Should().Be(expected);
    }

    [Fact]
    public void MapToResponse_Should_Handle_Null_Navigation_Properties()
    {
        var entity = CreateValidEntity();
        entity.Category = null;
        entity.Brand = null;
        entity.Seller = null;
        entity.Location = null;

        var act = () => _sut.MapToResponse(entity);

        act.Should().NotThrow();
    }

    [Fact]
    public void MapToResponse_Should_Preserve_Decimal_Precision()
    {
        var entity = CreateValidEntity();
        entity.BasePrice = 1234.5678m;

        var response = _sut.MapToResponse(entity);

        response.BasePrice.Should().Be(1234.5678m);
    }
}
```

---

## Summary

### Key Metrics

| Metric | Current | Target (3 months) | Target (6 months) |
|--------|---------|--------------------|--------------------|
| Automated Test Projects | 0 | 2 (Unit + Integration) | 3 (+E2E) |
| Unit Tests | 0 | ~200 | ~400 |
| Integration Tests | 0 | ~80 | ~150 |
| E2E Tests | 0 | 0 | ~50 |
| Code Coverage | 0% | ~40% | ~65% |
| CI Test Execution | No | Yes (Unit only) | Yes (All) |

### Top 5 Immediate Actions

1. **Create `LivestockTrading.Tests.Unit` project** with xUnit + FluentAssertions + NSubstitute
2. **Write PermissionService tests** -- authorization is the #1 security boundary
3. **Write Product Create/Update Validator tests** -- core business validation
4. **Write IDOR security tests** for Seller/Product/Conversation access
5. **Add DomainError uniqueness test** to prevent TypeScript export failures

### Risk Summary

The platform has **zero automated tests** protecting a codebase with **259+ API endpoints**, **28 domain entities**, and **real-time messaging via SignalR**. The most critical gaps are:

- **Authorization bypass**: ChatHub has no conversation membership checks
- **IDOR vulnerabilities**: Multiple endpoints don't verify resource ownership
- **Status manipulation**: Product status can be set directly by clients
- **No rate limiting**: All endpoints (including auth) are unprotected
- **No state machine enforcement**: Entity status transitions are not validated
