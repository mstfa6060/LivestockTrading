# Karar 6 — API Contract Çıkarma Stratejisi

**Status:** FINAL — 10 madde
**Karar tarihi:** Planning session #6

## İlişkili Kararlar

- **Üst:** [Karar 5 modülleri](05-modules/), [02-modules-list.md](02-modules-list.md), [03-domain-patterns.md](03-domain-patterns.md)
- **Alt:** [Karar 7 — Operations](07-operations.md) (CI/CD, OpenAPI artifact publish)

---

# Madde 1 — Frontend MSW vs Domain-First Kaynak Seçimi

## Karar: **Hibrit — Domain-First Authoritative + MSW Validation Oracle**

| | A. MSW-First | B. Domain-First | C. Hybrid (Karar) |
|---|---|---|---|
| Authoritative source | Frontend MSW handlers | Backend OpenAPI spec | **Backend OpenAPI** |
| Frontend mock cross-check | Frontend ekliyor | Backend ekliyor | Backend OpenAPI ↔ MSW her commit'te diff |
| Type generation | Frontend manuel | Backend → OpenAPI → frontend codegen | Aynı |
| Change initiation | Frontend developer | Backend developer | Domain change → backend → frontend regenerates |
| Sync mekanizması | Manuel | Manuel | **Automated CI check** |

**Gerekçe:** Karar 5 zaten domain-first yapıldı; MSW production'a gitmiyor, sadece dev mock.

## Workflow

```
Backend developer endpoint ekliyor:
   1. Code change (handler + validator + DTO)
   2. CI: dotnet run --project Tools/OpenApiGen → openapi.json artifact
   3. CI: diff against previous → contract change detected
   4. CI: openapi-msw-check.ts → MSW handlers eşleşiyor mu?
       - Field eklendi → MSW optional add (auto-PR Faz 2)
       - Field silindi → manual review (breaking)
       - Path değişti → manual review

Frontend developer yeni endpoint istiyor:
   1. /docs/api-contract/proposals/{topic}.md propose
   2. Backend reviewer assesses domain alignment
   3. Approved → backend implement
```

## MSW Cross-Check Document

`_docs/api-contract/msw-cross-check.md` — granular backend vs aggregated frontend.

---

# Madde 2 — OpenAPI / Scalar Entegrasyonu

## Stack

| Bileşen | Seçim |
|---|---|
| OpenAPI version | 3.1 |
| Generator | `Microsoft.AspNetCore.OpenApi` (built-in .NET 10) |
| UI | **Scalar** (modern, dark mode, OpenAPI 3.1 native) |
| Schema enrichment | `[ProducesResponseType]` + XML doc + custom transformers |

## Per-Module OpenAPI Doc

```
/openapi/identity.json
/openapi/accounts.json
/openapi/carrier.json
/openapi/catalog.json
/openapi/listings.json
/openapi/marketplace.json
/openapi/messaging.json
/openapi/notifications.json
/openapi/subscription.json
/openapi/admin.json
/openapi/all.json          # combined
```

## Setup Pattern

```csharp
// Program.cs
builder.Services.AddOpenApi("identity", options => 
{
    options.ShouldInclude = description =>
        description.RelativePath?.StartsWith("identity/") == true
        || description.RelativePath?.StartsWith("connect/") == true;
    options.AddDocumentTransformer<IdentityDocTransformer>();
    options.AddSchemaTransformer<EnumStringTransformer>();
});
// ... 10 modül için

builder.Services.AddOpenApi("all", options => { /* No filter */ });

app.MapOpenApi("/openapi/{documentName}.json");

app.MapScalarApiReference(options =>
{
    options.WithTheme(ScalarTheme.Default)
           .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Fetch)
           .WithDownloadButton(true);
});
// /scalar/identity, /scalar/accounts, ..., /scalar (all)
```

## Common Document Transformer

```csharp
public sealed class CommonDocTransformer : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, ...)
    {
        document.Servers =
        [
            new() { Url = "https://livestock-trading.com/api", Description = "Production" },
            new() { Url = "https://staging.livestock-trading.com/api", Description = "Staging" },
            new() { Url = "http://localhost:5000", Description = "Local dev" },
        ];
        
        document.Components.SecuritySchemes["BearerAuth"] = new()
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "JWT obtained from POST /identity/auth/login or /connect/token"
        };
        
        document.SecurityRequirements.Add(new()
        {
            [new OpenApiSecurityScheme { Reference = new() { Type = ReferenceType.SecurityScheme, Id = "BearerAuth" } }] = []
        });
    }
}
```

## Versioning Strategy

| Versiyon | Path | Durum |
|---|---|---|
| v1 | `/openapi/v1/{module}.json` | Faz 1 default |
| v2 | `/openapi/v2/{module}.json` | Faz 2 breaking changes |

URL prefix versioning. Faz 1 prefix yok (`/identity/...`); v2 geçişte `/v2/identity/...`.

## CI Pipeline

```yaml
jobs:
  generate-openapi:
    steps:
      - run: dotnet run --project Tools/OpenApiGen -- --output ./openapi-artifacts
      - uses: actions/upload-artifact@v4
        with: { name: openapi-${{ github.sha }}, path: ./openapi-artifacts/ }
      
      - name: Compare against base
        run: # openapi-diff, comment PR if breaking
  
  contract-test:
    needs: generate-openapi
    steps:
      - name: Validate MSW
        run: # npx openapi-msw-check, fail if drift > threshold
```

---

# Madde 3 — Cursor Pagination Shared Utility

## Cursor Format

**Karar: Opaque Base64-Encoded JSON.**

```json
{ "k": ["created_at", "id"], "v": ["2026-05-10T12:00:00.000Z", "01J5C..."] }
```

Base64 URL-safe: `eyJrIjpbI...`.

**Tampering koruması:** Faz 1 soft (decode visible); Faz 2 HMAC opsiyonel (Backlog #174).

## `CursorPage<T>` Response Shape

```csharp
namespace Shared.Pagination;

public sealed record CursorPage<T>(
    IReadOnlyList<T> Items,
    string? NextCursor,
    bool HasMore,
    int? TotalCount = null);   // admin endpoints'te dolu, hot path null

public sealed record CursorRequest(
    string? Cursor,
    int PageSize = 50,
    int MaxPageSize = 200);

public sealed record CursorState(string[] Keys, object[] Values);
```

`PrevCursor` yok — backend hesaplamıyor (browser back navigation client'ın işi).

## Sort Key Stability

**Kural:** Composite `(primary_sort_key, id)`.

| Endpoint | Primary Sort | Tiebreaker | Direction |
|---|---|---|---|
| `/listings?sort=newest` | `created_at` | `id` | DESC |
| `/listings?sort=price-low` | `price.amount` | `id` | ASC |
| `/listings?sort=distance` | `ST_Distance(...)` | `id` | ASC |
| `/me/conversations/{id}/messages` | `created_at` | `id` | DESC veya ASC |
| `/admin/users` | `created_at` | `id` | DESC |

## IQueryable Extension

```csharp
public static class QueryablePaginationExtensions
{
    public static async Task<CursorPage<T>> PaginateByCursorAsync<T, TPrimary>(
        this IQueryable<T> query,
        CursorRequest request,
        Expression<Func<T, TPrimary>> primarySort,
        Expression<Func<T, Guid>> idSelector,
        SortDirection direction = SortDirection.Descending,
        CancellationToken ct = default)
        where TPrimary : struct, IComparable<TPrimary>
    {
        var pageSize = Math.Min(Math.Max(request.PageSize, 1), request.MaxPageSize);
        
        if (!string.IsNullOrEmpty(request.Cursor))
        {
            var state = CursorCodec.Decode(request.Cursor);
            // Apply tuple comparison filter
        }
        
        query = direction == SortDirection.Descending
            ? query.OrderByDescending(primarySort).ThenByDescending(idSelector)
            : query.OrderBy(primarySort).ThenBy(idSelector);
        
        var items = await query.Take(pageSize + 1).ToListAsync(ct);
        var hasMore = items.Count > pageSize;
        if (hasMore) items.RemoveAt(items.Count - 1);
        
        string? nextCursor = null;
        if (hasMore && items.Count > 0)
        {
            var last = items[^1];
            nextCursor = CursorCodec.Encode(new CursorState(
                Keys: ["primary", "id"],
                Values: [primarySort.Compile()(last)!, idSelector.Compile()(last)]));
        }
        
        return new CursorPage<T>(items, nextCursor, hasMore);
    }
}

public static CursorPage<TDest> Map<TSrc, TDest>(
    this CursorPage<TSrc> src, Func<TSrc, TDest> mapper) =>
    new(src.Items.Select(mapper).ToList(), src.NextCursor, src.HasMore, src.TotalCount);
```

## CursorCodec

```csharp
public static class CursorCodec
{
    public static string Encode(CursorState state)
    {
        var json = JsonSerializer.SerializeToUtf8Bytes(state, Options);
        return Convert.ToBase64String(json)
            .Replace('+', '-').Replace('/', '_').TrimEnd('=');
    }
    
    public static CursorState Decode(string cursor)
    {
        try
        {
            var b64 = cursor.Replace('-', '+').Replace('_', '/');
            switch (b64.Length % 4) { case 2: b64 += "=="; break; case 3: b64 += "="; break; }
            var bytes = Convert.FromBase64String(b64);
            return JsonSerializer.Deserialize<CursorState>(bytes, Options) 
                ?? throw new InvalidCursorException();
        }
        catch (Exception ex) when (ex is FormatException or JsonException)
        {
            throw new InvalidCursorException("Cursor decode failed", ex);
        }
    }
}

public sealed class InvalidCursorException : Exception { ... }
```

API'de invalid cursor → 400 `{ error: "INVALID_CURSOR" }`.

## Query String Convention

```
GET /listings?cursor=eyJ...&pageSize=50&sort=newest
GET /listings?pageSize=50&sort=newest         # first page
```

---

# Madde 4 — Locale Resolution Middleware

## 6-Step Priority

```
1. URL query param  ?locale=tr           (highest — explicit override)
2. User.Preferences.Locale               (authenticated user setting)
3. Cookie i18nextLng                     (frontend i18next default)
4. Accept-Language header                (browser/client default)
5. IP-based country → DefaultLocale      (Catalog.Country.DefaultLanguageCode)
6. Fallback "en"                         (final)
```

## Middleware

```csharp
namespace Shared.Localization;

public sealed class LocaleResolutionMiddleware
{
    public async Task InvokeAsync(HttpContext ctx, ICurrentUserService currentUser)
    {
        var locale = await ResolveAsync(ctx, currentUser, ctx.RequestAborted);
        ctx.Items[LocaleContextKey] = locale;
        
        // ASP.NET Core localization (FluentValidation error messages)
        var culture = new CultureInfo(locale.Value);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        
        await _next(ctx);
    }
    
    private async Task<LanguageCode> ResolveAsync(HttpContext ctx, ICurrentUserService currentUser, CancellationToken ct)
    {
        // 1. Query
        if (ctx.Request.Query.TryGetValue("locale", out var qsLocale) 
            && await IsValidLocaleAsync(qsLocale, ct))
            return LanguageCode.Parse(qsLocale!);
        
        // 2. Authenticated user pref
        if (currentUser.IsAuthenticated)
        {
            var userLocale = currentUser.GetLocale();
            if (await IsValidLocaleAsync(userLocale, ct))
                return LanguageCode.Parse(userLocale);
        }
        
        // 3. Cookie
        foreach (var cookieName in new[] { "i18nextLng", "locale" })
        {
            if (ctx.Request.Cookies.TryGetValue(cookieName, out var cookieLocale)
                && await IsValidLocaleAsync(cookieLocale, ct))
                return LanguageCode.Parse(cookieLocale);
        }
        
        // 4. Accept-Language
        var acceptLang = ctx.Request.Headers.AcceptLanguage.ToString();
        foreach (var lang in ParseAcceptLanguage(acceptLang))
        {
            if (await IsValidLocaleAsync(lang, ct))
                return LanguageCode.Parse(lang);
        }
        
        // 5. IP-based country
        var ip = ctx.Connection.RemoteIpAddress?.ToString();
        if (!string.IsNullOrEmpty(ip))
        {
            var geo = await _geoIp.LookupAsync(ip, ct);
            if (geo?.CountryCode is not null)
            {
                var country = await _catalog.GetCountryAsync(geo.CountryCode, ct);
                if (country?.DefaultLanguageCode is { } defaultLang
                    && await IsValidLocaleAsync(defaultLang, ct))
                    return LanguageCode.Parse(defaultLang);
            }
        }
        
        // 6. Fallback
        return LanguageCode.Parse("en");
    }
    
    public const string LocaleContextKey = "RequestedLocale";
}
```

**Pipeline order:**

```csharp
app.UseAuthentication();        // user claims available
app.UseAuthorization();
app.UseLocaleResolution();      // AFTER auth (user.GetLocale çağırabilsin)
```

## `ICurrentLocaleService`

```csharp
public interface ICurrentLocaleService
{
    LanguageCode GetLocale();
    bool IsRtl();
    string GetUiCulture();        // örn. "tr-TR"
}

services.AddScoped<ICurrentLocaleService, CurrentLocaleService>();
```

## Cross-Modül Kullanım

```csharp
// TranslationHelper
var resolved = TranslationHelper.Resolve(translations, _locale.GetLocale().Value);

// Notifications template
var template = await _templates.GetAsync(templateKey, _locale.GetLocale(), channel, ct);

// System message render
var rendered = await _systemMessageRenderer.RenderAsync(systemKey, contextJson, _locale.GetLocale(), ct);

// FluentValidation error messages (auto via CultureInfo)
```

---

# Madde 5 — Admin Endpoint Route Convention

## Path Convention

| Path Pattern | Anlam | Auth |
|---|---|---|
| `/{module}/...` | Public | Genelde public read |
| `/me/...` | Authenticated user kendi kaynakları | `[Authorize]` |
| `/{module}/me/...` | Modül-spesifik "my X" | `[Authorize]` |
| `/admin/{module}/...` | Admin/moderator panel | `[Authorize(Roles="admin,moderator")]` |
| `/admin/superadmin/...` | Sadece admin role | `[Authorize(Roles="admin")]` |
| `/connect/...` | OpenIddict standard | Spec |
| `/.well-known/...` | OIDC discovery | Public |
| `/{module}/webhooks/...` | Provider webhook | Signature-verified, AllowAnonymous |
| `WS /{module}/hub` | SignalR | Bearer JWT query string |

## Per-Modül Admin Endpoint Yerleşimi

`/admin/{module}/...` endpoint'leri **ilgili modülün** `.Application/Features/Admin/` klasöründe yaşar.

**Admin modülü** sadece kendi 6 endpoint grubunu (dashboard, feature flags, scheduled reports, system versions, impersonation, audit) barındırıyor — `/admin/dashboard`, `/admin/feature-flags`, vb.

Cross-module aksiyonlar için Admin modülü **`IAdminXCommands` sync** çağırır.

## Authorization Pattern

```csharp
// Route group
var adminCatalogGroup = app.MapGroup("/admin/catalog")
    .RequireAuthorization(policy => policy.RequireRole("admin", "moderator"))
    .WithTags("Admin: Catalog");

adminCatalogGroup.MapPost("/categories", CreateCategoryEndpoint.Handle);

// Specific endpoint stricter
adminIdentityGroup
    .MapPost("/users/{id}/grant-role", GrantRoleEndpoint.Handle)
    .RequireAuthorization(p => p.RequireRole("admin"));   // moderator yapamaz
```

## NoImpersonationOnAdminRoutes Defense-in-Depth

```csharp
public sealed class NoImpersonationOnAdminRoutes : IAuthorizationHandler
{
    public Task HandleAsync(AuthorizationHandlerContext ctx)
    {
        if (ctx.Resource is HttpContext http
            && http.Request.Path.StartsWithSegments("/admin"))
        {
            var imp = http.User.FindFirst("imp")?.Value == "true";
            if (imp)
            {
                ctx.Fail(new AuthorizationFailureReason(this, 
                    "Admin routes cannot be accessed during impersonation"));
            }
        }
        return Task.CompletedTask;
    }
}
```

Impersonation context'inde `/admin/*` reddedilir.

## OpenAPI Tag Grouping

```csharp
document.Tags =
[
    new() { Name = "Auth", Description = "Login, register, refresh" },
    new() { Name = "Profile" },
    new() { Name = "Sessions" },
    new() { Name = "OAuth" },
    new() { Name = "Admin: Users", Description = "User administration (requires admin)" }
];
```

Scalar UI ayrı bölüm — visual separation.

## Cross-Modül Admin Endpoint Sayım

| Modül | Admin Endpoint | Konum |
|---|---|---|
| Catalog | 17 | Catalog modülü |
| Identity | 9 | Identity |
| Accounts | 16 | Accounts |
| Carrier | 5 | Carrier |
| Subscription | 15 | Subscription |
| Listings | 5 | Listings |
| Marketplace | 8 | Marketplace |
| Messaging | 3 | Messaging |
| Notifications | 5 | Notifications |
| Admin (own data) | 27 | Admin |
| **Toplam** | **110** | |

---

# Madde 6 — Realtime Push Catalog (80 Event Full Matrix)

## Hub Topology

| Hub | Modül | Scope | Auth |
|---|---|---|---|
| `/messaging/hub` | Messaging | Conversation-scoped | JWT query string |
| `/notifications/hub` | Notifications | User-scoped + role groups | JWT query string |

**Faz 1:** Single instance, in-memory groups. **Faz 2:** Redis backplane.

## Client Group Naming

| Group | Sahibi | Üye | Kullanım |
|---|---|---|---|
| `user:{userId}` | Notifications + Messaging | Bir user'ın tüm connection'ları | User-targeted notifications |
| `conv:{conversationId}` | Messaging | Conversation 2 katılımcısı | Message broadcast, typing |
| `admin-moderation` | Notifications | `moderator` veya `admin` role | Moderation queue updates |
| `superadmin` | Notifications | Sadece `admin` role | System-critical alerts |

## Server → Client Event Name Convention

**PascalCase, single JSON payload object.**

## 80 Public Event Mapping (FULL MATRIX)

### Identity (9)

| Event | Hub | Group | Client Event | Not |
|---|---|---|---|---|
| `UserRegistered` | None | — | — | Self-event; welcome mail yeter |
| `UserEmailVerified` | Notifications | `user:{id}` | `NotificationReceived` | "Email doğrulandı" |
| `UserPasswordChanged` | Notifications | `user:{id}` | `NotificationReceived` | Security alert |
| `UserSuspended` | Notifications | `user:{id}` | `AccountStatusChanged` | Kritik — UI banner |
| `UserReactivated` | Notifications | `user:{id}` | `AccountStatusChanged` | |
| `UserDeleted` | None | — | — | User offline zaten |
| `EmailBounced` | None | — | — | Producer-side internal |
| `SmsDeliveryFailed` | None | — | — | Aynı |
| `PushTokenInvalidated` | None | — | — | Aynı |

### Accounts (11)

| Event | Hub | Group | Client Event |
|---|---|---|---|
| `SellerOnboardingSubmitted` | Notifications | `user:{sellerUserId}` + `admin-moderation` | `NotificationReceived` |
| `SellerVerified` | Notifications | `user:{sellerUserId}` | `NotificationReceived` ("approved!") |
| `SellerSuspended` | Notifications | `user:{sellerUserId}` | `AccountStatusChanged` |
| `SellerReactivated` | Notifications | `user:{sellerUserId}` | `AccountStatusChanged` |
| `VetVerified` | Notifications | `user:{vetUserId}` | `NotificationReceived` |
| `VetSuspended` | Notifications | `user:{vetUserId}` | `AccountStatusChanged` |
| `VetReactivated` | Notifications | `user:{vetUserId}` | `AccountStatusChanged` |
| `ReviewWritten` | Notifications | `user:{reviewedUserId}` | `NotificationReceived` |
| `SellerCertificationExpired` | Notifications | `user:{sellerUserId}` | `NotificationReceived` |

### Carrier (8)

| Event | Hub | Group | Client Event |
|---|---|---|---|
| `CarrierOnboardingSubmitted` | Notifications | `user:{carrierUserId}` + `admin-moderation` | `NotificationReceived` |
| `CarrierVerified` | Notifications | `user:{carrierUserId}` | `NotificationReceived` |
| `CarrierSuspended` | Notifications | `user:{carrierUserId}` | `AccountStatusChanged` |
| `CarrierReactivated` | Notifications | `user:{carrierUserId}` | `AccountStatusChanged` |
| `CarrierShipmentCreated` | Notifications | `user:{sellerUserId}` + `user:{buyerUserId}` | `NotificationReceived` |
| `CarrierShipmentPickedUp` | Notifications | `user:{buyerUserId}` | `NotificationReceived` ("kargo yola çıktı") |
| `CarrierShipmentDelivered` | Notifications | `user:{buyerUserId}` | `NotificationReceived` (action: confirm receipt) |
| `CarrierShipmentCancelled` | Notifications | `user:{sellerUserId}` + `user:{buyerUserId}` | `NotificationReceived` |

### Catalog (7)

| Event | Hub | Group | Client Event | Not |
|---|---|---|---|---|
| `CategoryDeactivated` | None | — | — | System-level |
| `CategoryReactivated` | None | — | — | |
| `BreedDeactivated` | None | — | — | |
| `BreedReactivated` | None | — | — | |
| `BrandApproved` | Notifications | `user:{suggesterUserId}` (varsa) | `NotificationReceived` | Sadece seller-suggested ise |
| `BrandDeactivated` | None | — | — | System change |
| `BrandReactivated` | None | — | — | |

### Listings (11)

| Event | Hub | Group | Client Event |
|---|---|---|---|
| `ListingSubmittedForReview` | Notifications | `admin-moderation` | `NotificationReceived` (queue) |
| `ListingApproved` | Notifications | `user:{sellerUserId}` | `ListingStatusChanged` |
| `ListingRejected` | Notifications | `user:{sellerUserId}` | `ListingStatusChanged` |
| `ListingPaused` | Notifications | `user:{sellerUserId}` | `ListingStatusChanged` |
| `ListingResumed` | Notifications | `user:{sellerUserId}` | `ListingStatusChanged` |
| `ListingSold` | Notifications | `user:{sellerUserId}` + `user:{favoriteUserId}*` | `ListingStatusChanged` |
| `ListingExpired` | Notifications | `user:{sellerUserId}` | `ListingStatusChanged` |
| `ListingDeleted` | Notifications | `user:{favoriteUserId}*` | `ListingRemoved` (cleanup hint) |
| `ListingPriceChanged` | Notifications | `user:{favoriteUserId}*` | `NotificationReceived` ("favori fiyat düştü") |
| `ListingReportFiled` | Notifications | `admin-moderation` | `NotificationReceived` |
| `SavedSearchMatchFound` | Notifications | `user:{savedSearchOwnerUserId}` | `NotificationReceived` |

`*` Favorite owner: Notifications consumer Favorites tablosu sorgular, her sahibe push.

### Marketplace (13)

| Event | Hub | Group | Client Event |
|---|---|---|---|
| `OfferSubmitted` | Notifications | `user:{sellerUserId}` | `NotificationReceived` |
| `OfferAccepted` | Notifications | `user:{buyerUserId}` | `OfferStatusChanged` + `NotificationReceived` |
| `OfferRejected` | Notifications | `user:{buyerUserId}` | `OfferStatusChanged` |
| `OfferWithdrawn` | Notifications | `user:{sellerUserId}` | `OfferStatusChanged` |
| `OfferCounterProposed` | Notifications | `user:{recipientUserId}` | `OfferStatusChanged` |
| `OfferExpired` | Notifications | `user:{buyerUserId}` + `user:{sellerUserId}` | `OfferStatusChanged` |
| `DealPaymentConfirmed` | Notifications | `user:{sellerUserId}` | `DealStatusChanged` + `NotificationReceived` |
| `DealShipmentStarted` | Notifications | `user:{buyerUserId}` | `DealStatusChanged` |
| `DealDelivered` | Notifications | `user:{buyerUserId}` | `DealStatusChanged` (urgent: confirm) |
| `DealCompleted` | Notifications | `user:{buyerUserId}` + `user:{sellerUserId}` | `DealStatusChanged` + `NotificationReceived` (review prompt) |
| `DealDisputed` | Notifications | `admin-moderation` + opposite party | `NotificationReceived` |
| `DealResolved` | Notifications | both parties | `DealStatusChanged` + `NotificationReceived` |
| `DealCancelled` | Notifications | both parties | `DealStatusChanged` |

### Messaging (4)

| Event | Hub | Group | Client Event |
|---|---|---|---|
| `ConversationStarted` | Messaging | `user:{recipientUserId}` | `ConversationStarted` (refresh conv list) |
| `MessageSent` | Messaging | `conv:{conversationId}` | `MessageSent` |
| `MessageRead` | Messaging | `conv:{conversationId}` | `MessageRead` (opt-out filtered) |
| `MessageReportFiled` | Notifications | `admin-moderation` | `NotificationReceived` |

**Çift hub fan-out:** `MessageSent` counter-party offline ise Notifications hub'a da gider (push + in-app extra).

**Pure WS (no event bus):** `TypingIndicator` — `/messaging/hub` SendTypingIndicator method, no persist.

### Subscription (14)

| Event | Hub | Group | Client Event |
|---|---|---|---|
| `SubscriptionActivated` | Notifications | `user:{subscriberUserId}` | `NotificationReceived` + `SubscriptionStatusChanged` |
| `SubscriptionRenewed` | Notifications | `user:{subscriberUserId}` | `NotificationReceived` |
| `SubscriptionExpired` | Notifications | `user:{subscriberUserId}` | `SubscriptionStatusChanged` (kritik) |
| `SubscriptionCancelled` | Notifications | `user:{subscriberUserId}` | `SubscriptionStatusChanged` |
| `SubscriptionUpgraded` | Notifications | `user:{subscriberUserId}` | `SubscriptionStatusChanged` |
| `SubscriptionDowngraded` | Notifications | `user:{subscriberUserId}` | `SubscriptionStatusChanged` |
| `InvoiceIssued` | Notifications | `user:{subscriberUserId}` | `NotificationReceived` |
| `InvoicePaid` | Notifications | `user:{subscriberUserId}` | `NotificationReceived` |
| `InvoiceFailed` | Notifications | `user:{subscriberUserId}` | `NotificationReceived` (kritik) |
| `InvoiceRefunded` | Notifications | `user:{subscriberUserId}` | `NotificationReceived` |
| `BoostActivated` | Notifications | `user:{subscriberUserId}` | `NotificationReceived` |
| `BoostExpired` | Notifications | `user:{subscriberUserId}` | `NotificationReceived` |
| `BoostCancelled` | Notifications | `user:{subscriberUserId}` | `NotificationReceived` |
| `CommissionCharged` | Notifications | `user:{sellerUserId}` | `NotificationReceived` |

### Admin (0)

Admin terminal modül — Public event yok. Internal events `/notifications/hub` admin-moderation group'a broadcast (Faz 2 Backlog #182).

## Push Catalog Özet

| Kategori | Sayım |
|---|---|
| Toplam Public Event | 80 |
| Realtime push'a sahip | ~58 |
| Push yok (system/delivery feedback) | ~22 |
| `/notifications/hub` push | ~54 |
| `/messaging/hub` push | 3 (MessageSent/Read + ConversationStarted) |
| admin-moderation group | ~8 |

## Frontend Type-Safe Event Catalog

```typescript
// common/realtime/events.ts (codegen-friendly)
type RealtimeEvents = {
  // Notifications hub
  NotificationReceived: NotificationDto;
  UnreadCountChanged: { total: number };
  AccountStatusChanged: { newStatus: UserStatus; reason?: string };
  ListingStatusChanged: { listingId: string; newStatus: ListingStatus };
  ListingRemoved: { listingId: string };
  OfferStatusChanged: { offerId: string; newStatus: OfferStatus };
  DealStatusChanged: { dealId: string; newStatus: DealStatus };
  SubscriptionStatusChanged: { subscriptionId: string; newStatus: SubscriptionStatus };
  ForcedDisconnect: { reason: string };
  
  // Messaging hub
  MessageSent: MessageDto;
  MessageRead: { messageId: string; readerId: string; readAt: string };
  MessageEdited: { messageId: string; newContent: string };
  MessageDeleted: { messageId: string };
  TypingIndicator: { conversationId: string; userId: string; isTyping: boolean };
  ConversationStarted: ConversationSummaryDto;
};
```

## WebSocket Auth + Reconnect

### Auth

`?access_token=<jwt>` query string (SignalR standard).

### Token Expiry & Reconnect

**Karar:** Connection lifetime ≠ token lifetime.
- 15dk JWT expire ama WS connection long-lived
- Server periodic re-validate yapmıyor (sadece handshake)
- Suspension/logout → server explicit `ForcedDisconnect` event + connection drop

### Auto-Reconnect

```typescript
const connection = new HubConnectionBuilder()
  .withUrl("/notifications/hub", { 
    accessTokenFactory: () => getCurrentAccessToken()
  })
  .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
  .build();
```

### Missed Messages

Faz 1: Reconnect → `GET /me/notifications/unread-since=` pull.
Faz 2: SignalR Streams + Last-Event-Id replay buffer.

---

# Madde 7 — MultiPolygon GeoJSON 4-Derinlik Şema Doğrulama

## Nesting Doğrulama

**Polygon (3-depth):**
```json
{
  "type": "Polygon",
  "coordinates": [
    [                          ← LinearRing
      [28.5, 40.8],            ← Position [lng, lat]
      [29.5, 40.8],
      [29.5, 41.3],
      [28.5, 41.3],
      [28.5, 40.8]             ← First = last (closed)
    ]
  ]
}
```

**MultiPolygon (4-depth — 1 fazla):**
```json
{
  "type": "MultiPolygon",
  "coordinates": [
    [                          ← Polygon 1
      [                        ← LinearRing
        [28.5, 40.8], [29.5, 40.8], [29.5, 41.3], [28.5, 41.3], [28.5, 40.8]
      ]
    ],
    [                          ← Polygon 2 (disconnected region)
      [
        [27.0, 41.0], [27.5, 41.0], [27.5, 41.5], [27.0, 41.5], [27.0, 41.0]
      ]
    ]
  ]
}
```

Carrier.ServiceArea = `geometry(MultiPolygon, 4326)` — disconnected hizmet bölgeleri için 4-depth zorunlu.

## 3-Layer Validation

### Layer 1: FluentValidation (JSON Schema)

```csharp
public sealed class GeoJsonMultiPolygonValidator : AbstractValidator<GeoJsonMultiPolygon>
{
    public GeoJsonMultiPolygonValidator()
    {
        RuleFor(x => x.Type).Equal("MultiPolygon");
        RuleFor(x => x.Coordinates)
            .NotEmpty().WithMessage("At least 1 polygon")
            .Must(c => c.Count <= 50).WithMessage("Max 50 polygons (DoS guard)");
        RuleForEach(x => x.Coordinates).SetValidator(new PolygonCoordinatesValidator());
    }
}

// PolygonCoordinatesValidator: rings.Count >= 1, <= 20 (outer + holes)
// LinearRingValidator: positions.Count >= 4, <= 1000, BeClosed
// PositionValidator: [lng,lat] [-180,180] × [-90,90]
```

### Layer 2: NetTopologySuite Parse + Geometric Validation

```csharp
public sealed class GeoJsonMultiPolygonParser
{
    public Result<MultiPolygon> Parse(string geoJson)
    {
        try
        {
            using var reader = new StringReader(geoJson);
            using var jsonReader = new JsonTextReader(reader);
            var geometry = Serializer.Deserialize<Geometry>(jsonReader);
            
            if (geometry is not MultiPolygon mp)
                return Result.Failure<MultiPolygon>("GEOJSON_TYPE_MISMATCH", "Expected MultiPolygon");
            
            mp.SRID = 4326;
            
            if (!mp.IsValid)
            {
                var validOp = new IsValidOp(mp);
                return Result.Failure<MultiPolygon>("GEOMETRY_INVALID", validOp.ValidationError?.Message);
            }
            
            int totalVertices = CountVertices(mp);
            if (totalVertices > 10_000)
                return Result.Failure<MultiPolygon>("GEOMETRY_TOO_COMPLEX", 
                    $"Total vertex count {totalVertices} exceeds 10,000 limit");
            
            return Result.Success(mp);
        }
        catch (JsonException ex)
        {
            return Result.Failure<MultiPolygon>("GEOJSON_PARSE_ERROR", ex.Message);
        }
    }
}
```

### Layer 3: PostGIS ST_IsValid DB Constraint

```sql
ALTER TABLE carrier.carrier_service_areas
    ADD CONSTRAINT chk_area_valid CHECK (ST_IsValid(area));
```

Defense-in-depth — invalid polygon DB error.

## DoS Limits

| Limit | Değer |
|---|---|
| MultiPolygon.coordinates.length | 50 polygons |
| Polygon holes | 20 |
| LinearRing.points | 1000 |
| Total vertices across geometry | 10,000 |
| Request body size | 1 MB (nginx) |

## API Contract Sample

### Input

```http
POST /carrier/me/service-areas
Content-Type: application/json
Authorization: Bearer <jwt>

{
  "name": "Marmara Bölgesi",
  "country": "TR",
  "geometry": {
    "type": "MultiPolygon",
    "coordinates": [
      [
        [
          [28.5, 40.8], [29.5, 40.8], [29.5, 41.3], [28.5, 41.3], [28.5, 40.8]
        ]
      ]
    ]
  }
}
```

### Response (Success)

```json
{
  "id": "01J5C-...",
  "name": "Marmara Bölgesi",
  "country": "TR",
  "geometry": { ... },
  "areaSquareKm": 1234.5,
  "createdAt": "2026-05-10T12:00:00Z"
}
```

`areaSquareKm` = `ST_Area(geometry::geography) / 1_000_000`.

### Response (Validation Error)

```json
{
  "error": "VALIDATION_ERROR",
  "code": "GEOMETRY_INVALID",
  "message": "Self-intersection at coordinate (29.0, 41.0)",
  "details": {
    "validationType": "GeometricValidity",
    "errorPoint": [29.0, 41.0]
  }
}
```

## Frontend Map UI Compatibility

| Library | Output | Compatible? |
|---|---|---|
| Leaflet.draw | FeatureCollection wrapping Polygon/MultiPolygon | ✓ extract `features[0].geometry` |
| Mapbox GL Draw | FeatureCollection | ✓ |
| Google Maps Drawing Manager | Custom lat/lng | Manual conversion |
| react-leaflet-draw | FeatureCollection | ✓ |

Frontend extract Polygon → MultiPolygon wrap (1-polygon array).

## OpenAPI Schema

```yaml
components:
  schemas:
    GeoJsonMultiPolygon:
      type: object
      required: [type, coordinates]
      properties:
        type: { type: string, enum: [MultiPolygon] }
        coordinates:
          type: array
          minItems: 1
          maxItems: 50
          items: { $ref: '#/components/schemas/PolygonCoordinates' }
    
    PolygonCoordinates:
      type: array
      minItems: 1
      maxItems: 20
      items: { $ref: '#/components/schemas/LinearRingCoordinates' }
    
    LinearRingCoordinates:
      type: array
      minItems: 4
      maxItems: 1000
      items: { $ref: '#/components/schemas/GeoJsonPosition' }
    
    GeoJsonPosition:
      type: array
      minItems: 2
      maxItems: 3
      items: { type: number }
      example: [29.0, 41.0]
```

## NuGet

```xml
<PackageReference Include="NetTopologySuite" Version="2.5.x" />
<PackageReference Include="NetTopologySuite.IO.GeoJSON4STJ" Version="4.0.x" />
```

---

# Madde 8 — TypeScript Client Generation

## Stack: `@hey-api/openapi-ts`

| Araç | Tip | Sebep |
|---|---|---|
| openapi-typescript | Type-only | Fetch boilerplate manuel — yetersiz |
| orval | Full SDK + MSW | Opinionated, slow, MSW conflict |
| **@hey-api/openapi-ts** | SDK + TanStack hooks | OpenAPI 3.1 native, modern, type-safe |
| arf-cli (legacy) | İç tool | Maintenance burden, OpenAPI 3.1 eksik |

**Karar:** `@hey-api/openapi-ts` + TanStack Query plugin.

**arf-cli geçişi:** Kademeli kaldırma (Backlog #190).

## Per-Module SDK Yerleşim

```
livestock-frontend/
└── apps/web/src/api/
    ├── catalog/
    │   ├── client.ts
    │   ├── types.gen.ts
    │   ├── services.gen.ts
    │   └── @tanstack/react-query.gen.ts
    ├── identity/
    ├── accounts/
    ├── carrier/
    ├── listings/
    ├── marketplace/
    ├── messaging/
    ├── notifications/
    ├── subscription/
    ├── admin/
    └── shared/        ← cross-module DTO'lar
```

## Config

```typescript
// openapi-ts.config.ts
import { defineConfig } from '@hey-api/openapi-ts';

const modules = ['catalog', 'identity', 'accounts', 'carrier', 'listings',
                 'marketplace', 'messaging', 'notifications', 'subscription', 'admin'];

export default modules.map(module => defineConfig({
  client: '@hey-api/client-fetch',
  input: `./openapi-specs/${module}.json`,
  output: { path: `./src/api/${module}`, format: 'prettier', lint: 'eslint' },
  plugins: [
    '@hey-api/types',
    '@hey-api/services',
    { name: '@hey-api/transformers', dates: true },        // ISO string → Date
    { name: '@tanstack/react-query', queryKeys: true, infiniteQueries: true },
  ],
}));
```

## tsconfig Path Mapping

```json
{
  "compilerOptions": {
    "paths": {
      "@livestock/api/catalog": ["./src/api/catalog/index.ts"],
      "@livestock/api/identity": ["./src/api/identity/index.ts"],
      // ... 10 modül + shared
    }
  }
}
```

Frontend kullanım:
```typescript
import { useListingsListInfiniteQuery } from '@livestock/api/listings/@tanstack/react-query';

const { data, fetchNextPage } = useListingsListInfiniteQuery({
  query: { category: 'livestock-cattle/dairy-cow', sort: 'newest' },
});
```

## Generated Code Commit Strategy

**Karar: COMMIT EDİLİYOR.**

- Pros: IDE intellisense, PR review visibility, no CI dep, offline dev
- Cons: Büyük diff regen sırasında
- Mitigation: Prettier auto-format → consistent diff

## Custom Type Overrides

Money/Translations runtime class hydration:

```typescript
// src/api/shared/overrides.ts (manuel, codegen değil)
export class Money {
  constructor(readonly amount: number, readonly currency: string) {}
  
  static fromDto(dto: MoneyDto): Money { return new Money(dto.amount, dto.currency); }
  
  format(locale: string): string {
    return new Intl.NumberFormat(locale, { style: 'currency', currency: this.currency }).format(this.amount);
  }
}
```

Faz 1: kullanım yerinde `Money.fromDto(response.price)`. Faz 2 middleware auto-transform.

## Discriminated Union Response

```typescript
const { data, error } = await api.listings.getListing({ id: '...' });
// data: ListingDetailDto | undefined
// error: ApiError | undefined (RFC 7807 — Madde 9)
```

## TanStack Query Hook Output

```typescript
export const useListingsListInfiniteQuery = (options) => 
  useInfiniteQuery({
    queryKey: ['listings', 'list', options.query],
    queryFn: ({ pageParam }) => 
      api.listings.list({ ...options, query: { ...options.query, cursor: pageParam } }),
    getNextPageParam: (lastPage) => lastPage.nextCursor,
    initialPageParam: undefined,
  });
```

**Cursor pagination native support:** `useInfiniteQuery` Madde 3 `CursorPage<T>` ile uyumlu.

## Runtime Config

```typescript
const baseUrl = process.env.NEXT_PUBLIC_API_BASE_URL ?? 'http://localhost:5000';

[catalogClient, identityClient, /* ... */].forEach(client => {
  client.setConfig({
    baseUrl,
    headers: () => ({
      'Accept-Language': getCurrentLocale(),
      'X-Trace-Id': generateTraceId(),
    }),
    interceptors: {
      request: [addAuthHeader, addLocaleHeader],
      response: [handleAuthRefresh, handleGlobalError],
    },
  });
});
```

## CI Pipeline

```yaml
# Frontend repo .github/workflows/codegen-check.yml
- name: Download OpenAPI specs (from backend gh-pages)
  run: pnpm run sync:openapi
- name: Generate API client
  run: pnpm run codegen
- name: Check uncommitted changes
  run: |
    if [[ -n "$(git status --porcelain src/api/)" ]]; then
      echo "Generated code outdated — run pnpm codegen + commit"
      exit 1
    fi
- name: Type check
  run: pnpm run type-check
```

---

# Madde 9 — Error Code Matrix (155 Code Full)

## Standard Error Response Shape — RFC 7807 + Custom Code

```json
{
  "type": "https://docs.livestock-trading.com/errors/validation-error",
  "title": "Validation Error",
  "status": 400,
  "code": "VALIDATION_ERROR",
  "detail": "One or more validation failures occurred",
  "instance": "/listings",
  "traceId": "01J5C-CORR-ID",
  "timestamp": "2026-05-10T12:00:00Z",
  "errors": [
    { "field": "title.tr", "code": "REQUIRED", "message": "Turkish title is required" }
  ]
}
```

**Content-Type:** `application/problem+json` (RFC 7807).

## Localization — Code-First, Frontend Localized

| Aktör | Sorumluluk |
|---|---|
| Backend | Stable `code` + English `detail` default |
| Frontend | `code` → i18n bundle → kullanıcı locale |

Frontend bundle örnek:
```json
// frontend/src/i18n/locales/tr/errors.json
{
  "VALIDATION_ERROR": "Form bilgilerinde hatalar var",
  "LISTING_NOT_FOUND": "İlan bulunamadı",
  "OFFER_EXPIRED": "Teklifin süresi doldu",
  "REFRESH_QUOTA_EXHAUSTED": "Aylık ilan yenileme kotanı doldurdun. Pro plana geç!"
}
```

## HTTP Status Code Mapping

| Status | Code'lar (örnek) |
|---|---|
| **400 Bad Request** | VALIDATION_ERROR, INVALID_CURSOR, MALFORMED_REQUEST |
| **401 Unauthorized** | AUTH_REQUIRED, TOKEN_EXPIRED, TOKEN_INVALID, TOKEN_REVOKED, INVALID_CREDENTIALS |
| **403 Forbidden** | INSUFFICIENT_ROLE, ACCOUNT_SUSPENDED, OWNERSHIP_REQUIRED, KVKK_CONSENT_REQUIRED |
| **404 Not Found** | NOT_FOUND, USER_NOT_FOUND, LISTING_NOT_FOUND |
| **409 Conflict** | DUPLICATE_RESOURCE, INVALID_STATE_TRANSITION, OPTIMISTIC_LOCK_FAILURE |
| **410 Gone** | LISTING_DELETED, USER_ANONYMIZED |
| **422 Unprocessable** | BUSINESS_RULE_VIOLATION, OFFER_EXPIRED, LISTING_NOT_ACTIVE |
| **429 Too Many Requests** | RATE_LIMITED, OTP_RATE_LIMITED, REFRESH_QUOTA_EXHAUSTED, AI_TAG_RATE_LIMITED |
| **500 Internal** | INTERNAL_ERROR (generic, no detail prod) |
| **502 Bad Gateway** | PAYMENT_PROVIDER_UNAVAILABLE, DELIVERY_PROVIDER_FAILED |
| **503 Service Unavailable** | MAINTENANCE_MODE |
| **504 Gateway Timeout** | DOWNSTREAM_TIMEOUT, OPERATION_TIMEOUT |

Status başlıkları örnek kod gösterir; her örnek kod ilgili modül listesindedir (aşağıda Universal + per-module bölümleri).

## Universal Error Codes (20)

| Code | Status | Açıklama |
|---|---|---|
| `VALIDATION_ERROR` | 400 | Field-level validation hatası |
| `INVALID_CURSOR` | 400 | Cursor pagination token bozuk |
| `MALFORMED_REQUEST` | 400 | Body parse veya schema fail |
| `AUTH_REQUIRED` | 401 | Token yok |
| `TOKEN_EXPIRED` | 401 | Access token süresi doldu |
| `TOKEN_INVALID` | 401 | Token signature/format bozuk |
| `TOKEN_REVOKED` | 401 | Refresh token revoke edilmiş |
| `INSUFFICIENT_ROLE` | 403 | Rol yetersiz |
| `OWNERSHIP_REQUIRED` | 403 | Kaynak başkasına ait |
| `NOT_FOUND` | 404 | Generic kaynak bulunamadı |
| `DUPLICATE_RESOURCE` | 409 | Slug çakışması dışı generic duplicate |
| `INVALID_STATE_TRANSITION` | 409 | FSM transition reddedildi |
| `OPTIMISTIC_LOCK_FAILURE` | 409 | Concurrent update conflict |
| `USER_ANONYMIZED` | 410 | GDPR cascade sonrası user fetch |
| `BUSINESS_RULE_VIOLATION` | 422 | Modül-spesifik olmayan generic 422 |
| `RATE_LIMITED` | 429 | Generic rate limit |
| `INTERNAL_ERROR` | 500 | Beklenmedik hata (prod'da detail yok) |
| `MAINTENANCE_MODE` | 503 | Planlı bakım |
| `DOWNSTREAM_TIMEOUT` | 504 | Aşağı akış servis timeout |
| `OPERATION_TIMEOUT` | 504 | Kendi handler timeout |

## Identity (16 Code)

| Code | Status |
|---|---|
| `USER_NOT_FOUND` | 404 |
| `INVALID_CREDENTIALS` | 401 |
| `ACCOUNT_SUSPENDED` | 403 |
| `ACCOUNT_PENDING_DELETION` | 403 |
| `EMAIL_NOT_VERIFIED` | 403 |
| `PHONE_NOT_VERIFIED` | 403 |
| `EMAIL_ALREADY_REGISTERED` | 409 |
| `PHONE_ALREADY_REGISTERED` | 409 |
| `NATIONAL_ID_ALREADY_REGISTERED` | 409 |
| `KVKK_CONSENT_REQUIRED` | 403 |
| `OTP_INVALID_CODE` | 401 |
| `OTP_EXPIRED` | 401 |
| `OTP_ATTEMPT_LIMIT_EXCEEDED` | 401 |
| `OTP_RATE_LIMITED` | 429 |
| `IMPERSONATION_ACTIVE` | 403 |
| `PASSWORD_RESET_TOKEN_INVALID` | 401 |

## Accounts (17 Code)

| Code | Status |
|---|---|
| `SELLER_NOT_FOUND` | 404 |
| `SELLER_ONBOARDING_INCOMPLETE` | 422 |
| `SELLER_NOT_VERIFIED` | 403 |
| `SELLER_SUSPENDED` | 403 |
| `SELLER_ACCOUNT_TYPE_MISMATCH` | 422 |
| `SLUG_ALREADY_TAKEN` | 409 |
| `SLUG_CHANGE_RATE_LIMITED` | 429 |
| `IBAN_INVALID` | 400 |
| `CKS_INVALID_FORMAT` | 400 |
| `FARM_NOT_FOUND` | 404 |
| `FARM_NOT_OWNED` | 403 |
| `FARM_CANNOT_LIST_ANIMALS` | 422 |
| `HEALTH_RECORD_IMMUTABLE` | 422 |
| `VET_PROFILE_NOT_FOUND` | 404 |
| `VET_NOT_VERIFIED` | 403 |
| `REVIEW_ALREADY_WRITTEN` | 409 |
| `REVIEW_DEAL_NOT_COMPLETED` | 422 |

## Carrier (11 Code)

| Code | Status |
|---|---|
| `CARRIER_NOT_FOUND` | 404 |
| `CARRIER_NOT_VERIFIED` | 403 |
| `CARRIER_SUSPENDED` | 403 |
| `CARRIER_RATE_MISSING_FOR_ZONE` | 422 |
| `CARRIER_NO_VEHICLE_CAPACITY` | 422 |
| `SHIPMENT_NOT_FOUND` | 404 |
| `SHIPMENT_INVALID_STATUS_TRANSITION` | 422 |
| `SHIPMENT_NOT_ASSIGNED_TO_CARRIER` | 403 |
| `CARRIER_OFFER_EXPIRED` | 422 |
| `GEOMETRY_INVALID` | 400 |
| `GEOMETRY_TOO_COMPLEX` | 400 |

## Catalog (8 Code)

| Code | Status |
|---|---|
| `CATEGORY_NOT_FOUND` | 404 |
| `CATEGORY_DEACTIVATED` | 422 |
| `BREED_NOT_VALID_FOR_CATEGORY` | 422 |
| `BRAND_NOT_VALID_FOR_CATEGORY` | 422 |
| `BRAND_NOT_ACTIVE` | 422 |
| `BRAND_SLUG_ALREADY_TAKEN` | 409 |
| `LOCATION_NOT_FOUND` | 404 |
| `BORDER_RULE_VIOLATION` | 422 |

## Listings (15 Code)

| Code | Status |
|---|---|
| `LISTING_NOT_FOUND` | 404 |
| `LISTING_NOT_OWNED` | 403 |
| `LISTING_DELETED` | 410 |
| `LISTING_NOT_ACTIVE` | 422 |
| `LISTING_RESERVED` | 422 |
| `LISTING_INVALID_STATUS_TRANSITION` | 422 |
| `LISTING_EDIT_REQUIRES_REAPPROVAL` | 422 |
| `LISTING_MAX_LIMIT_REACHED` | 403 |
| `LISTING_MIN_IMAGES_REQUIRED` | 422 |
| `SAVED_SEARCH_NOT_FOUND` | 404 |
| `SAVED_SEARCH_FILTER_INVALID` | 400 |
| `REFRESH_QUOTA_EXHAUSTED` | 429 |
| `AI_TRANSLATION_QUOTA_EXHAUSTED` | 429 |
| `AI_TAG_RATE_LIMITED` | 429 |
| `LISTING_PRICE_CHANGE_TOO_FREQUENT` | 429 |

## Marketplace (18 Code)

| Code | Status |
|---|---|
| `OFFER_NOT_FOUND` | 404 |
| `OFFER_NOT_PARTICIPANT` | 403 |
| `OFFER_NOT_PENDING` | 422 |
| `OFFER_EXPIRED` | 422 |
| `OFFER_COUNTER_DEPTH_EXCEEDED` | 422 |
| `OFFER_WRONG_ACTOR_FOR_ACCEPT` | 403 |
| `DEAL_NOT_FOUND` | 404 |
| `DEAL_NOT_PARTICIPANT` | 403 |
| `DEAL_INVALID_STATUS_FOR_CHECKOUT` | 422 |
| `DEAL_INVALID_STATUS_FOR_CANCEL` | 422 |
| `DEAL_INVALID_STATUS_FOR_DISPUTE` | 422 |
| `PAYMENT_METHOD_NOT_FOUND` | 404 |
| `PAYMENT_FAILED` | 422 |
| `PAYMENT_PROVIDER_UNAVAILABLE` | 502 |
| `DISPUTE_NOT_FOUND` | 404 |
| `DISPUTE_ALREADY_RAISED` | 409 |
| `DISPUTE_EVIDENCE_IMMUTABLE` | 422 |
| `FAVORITE_LISTING_INACTIVE` | 422 |

## Subscription (12 Code)

| Code | Status |
|---|---|
| `SUBSCRIPTION_NOT_FOUND` | 404 |
| `SUBSCRIPTION_ALREADY_ACTIVE` | 409 |
| `SUBSCRIBER_NOT_SELLER` | 403 |
| `PLAN_NOT_FOUND` | 404 |
| `PLAN_INACTIVE` | 422 |
| `TRIAL_ALREADY_USED` | 409 |
| `INVOICE_NOT_FOUND` | 404 |
| `INVOICE_NOT_PAYABLE` | 422 |
| `PAYMENT_DECLINED` | 422 |
| `STRIPE_WEBHOOK_INVALID_SIGNATURE` | 401 |
| `BOOST_PACKAGE_INACTIVE` | 422 |
| `BOOST_LISTING_NOT_OWNED` | 403 |

## Messaging (8 Code)

| Code | Status |
|---|---|
| `CONVERSATION_NOT_FOUND` | 404 |
| `CONVERSATION_NOT_PARTICIPANT` | 403 |
| `MESSAGE_NOT_FOUND` | 404 |
| `MESSAGE_EDIT_WINDOW_EXPIRED` | 422 |
| `MESSAGE_TYPE_NOT_EDITABLE` | 422 |
| `USER_BLOCKED` | 403 |
| `ATTACHMENT_TOO_LARGE` | 422 |
| `MESSAGE_TRANSLATION_QUOTA_EXHAUSTED` | 429 |

## Notifications (5 Code)

| Code | Status |
|---|---|
| `TEMPLATE_NOT_FOUND` | 404 |
| `TEMPLATE_KEY_CONFLICT` | 409 |
| `DELIVERY_PROVIDER_FAILED` | 502 |
| `MARKETING_CONSENT_MISSING` | 403 |
| `IN_APP_NOTIFICATION_NOT_FOUND` | 404 |

## Admin (8 Code)

| Code | Status |
|---|---|
| `FEATURE_FLAG_NOT_FOUND` | 404 |
| `FEATURE_FLAG_CODE_TAKEN` | 409 |
| `SCHEDULED_REPORT_NOT_FOUND` | 404 |
| `IMPERSONATION_TARGET_IS_ADMIN` | 403 |
| `IMPERSONATION_SESSION_ACTIVE` | 409 |
| `IMPERSONATION_NOT_AUTHORIZED` | 403 |
| `SYSTEM_VERSION_DUPLICATE` | 409 |
| `SYSTEM_VERSION_INACTIVATE_REQUIRES_ALTERNATIVE` | 422 |

## Field Error Codes (17 — `errors[].code`)

| Code | Anlam | Örnek field |
|---|---|---|
| `REQUIRED` | NotEmpty | name, email, title |
| `OUT_OF_RANGE` | InclusiveBetween/GreaterThan | amount, count |
| `INVALID_FORMAT` | Regex match | phone, slug |
| `INVALID_EMAIL` | EmailAddress | email |
| `INVALID_URL` | URL format | website |
| `INVALID_ENUM` | Must in enum | status, type |
| `TOO_LONG` | MaximumLength | title, description |
| `TOO_SHORT` | MinimumLength | password |
| `MUST_BE_UNIQUE` | DB unique check | code, slug |
| `MUST_EXIST` | DB FK check | categoryId, brandId |
| `INVALID_IBAN` | IBAN checksum | iban |
| `INVALID_NATIONAL_ID` | TC algoritmik | nationalId |
| `INVALID_LANGUAGE_CODE` | Catalog whitelist | preferredLocale |
| `INVALID_CURRENCY_CODE` | Catalog whitelist | currency |
| `INVALID_COUNTRY_CODE` | Catalog whitelist | countryCode |
| `FUTURE_DATE_REQUIRED` | Geçmiş tarih reddedildi | scheduledPickupAt |
| `PAST_DATE_REQUIRED` | Gelecek tarih reddedildi | issuedAt |

`errors[].field` JSONPath: `"title.tr"`, `"price.amount"`, `"address.iban"`, `"items[2].quantity"`.

## Toplam Sayım

| Modül | Code Sayısı |
|---|---|
| Universal | 20 |
| Identity | 16 |
| Accounts | 17 |
| Carrier | 11 |
| Catalog | 8 |
| Listings | 15 |
| Marketplace | 18 |
| Subscription | 12 |
| Messaging | 8 |
| Notifications | 5 |
| Admin | 8 |
| Field-level | 17 |
| **TOPLAM** | **155** |

## Backend Implementation — Exception Hierarchy

```csharp
public abstract class AppException : Exception
{
    public string Code { get; }
    public int Status { get; }
    public IReadOnlyDictionary<string, object>? Details { get; }
    
    protected AppException(string code, int status, string message, IReadOnlyDictionary<string, object>? details = null) 
        : base(message)
    {
        Code = code; Status = status; Details = details;
    }
}

public class NotFoundException : AppException { public NotFoundException(string code, string message) : base(code, 404, message) { } }
public class ValidationException : AppException
{
    public IReadOnlyList<FieldError> Errors { get; }
    public ValidationException(IReadOnlyList<FieldError> errors) : base("VALIDATION_ERROR", 400, "Validation failed") { Errors = errors; }
}
public sealed record FieldError(string Field, string Code, string Message);

public class ConflictException : AppException { ... }
public class ForbiddenException : AppException { ... }
public class UnauthorizedException : AppException { ... }
public class BusinessRuleException : AppException { ... }
public class RateLimitException : AppException 
{
    public int RetryAfterSeconds { get; }
    public RateLimitException(string code, int retryAfter) : base(code, 429, "Rate limited") { RetryAfterSeconds = retryAfter; }
}
```

## ProblemDetailsMiddleware

```csharp
public sealed class ProblemDetailsMiddleware
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        try { await _next(ctx); }
        catch (Exception ex) { await HandleAsync(ctx, ex); }
    }
    
    private async Task HandleAsync(HttpContext ctx, Exception ex)
    {
        var traceId = ctx.TraceIdentifier;
        
        var (status, code, message, details, fieldErrors) = ex switch
        {
            ValidationException ve => (400, "VALIDATION_ERROR", "Validation failed", null, ve.Errors),
            NotFoundException nfe => (nfe.Status, nfe.Code, nfe.Message, nfe.Details, null),
            ConflictException ce => (409, ce.Code, ce.Message, ce.Details, null),
            ForbiddenException fe => (403, fe.Code, fe.Message, fe.Details, null),
            UnauthorizedException ue => (401, ue.Code, ue.Message, ue.Details, null),
            BusinessRuleException bre => (422, bre.Code, bre.Message, bre.Details, null),
            RateLimitException rle => (429, rle.Code, rle.Message, 
                new Dictionary<string, object> { ["retryAfterSeconds"] = rle.RetryAfterSeconds }, null),
            AppException ae => (ae.Status, ae.Code, ae.Message, ae.Details, null),
            _ => Handle500(ex, traceId)
        };
        
        var problem = new
        {
            type = $"https://docs.livestock-trading.com/errors/{code.ToLowerInvariant().Replace('_', '-')}",
            title = GetTitleFromCode(code),
            status, code,
            detail = message,
            instance = ctx.Request.Path.Value,
            traceId,
            timestamp = DateTimeOffset.UtcNow,
            errors = fieldErrors,
            details
        };
        
        ctx.Response.StatusCode = status;
        ctx.Response.ContentType = "application/problem+json";
        if (status == 429 && details?.ContainsKey("retryAfterSeconds") == true)
            ctx.Response.Headers["Retry-After"] = details["retryAfterSeconds"].ToString();
        
        await ctx.Response.WriteAsJsonAsync(problem);
    }
}
```

## FluentValidation Integration

```csharp
public sealed class FluentValidationFilter<TRequest> : IRequestFilter<TRequest>
{
    public async Task InvokeAsync(TRequest request, CancellationToken ct)
    {
        var result = await _validator.ValidateAsync(request, ct);
        if (!result.IsValid)
        {
            var errors = result.Errors
                .Select(e => new FieldError(e.PropertyName, MapErrorCode(e.ErrorCode), e.ErrorMessage))
                .ToList();
            throw new ValidationException(errors);
        }
    }
    
    private static string MapErrorCode(string fluentCode) => fluentCode switch
    {
        "NotEmptyValidator" or "NotNullValidator" => "REQUIRED",
        "EmailValidator" => "INVALID_EMAIL",
        "RegularExpressionValidator" => "INVALID_FORMAT",
        "MaximumLengthValidator" => "TOO_LONG",
        "MinimumLengthValidator" => "TOO_SHORT",
        "GreaterThanValidator" or "GreaterThanOrEqualValidator" => "OUT_OF_RANGE",
        "InclusiveBetweenValidator" => "OUT_OF_RANGE",
        _ => fluentCode.Replace("Validator", "").ToUpperSnakeCase()
    };
}
```

## Frontend Discriminated Union

```typescript
export interface ApiError {
  type: string;
  title: string;
  status: number;
  code: string;            // discriminator
  detail: string;
  instance: string;
  traceId: string;
  timestamp: string;
  errors?: FieldError[];
  details?: Record<string, unknown>;
}

export interface FieldError { field: string; code: string; message: string; }
```

### Global Error Boundary

```typescript
function GlobalErrorHandler({ error }: { error: ApiError }) {
  const { t } = useTranslation('errors');
  
  switch (error.code) {
    case 'AUTH_REQUIRED':
    case 'TOKEN_EXPIRED':
      router.push('/login?from=' + window.location.pathname);
      return null;
    case 'ACCOUNT_SUSPENDED':
      return <SuspendedAccountBanner reason={error.detail} />;
    case 'MAINTENANCE_MODE':
      return <MaintenancePage />;
    case 'RATE_LIMITED':
    case 'OTP_RATE_LIMITED':
      const retryAfter = error.details?.retryAfterSeconds as number;
      return <RateLimitedToast seconds={retryAfter} />;
    default:
      toast.error(t(error.code, { defaultValue: error.detail }));
      return null;
  }
}
```

### Form Field Error Mapping

```typescript
function useApiFormErrors<TForm>(setError: UseFormSetError<TForm>, apiError: ApiError | null) {
  useEffect(() => {
    if (apiError?.code !== 'VALIDATION_ERROR') return;
    apiError.errors?.forEach(fe => {
      setError(fe.field as Path<TForm>, {
        type: fe.code,
        message: t(`field-errors.${fe.code}`, { defaultValue: fe.message })
      });
    });
  }, [apiError]);
}
```

## OpenAPI Schema

```yaml
components:
  schemas:
    ProblemDetails:
      type: object
      required: [type, title, status, code]
      properties:
        type: { type: string, format: uri }
        title: { type: string }
        status: { type: integer }
        code: { type: string }
        detail: { type: string }
        instance: { type: string }
        traceId: { type: string }
        timestamp: { type: string, format: date-time }
        errors: { type: array, items: { $ref: '#/components/schemas/FieldError' } }
        details: { type: object, additionalProperties: true }
```

Endpoint annotation: `[ProducesProblem(400, "VALIDATION_ERROR")]` → OpenAPI'ye yansır.

---

# Madde 10 — Migration Timing (Frontend MSW → Real API)

## Per-Wave Switchover Plan

```
Wave 0 — Infrastructure (1 hafta)
  Backend: PostgreSQL + Redis + RabbitMQ + MinIO setup
  Frontend: MSW tüm modüller aktif

Wave 1 — Catalog (1-2 hafta)
  Backend deploy: seed (250 country, 180 currency, 50 lang, 12 cert, 54 category, 80 breed, 100 brand, TR 885K location)
  Frontend: NEXT_PUBLIC_USE_MSW_CATALOG=false; real /catalog/*

Wave 2 — Identity (2 hafta)
  Backend deploy: 3-method login + KVKK + OTP + OAuth + Data Export
  Frontend: real auth flow

Wave 3 — Accounts (2 hafta)
  Backend deploy: Seller 5-step onboarding + Farm + Vet + Review + Slug
  Frontend: Seller dashboard real

Wave 4 — Listings (1-2 hafta)
  Backend deploy: Listing CRUD + faceted search + AI translate/tag + multi-currency
  Frontend: Listings page real

Wave 5 — Carrier + Subscription (paralel, 2-3 hafta)
  Frontend: Carrier dashboard + Subscription /pricing real

Wave 6 — Marketplace + Messaging + Notifications (paralel, 3 hafta)
  Frontend: Offer/Deal/Dispute real, chat real-time, notif stream

Wave 7 — Admin (1-2 hafta)
  Frontend: Admin panel real

Production Cutover (1 hafta)
  DNS cutover + eski sistem kapatma
```

**Toplam takvim: ~15-19 hafta = 4-5 ay solo developer.**

## MSW Handler Removal Policy

**Karar: Handler'lar KEEP, kullanım disable.**

```typescript
// src/mocks/setup.ts
const enabledModules = process.env.NEXT_PUBLIC_MSW_MODULES?.split(',') ?? [];

const allHandlers = [
  ...(enabledModules.includes('listings') ? listingsHandlers : []),
  ...(enabledModules.includes('identity') ? identityHandlers : []),
  // ...
];

setupWorker(...allHandlers).start();
```

## Environment Matrix

| Env | Backend | Stripe | AI | MSW |
|---|---|---|---|---|
| Local dev (BE-only) | localhost real | Mock | Mock | none |
| Local dev (FE Wave N+ MSW) | staging real | Mock (BE) | Mock (BE) | Wave N+ |
| Staging | real | Stripe test mode | OpenAI test | none |
| Production | real | Stripe live | OpenAI prod | none |
| Unit tests | n/a (MSW) | MSW | MSW | all |

## Feature Flag Granular Switchover

```typescript
const useRealDisputes = useFeatureFlag('marketplace-real-disputes');

const handlers = useRealDisputes 
  ? realMarketplaceHandlers 
  : [...realMarketplaceCoreHandlers, ...mswDisputeHandlers];
```

Backend FeatureFlag (Karar 5/Admin) ile granular control.

## Rollback Strategy

| Senaryo | Aksiyon |
|---|---|
| Frontend rollback | `NEXT_PUBLIC_USE_MSW_{MODULE}=true` → MSW geri devreye |
| Backend rollback | Karar 4g Senaryo A — image tag previous (~2dk) |
| DB rollback | Karar 4g Senaryo D — snapshot restore (~30-60dk) |

## Production Cutover Checklist

```
☐ Tüm 10 modül staging Real API ile geçti
☐ E2E test suite passing
☐ Load test (1000 concurrent) passing
☐ Pre-deploy DB backup (pg_dump retention 90gün)
☐ DNS TTL 5dk (cutover öncesi)
☐ Eski sistem maintenance mode (read-only 1 hafta önce)
☐ Yeni sistem DNS cutover
☐ Eski sistem domain redirect → yeni
☐ 24/7 oncall hazır
☐ Rollback prosedürü test edildi
☐ Postmortem template + incident channel hazır
```

---

## Karar 6 Discovered Backlog (36 item)

| # | Konu | Hedef |
|---|---|---|
| 167-202 | Karar 6 boyunca biriken backlog item'lar | Karar 7 / Process / Faz 2 |

Detay [backlog.md](backlog.md).
