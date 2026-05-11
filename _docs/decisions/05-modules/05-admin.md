# Karar 5 / Admin Modülü

**Status:** FINAL (SON MODÜL)
**Wave:** 7 (cross-module aggregator, last)

## İlişkili Kararlar

- **Üst:** [Karar 2 revize](../02-modules-list.md) — Admin yeni modül (10. modül)
- **Patch:** Görev 1/G — synchronous `IAdminXCommands` pattern (Public event vs sync command karar)
- **Frontend:** `frontend-api-inventory.md` Admin (~22 endpoint — dashboard, moderation queues, impersonation, feature flags, reports)

---

## 1. AR vs Entity Sınıflandırma

| | Görev 4 AR Listesi | Final |
|---|---|---|
| AdminUser | ❌ — Identity.User + role | **YOK** |
| AdminRole | ❌ — Identity.UserRole | **YOK** |
| FeatureFlag | ✓ | ✓ AR |
| ScheduledReport | ✓ | ✓ AR |
| SystemVersion | ✓ | ✓ AR |
| ImpersonationSession | ✓ | ✓ AR |
| AdminAuditLog | entity | **entity** (logging concern, AR değil) |

**Final: 4 AR.**

---

## 2. Modülün Rolü ve Sınırları

### Sahip

| Konsept | Sahiplik |
|---|---|
| FeatureFlag AR (toggle, rollout, targeting) | Admin |
| ScheduledReport AR (cron + delivery) | Admin |
| SystemVersion AR (force-update per platform) | Admin |
| ImpersonationSession AR ("login as user" audit, 4h max) | Admin |
| AdminAuditLog entity (cross-module write audit, append-only) | Admin |
| Cross-module dashboard aggregation | Admin |
| 3-queue moderation orchestration (listings + accounts + carrier) | Admin (UI orchestration; her modül kendi queue'su) |

### Sahip Olmayan

| Konsept | Sahibi |
|---|---|
| Admin kullanıcı kimliği | Identity |
| Admin rolü grant/revoke | Identity (`IAdminUserCommands.GrantRoleAsync`) |
| Modül-spesifik admin endpoint'leri (suspend/verify/approve) | İlgili modül |

**Admin modülü = "BFF for admin panel" + "kendi data'sı (flags/reports/versions/impersonation)" — cross-module aksiyonları `IAdminXCommands` interface üzerinden tetikler.**

---

## 3. Aggregate Roots

### `FeatureFlag` AR

```csharp
public class FeatureFlag
{
    public Guid Id;
    public string Code;                                          // "carrier-fleet-gps", "ai-vet-recommendations"
    public Translations Name, Description;
    
    public bool IsEnabled;                                       // master switch
    public int RolloutPercentage;                                // 0-100
    
    public IReadOnlyList<Guid> TargetUserIds;                    // whitelist (JSONB)
    public IReadOnlyList<string> TargetCountries;                // ISO codes
    public IReadOnlyList<string> TargetRoles;                    // role names
    
    public DateTimeOffset? StartAt, EndAt;
    
    public Guid CreatedByUserId;
    public Guid? UpdatedByUserId;
    public DateTimeOffset CreatedAt, UpdatedAt;
    
    public static FeatureFlag Create(string code, Translations name, Guid actorAdminId) { ... }
    
    public void Toggle(bool enabled, Guid actorAdminId) { ... }
    public void SetRolloutPercentage(int percent, Guid actorAdminId) { ... }
    public void UpdateTargeting(IReadOnlyList<Guid>? users, IReadOnlyList<string>? countries, IReadOnlyList<string>? roles, Guid actorAdminId) { ... }
    public void SetTimeWindow(DateTimeOffset? start, DateTimeOffset? end, Guid actorAdminId) { ... }
    
    public bool IsEnabledFor(FeatureFlagContext ctx, DateTimeOffset now)
    {
        if (!IsEnabled) return false;
        if (StartAt.HasValue && now < StartAt.Value) return false;
        if (EndAt.HasValue && now > EndAt.Value) return false;
        
        // Whitelist override
        if (ctx.UserId.HasValue && TargetUserIds.Contains(ctx.UserId.Value)) return true;
        
        // Country filter
        if (TargetCountries.Count > 0 && (ctx.CountryCode is null || !TargetCountries.Contains(ctx.CountryCode)))
            return false;
        
        // Role filter
        if (TargetRoles.Count > 0 && !ctx.Roles.Any(r => TargetRoles.Contains(r)))
            return false;
        
        // Percentage rollout (stable per user)
        if (RolloutPercentage >= 100) return true;
        if (RolloutPercentage <= 0) return false;
        if (ctx.UserId.HasValue)
        {
            var hash = ComputeStableHash(Code, ctx.UserId.Value);   // 0-99
            return hash < RolloutPercentage;
        }
        return RolloutPercentage >= 50;
    }
    
    private static int ComputeStableHash(string code, Guid userId)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes($"{code}:{userId}"));
        return BitConverter.ToInt32(bytes, 0) & 0x7FFFFFFF % 100;
    }
}

public sealed record FeatureFlagContext(
    Guid? UserId,
    string? CountryCode,
    IReadOnlyList<string> Roles);
```

**Stable hash bucket:** Aynı (code, userId) → aynı bucket → rollout fluctuate yok.

### `ScheduledReport` AR

```csharp
public class ScheduledReport
{
    public Guid Id;
    public string Name;
    public Guid OwnerUserId;
    public ReportType Type;                                      // SalesGmv/SubscriptionMrr/UserGrowth/...
    public string FilterJson;
    public string CronExpression;                                // "0 0 9 * * MON"
    public string Timezone;                                       // IANA
    
    public ReportDeliveryChannel DeliveryChannel;                 // Email/MinioBucket/AdminDashboard
    public string DeliveryConfigJson;
    
    public bool IsActive;
    public DateTimeOffset? LastExecutedAt, NextScheduledAt;
    public DateTimeOffset CreatedAt, UpdatedAt;
    
    private readonly List<ReportExecution> _executions = new();
    
    public static ScheduledReport Create(...) { ... }
    public void UpdateSchedule(string cron, string tz) { ... }
    public void Activate() { ... }
    public void Deactivate() { ... }
    public ReportExecution RecordExecution(ReportExecutionStatus status, string? artifactUrl, string? errorMsg) { ... }
}

public enum ReportType
{
    SalesGmv = 1, SubscriptionMrr = 2, UserGrowth = 3,
    ListingsActivity = 4, CarrierShipments = 5, SellerVerifications = 6,
    Disputes = 7,
    CustomDapperQuery = 99                                       // Faz 2 sandboxed SQL
}

public enum ReportDeliveryChannel { Email = 1, MinioBucket = 2, AdminDashboard = 3 }
public enum ReportExecutionStatus { Pending = 1, Running = 2, Succeeded = 3, Failed = 4 }

public class ReportExecution
{
    public Guid Id, ScheduledReportId;
    public DateTimeOffset StartedAt;
    public DateTimeOffset? CompletedAt;
    public ReportExecutionStatus Status;
    public string? ArtifactUrl;                                  // signed URL 7 day TTL
    public string? ErrorMessage;
}
```

Quartz worker cross-module Dapper aggregation çalıştırır, MinIO'ya yükler, email/notification.

### `SystemVersion` AR

```csharp
public class SystemVersion
{
    public Guid Id;
    public Platform Platform;                                    // Web/Android/iOS
    public string Version;                                        // semver "2.4.3"
    public string MinSupportedVersion;                            // "2.0.0"
    public bool ForceUpdate;                                      // < MinSupported ise client block
    public Translations ReleaseNotes;
    public DateTimeOffset ReleasedAt;
    public bool IsActive;                                         // current per platform
    public Guid CreatedByUserId;
    public DateTimeOffset CreatedAt;
    
    public static SystemVersion Create(Platform p, string version, string minSupported, Translations notes, bool forceUpdate, Guid actorAdminId) { ... }
    
    public void Activate(Guid actorAdminId) 
    { 
        // Repository transactional: aynı platform diğer Active'ler IsActive=false
    }
    public void Deactivate(Guid actorAdminId) { ... }
    public void UpdateMinSupported(string min, Guid actorAdminId) { /* dikkat — client base'ini etkiler */ }
}

public enum Platform { Web = 1, Android = 2, iOS = 3 }
```

**Constraint:** Aynı platform için max 1 active version. `Activate` çağrısı eskilerini deaktive eder (transactional).

### `ImpersonationSession` AR

```csharp
public class ImpersonationSession
{
    public Guid Id;
    public Guid AdminUserId, TargetUserId;
    public string Justification;                                 // zorunlu — audit
    
    public DateTimeOffset StartedAt;
    public DateTimeOffset MaxEndAt;                              // StartedAt + 4 hours (hard cap)
    public DateTimeOffset? EndedAt;
    public ImpersonationEndReason? EndReason;
    
    public string IpAddress, UserAgent;
    public string ImpersonationTokenJti;                         // JWT jti for revocation
    
    public ImpersonationStatus Status;
    
    public static ImpersonationSession Start(
        Guid adminUserId, Guid targetUserId, string justification,
        string ipAddress, string userAgent, string jti)
    {
        if (string.IsNullOrWhiteSpace(justification))
            throw new DomainException("Justification required");
        var now = DateTimeOffset.UtcNow;
        return new ImpersonationSession
        {
            Id = Guid.CreateVersion7(),
            /* ... */
            MaxEndAt = now.AddHours(4),
            Status = ImpersonationStatus.Active,
        };
    }
    
    public void End(ImpersonationEndReason reason)
    {
        if (Status != ImpersonationStatus.Active) return;
        Status = reason == ImpersonationEndReason.Expired 
            ? ImpersonationStatus.Expired 
            : ImpersonationStatus.Ended;
        EndedAt = DateTimeOffset.UtcNow;
        EndReason = reason;
    }
}

public enum ImpersonationStatus { Active = 1, Ended = 2, Expired = 3 }
public enum ImpersonationEndReason { Manual = 1, Expired = 2, AdminLoggedOut = 3, SecurityForced = 4 }
```

### Impersonation Flow

```
POST /admin/impersonation/start { targetUserId, justification }
   ↓ Validators:
     - actor "admin" role
     - target NOT admin (invariant)
     - active session yok (1 admin = 1 active)
     - justification non-empty
   ↓ Identity new JWT issue:
     - sub = targetUserId
     - act = adminUserId (RFC 8693)
     - imp = true
     - jti = new ID
     - exp = now + 4h
   ↓ ImpersonationSession.Start(...)
   ↓ AdminAuditLog: "impersonation.started"
   ↓ Response: { impersonationToken, expiresAt }

End:
   - POST /admin/impersonation/end → session.End(Manual) + jti blacklist
   - 4h dolar → Quartz job session.End(Expired) + jti blacklist
```

---

## 4. AdminAuditLog Entity (Logging Concern, AR Değil)

```csharp
public class AdminAuditLog
{
    public Guid Id;
    public Guid AdminUserId;
    public Guid? ImpersonationSessionId;                         // impersonation context varsa
    public string Action;                                         // "user.suspend", "listing.approve"
    public string TargetEntityType;                               // "user", "listing", "deal"
    public Guid? TargetEntityId;
    public string? Details;                                       // JSONB
    public string IpAddress, UserAgent;
    public DateTimeOffset CreatedAt;
    
    public static AdminAuditLog Record(...) { /* construct only — append-only */ }
}
```

**Append-only:** EF Core update behavior yok. Faz 2 DB-level `REVOKE UPDATE, DELETE ON admin.audit_log FROM livestock_app` (hardening Backlog).

### `IAdminAuditService`

```csharp
public interface IAdminAuditService
{
    Task RecordAsync(
        string action, string targetEntityType, Guid? targetEntityId,
        object? details, CancellationToken ct);
}

public sealed class AdminAuditService : IAdminAuditService
{
    public async Task RecordAsync(...)
    {
        var adminUserId = _currentUser.GetUserId()!.Value;
        var imp = _currentUser.GetImpersonationSessionId();
        var ip = _http.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "";
        var ua = _http.HttpContext?.Request.Headers["User-Agent"].ToString() ?? "";
        
        var log = AdminAuditLog.Record(adminUserId, imp, action, entityType, entityId, details, ip, ua);
        _db.AdminAuditLogs.Add(log);
        await _db.SaveChangesAsync(ct);
    }
}
```

Faz 1 explicit her IAdminXCommands method'unda `_audit.RecordAsync(...)`. Faz 2 aspect/interceptor pattern (Backlog #151).

---

## 5. Cross-Module Sync Interfaces (Görev 1/G)

Her modülün `Shared.Contracts.{Module}.Admin/` altında 2 interface:

| Modül | `IAdminXReadService` | `IAdminXCommands` |
|---|---|---|
| Catalog | `IAdminCatalogReadService` | `IAdminCatalogCommands` (CRUD Category/Breed/Brand/BorderRule + CategoryAttribute) |
| Identity | `IAdminUserReadService` | `IAdminUserCommands` (suspend/role grant/force-logout/impersonate) |
| Accounts/Seller | `IAdminSellerReadService` | `IAdminSellerCommands` (verify/reject/suspend/document review/cert review) |
| Accounts/Vet | `IAdminVetReadService` | `IAdminVetCommands` |
| Carrier | `IAdminCarrierReadService` | `IAdminCarrierCommands` |
| Subscription | `IAdminSubscriptionReadService` (MRR, churn, invoices) | `IAdminSubscriptionCommands` (refund, plan manage) |
| Listings | `IAdminListingReadService` | `IAdminListingCommands` (approve/reject, report resolve) |
| Marketplace | `IAdminMarketplaceReadService` (disputes, GMV) | `IAdminMarketplaceCommands` (deal cancel, dispute resolve, refund) |
| Messaging | `IAdminMessagingReadService` (reports) | `IAdminMessagingCommands` (report resolve, message delete) |
| Notifications | `INotificationsReadService` | `IAdminNotificationsCommands` (template save, send-ad-hoc) |

**Toplam: ~20 interface (10 modül × 2).**

---

## 6. Cross-Module Dashboard Aggregator

```csharp
public sealed record DashboardResponse(
    DashboardCounters Counters,
    KpiTimeseries Mrr,
    KpiTimeseries Gmv,
    IReadOnlyList<RecentActivityItem> ActivityFeed,
    IReadOnlyList<GeoBreakdownItem> CountryBreakdown);

public sealed record DashboardCounters(
    int ListingsPendingReview, int SellersPendingVerification,
    int VetsPendingVerification, int CarriersPendingVerification,
    int OpenDisputes, int OpenListingReports, int OpenMessageReports,
    int FailedInvoicesLast24h,
    
    int TotalUsers, int ActiveUsersLast7Days,
    int ActiveListings, int ActiveSubscriptions,
    Money TotalGmvLast30Days, Money MrrCurrent,
    decimal ChurnRateLast30Days);

public sealed class GetDashboardHandler
{
    public async Task Consume(...)
    {
        var ct = ctx.CancellationToken;
        
        // PARALEL cross-module read (~11 service)
        var (lp, sp, vp, cp, od, lr, mr, fi, idStats, mrr, gmv) = await ParallelAggregate(
            _listingsRead.GetPendingReviewCountAsync(ct),
            _sellerRead.GetPendingVerificationCountAsync(ct),
            _vetRead.GetPendingVerificationCountAsync(ct),
            _carrierRead.GetPendingVerificationCountAsync(ct),
            _marketplaceRead.GetOpenDisputeCountAsync(ct),
            _listingsRead.GetOpenReportCountAsync(ct),
            _messagingRead.GetOpenReportCountAsync(ct),
            _subscriptionRead.GetFailedInvoiceCountLastDaysAsync(1, ct),
            _identityRead.GetUserStatsAsync(ct),
            _subscriptionRead.GetMrrAsync(ct),
            _marketplaceRead.GetGmvAsync(periodDays: 30, ct));
        
        return BuildResponse(...);
    }
}
```

**Cache:** Dashboard response 1 dakika Redis, counter'lar 30sn TTL.

---

## 7. AdminUser ≠ AR — Identity Üzerinden

| Operasyon | Endpoint | Pattern |
|---|---|---|
| Admin kullanıcı oluştur | `IAdminUserCommands.GrantRoleAsync(userId, "admin", actorId)` | Identity |
| Admin role revoke | `IAdminUserCommands.RevokeRoleAsync(userId, "admin", actorId)` | Identity |
| İlk admin (bootstrap) | `Tools/AdminBootstrap` CLI | CLI script, secured |
| Admin login | Standart `/identity/auth/login` (JWT has "admin" role) | Identity |
| Admin 2FA Faz 2 zorunlu | Identity v2 patch | Faz 2 |

---

## 8. Cross-Modül Map

```
                [10 modülün IAdminXReadService] ────┐
                                                       │
                [10 modülün IAdminXCommands] ─────────┤
                                                       │
                                       sync read+write
                                                       │
                                                       ▼
                                              ┌────────────┐
              RabbitMQ Public events    ─────►│   Admin    │
              (read model projection         │  Module    │
               için consumer'lar)             │            │
                                              │  Own data: │
                                              │  Dashboard, FeatureFlag,
                                              │  Reports, SystemVersion,
                                              │  Impersonation, AuditLog
                                              └────────────┘
                                                       │
                                                       ▼ (publishes)
                                              Public events YOK
```

---

## 9. Public Event'ler (0 — Terminal)

Admin'in Public event'i **YOK**. Internal events (FeatureFlagToggled, ImpersonationStarted, AdminAuditLogRecorded) sadece kendi consumer'ları dinler.

Cross-modül etki **`IAdminXCommands` synchronous call** üzerinden.

---

## 10. API Endpoint Inventory

### Dashboard (2)

`GET /admin/dashboard` + `/timeseries?metric=&period=`.

### Feature Flags (6)

`GET /admin/feature-flags?cursor=` + `/{id}` + `POST` + `PATCH /{id}` + `/toggle` + `DELETE`.

### Public Feature Flag (1)

`GET /feature-flags/me` — current user's resolved flag map.

### Scheduled Reports (7)

`GET /admin/reports?cursor=` + `POST` + `/{id}` + `PATCH/DELETE` + `/run-now` + `/executions/{execId}/download`.

### System Versions (5)

`GET /admin/system-versions?platform=` + `POST` + `PATCH /{id}` + `/{id}/activate` + (Public) `GET /system-versions/check?platform=&version=`.

### Impersonation (4)

`POST /admin/impersonation/start` + `/end` + `GET /active` + `/history?cursor=`.

### Audit Log (2)

`GET /admin/audit-log?adminUserId=&action=&from=&to=&cursor=` + `/{id}`.

**Admin kendi endpoint: 27.**

**Cross-module admin endpoint:** 9 modül + Admin own × admin endpoint sayıları toplamı **74**.

**Toplam admin: 27 + 74 = 101.**

---

## 11. AdminAuditLog Insertion Disiplini

Her `IAdminXCommands` method'unun sonu:

```csharp
public async Task<Result> SuspendAsync(Guid userId, string reason, Guid actorId, CancellationToken ct)
{
    var user = await _db.Users.FindAsync(userId);
    if (user is null) return Result.NotFound();
    
    user.Suspend(reason, actorId);
    await _publish.Publish(new UserSuspended(...), ct);
    await _uow.SaveChangesAsync(ct);
    
    // Audit log — EXPLICIT Faz 1
    await _audit.RecordAsync(
        action: "user.suspend",
        targetEntityType: "user",
        targetEntityId: userId,
        details: new { reason },
        ct);
    
    return Result.Success();
}
```

PR review checklist (Karar 7/Process): IAdminXCommands write → audit.RecordAsync().

---

## 12. Discovered Backlog

| # | Konu | Hedef |
|---|---|---|
| 151 | Aspect/interceptor audit Faz 2 (`[AuditAction]` attribute) | Karar 7 |
| 152 | Feature flag cache invalidation (Redis pub/sub Faz 2) | Karar 7 / Performance |
| 153 | ScheduledReport Dapper sandbox Faz 2 (admin SQL input sanitize, RO connection) | Karar 7 / Security |
| 154 | Impersonation 2FA enforcement Faz 2 | Karar 7 / Security |
| 155 | AdminAuditLog DB-level append-only constraint Faz 2 (REVOKE UPDATE/DELETE) | Karar 7 / Security |
| 156 | Cross-module DataExportContributor admin contribution (impersonation log dahil) | Karar 5 / Identity ek |
| 157 | Dashboard parallel aggregator circuit breaker (Polly) | Karar 7 / Resilience |
| 158 | Dashboard refresh strategy (polling 5sn vs WS push Faz 2) | Karar 7 / Realtime Faz 2 |

---

## 13. Özet Tablo

| Konu | Karar |
|---|---|
| AR sayısı | 4 (FeatureFlag, ScheduledReport, SystemVersion, ImpersonationSession) |
| Logging entity | AdminAuditLog (append-only) |
| Public events | 0 (terminal aggregator) |
| AdminUser AR | YOK — Identity.User + role |
| AdminRole AR | YOK — Identity.UserRole |
| Cross-modül write | `IAdminXCommands` sync (Görev 1/G) — 10 modül × ayrı interface |
| Cross-modül read | `IAdminXReadService` sync — 10 modül |
| Dashboard | Cross-module parallel aggregator (~11 service), Redis 1dk cache |
| Feature flag | Stable hash bucket, targeting (user/country/role/percentage/time) |
| Impersonation | RFC 8693 actor claim, 4h max, jti blacklist on end; target NOT admin |
| AdminAuditLog | Faz 1 explicit `_audit.RecordAsync()`; Faz 2 aspect interceptor |
| Endpoint | 27 (Admin kendi) + 74 (cross-module admin in respective modules) = 101 toplam |
