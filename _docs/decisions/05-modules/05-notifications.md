# Karar 5 / Notifications Modülü

**Status:** FINAL
**Wave:** 6 (parallel with Marketplace + Messaging)

## İlişkili Kararlar

- **Üst:** [Karar 2](../02-modules-list.md), [Karar 3a — AR](../03-domain-patterns.md)
- **Patch:** [Patch 5 — Görev 3 düzeltmesi](../05-patch.md) — delivery feedback events producer = Notifications (önceki Identity'de listelenmişti)
- **Frontend:** `frontend-api-inventory.md` Notifications (11 endpoint — list, read-all, preferences, DND, digest)

---

## 1. Modülün Rolü ve Sınırları

### Sahip

| Konsept | Sahiplik |
|---|---|
| NotificationPreference (per-user channel + DND + digest) | Notifications |
| InAppNotification (UI-visible item) | Notifications |
| NotificationTemplate (key × locale × channel matrix — entity-level CRUD) | Notifications |
| DeliveryAttempt (channel-bazlı audit log) | Notifications |
| Multi-channel dispatch (email/SMS/push/in-app) | Notifications |
| Dedup logic (flood prevention) | Notifications |
| Digest batching (instant/daily/weekly) | Notifications |
| `/notifications/hub` realtime stream | Notifications |

### Sahip Olmayan

| Konsept | Sahibi |
|---|---|
| KVKK MarketingEmail consent | Identity (`UserConsent.MarketingEmail`) |
| Push device tokens | Identity (`UserDevice`) |
| Email provider connection | Notifications.Infrastructure (MailKit + Brevo SMTP) |
| SMS provider | Notifications.Infrastructure (Twilio) |
| Push provider (FCM/APNS) | Notifications.Infrastructure |

---

## 2. Aggregate Roots

### `NotificationPreference` AR

```csharp
public class NotificationPreference
{
    public Guid Id, UserId;
    
    // DND
    public bool DndEnabled;
    public TimeOnly? DndStart, DndEnd;                           // local time
    // TimeZone Identity.UserPreferences.TimeZone'dan
    
    // Per-category channel mask (JSONB)
    public string ChannelPreferencesJson;                         // { "message": { email: false, sms: false, push: true, inApp: true }, ...}
    
    // Per-category digest frequency
    public string DigestPreferencesJson;                          // { "marketing": "weekly", ... }
    
    public DateTimeOffset CreatedAt, UpdatedAt;
    
    public bool IsChannelEnabled(NotificationCategory cat, DeliveryChannel ch) { /* parse JSON */ }
    public DigestFrequency GetDigestFrequency(NotificationCategory cat) { ... }
    
    public bool IsInDndWindow(TimeZoneInfo tz, DateTimeOffset utcNow)
    {
        if (!DndEnabled || DndStart is null || DndEnd is null) return false;
        var local = TimeZoneInfo.ConvertTime(utcNow, tz);
        var localTime = TimeOnly.FromDateTime(local.DateTime);
        return DndStart < DndEnd 
            ? localTime >= DndStart && localTime <= DndEnd
            : localTime >= DndStart || localTime <= DndEnd;
    }
    
    public static NotificationPreference CreateDefault(Guid userId) 
    {
        return new NotificationPreference
        {
            Id = Guid.CreateVersion7(), UserId = userId,
            DndEnabled = false,
            ChannelPreferencesJson = JsonSerializer.Serialize(BuildDefaultChannelPrefs()),
            DigestPreferencesJson = JsonSerializer.Serialize(BuildDefaultDigestPrefs()),
        };
    }
    
    private static Dictionary<NotificationCategory, ChannelMask> BuildDefaultChannelPrefs() => new()
    {
        [NotificationCategory.Message]      = new(Email: false, Sms: false, Push: true, InApp: true),
        [NotificationCategory.Offer]        = new(Email: true,  Sms: false, Push: true, InApp: true),
        [NotificationCategory.Sale]         = new(Email: true,  Sms: true,  Push: true, InApp: true),
        [NotificationCategory.Verification] = new(Email: true,  Sms: false, Push: true, InApp: true),
        [NotificationCategory.System]       = new(Email: true,  Sms: false, Push: true, InApp: true),
        [NotificationCategory.Marketing]    = new(Email: true,  Sms: false, Push: false, InApp: false),
        [NotificationCategory.Shipment]     = new(Email: false, Sms: true,  Push: true, InApp: true),
    };
    
    public void UpdateChannelPreferences(IReadOnlyDictionary<NotificationCategory, ChannelMask> prefs) { ... }
    public void UpdateDigestPreferences(IReadOnlyDictionary<NotificationCategory, DigestFrequency> prefs) { ... }
    public void EnableDnd(TimeOnly start, TimeOnly end) { ... }
    public void DisableDnd() { ... }
}

public enum NotificationCategory
{
    Message = 1,        // mesaj (Messaging)
    Offer = 2,          // teklif (Marketplace)
    Sale = 3,           // anlaşma + ödeme + tamamlanma (Marketplace)
    Verification = 4,   // seller/vet/carrier (Accounts/Carrier)
    System = 5,         // hesap, güvenlik, sistem
    Marketing = 6,      // KVKK opt-in zorunlu
    Shipment = 7        // kargo (Carrier)
}

public enum DeliveryChannel { Email = 1, Sms = 2, Push = 4, InApp = 8 }  // bitwise flag

public sealed record ChannelMask(bool Email, bool Sms, bool Push, bool InApp);

public enum DigestFrequency { Instant = 1, Daily = 2, Weekly = 3, Never = 4 }
```

### `InAppNotification` AR

```csharp
public class InAppNotification
{
    public Guid Id, UserId;
    public NotificationCategory Category;
    public string TemplateKey;
    public NotificationPriority Priority;
    
    public string TitleSnapshot, BodySnapshot;                   // locale-rendered at insert
    public string? Icon, ActionUrl;
    public string? MetadataJson;                                  // { dealId, amount, ... }
    
    public InAppNotificationStatus Status;
    public DateTimeOffset CreatedAt;
    public DateTimeOffset? ReadAt, ArchivedAt;
    public DateTimeOffset ExpiresAt;                              // 90 day default
    
    public static InAppNotification Create(...) { ... }
    public void MarkRead() { Status = InAppNotificationStatus.Read; ReadAt = DateTimeOffset.UtcNow; }
    public void Archive() { Status = InAppNotificationStatus.Archived; ArchivedAt = DateTimeOffset.UtcNow; }
}

public enum InAppNotificationStatus { Unread = 1, Read = 2, Archived = 3 }
public enum NotificationPriority { Low = 1, Normal = 2, High = 3, Critical = 4 }
```

**Expiry cron:** Daily — `expires_at < now` → hard delete.

---

## 3. Entities (AR Değil)

### `NotificationTemplate` (Logging-Style, Admin CRUD)

```csharp
public class NotificationTemplate
{
    public Guid Id;
    public string Key;                                            // "offer.received"
    public LanguageCode Locale;
    public DeliveryChannel Channel;
    public NotificationCategory Category;
    public NotificationPriority DefaultPriority;
    
    public string? Subject;                                       // email only
    public string Body;                                            // {{variable}} substitution
    public string? Icon, ActionUrlTemplate;
    
    public bool IsCriticalOverride;                               // bypass DND
    public bool IsMarketing;                                       // requires MarketingEmail consent
    public bool IsActive;
    public int Version;                                            // her update'te +1
    
    public DateTimeOffset CreatedAt, UpdatedAt;
}
```

DB: `UNIQUE(key, locale, channel)`. AR değil — domain event üretmiyor, lifecycle minimal admin CRUD.

### `DeliveryAttempt` (Append-Only Log)

```csharp
public class DeliveryAttempt
{
    public Guid Id;
    public Guid UserId;
    public NotificationCategory Category;
    public string TemplateKey;
    public DeliveryChannel Channel;
    public DeliveryStatus Status;
    public string? ProviderResponse;                              // Mail-ID, SMS-SID, FCM message-ID
    public string? FailureReason;
    public string? DedupHash;
    public DateTimeOffset AttemptedAt;
    public DateTimeOffset? CompletedAt;
}

public enum DeliveryStatus { Pending = 1, Sent = 2, Failed = 3, Bounced = 4, Suppressed = 5 }
```

### `DigestQueueItem`

```csharp
public class DigestQueueItem
{
    public Guid Id, UserId;
    public NotificationCategory Category;
    public string TemplateKey;
    public string RenderedTitle, RenderedBody;
    public string? ActionUrl;
    public DateTimeOffset QueuedAt;
    public DigestFrequency Frequency;
    public DateTimeOffset? DispatchedAt;
}
```

---

## 4. Multi-Channel Dispatch Flow

```
Public event fired (örn. OfferSubmitted)
   ↓
Notifications consumer: OfferSubmittedHandler
   ↓
1. Resolve recipient userId
2. Load NotificationPreference (Redis cache 5dk, miss → DB)
3. Load user.Locale (Identity prefs) — cached
4. Load templates: key="offer.received", locale=user.Locale, channels={Email, Sms, Push, InApp}
5. Per-channel evaluate:
   - Channel enabled in prefs?
   - Marketing && !consent.MarketingEmail → skip (KVKK)
   - DND active && !template.IsCriticalOverride → digest queue (or skip)
   - DigestFrequency != Instant → push to digest_queue (NOT immediate send)
   - Dedup check (5min hash) → skip if recent
6. Per-channel dispatch:
   - Email: MailKit → Brevo SMTP
   - SMS: Twilio
   - Push: FCM (Android) + APNS via FCM proxy (iOS Faz 1; direct APNS Faz 2)
   - InApp: DB insert InAppNotification + SignalR push
7. DeliveryAttempt log per channel
8. Failure handling:
   - Email hard bounce → publish EmailBounced Public event
   - SMS permanent fail → publish SmsDeliveryFailed
   - FCM "InvalidRegistration" → publish PushTokenInvalidated
```

### Dedup Pattern

```csharp
public sealed class DedupService
{
    public string ComputeHash(Guid userId, string templateKey, IReadOnlyDictionary<string, object> contextKey)
    {
        var input = $"{userId}|{templateKey}|{string.Join(",", contextKey.Select(kv => $"{kv.Key}={kv.Value}"))}";
        return SHA256(input);
    }
    
    public async Task<bool> IsRecentlyDispatchedAsync(string hash, TimeSpan window, CancellationToken ct)
    {
        var key = $"notifications:dedup:{hash}";
        var acquired = await _redis.SetIfNotExistsAsync(key, "1", window);
        return !acquired;
    }
}
```

Window default 5 min per template.

---

## 5. DND + Critical Override

```
Event geliyor → dispatch evaluator
   ↓
user_prefs.IsInDndWindow(timezone, utcNow):
   ↓ true
   template.IsCriticalOverride:
      ↓ true → dispatch immediately (override)
      ↓ false → digest_queue (instant kanal skip)
```

### Critical-Override Templates (Faz 1)

| Template Key | Sebep |
|---|---|
| `auth.security_alert` | Şifre değişti, suspicious login |
| `payment.failed` | Ödeme başarısız, kullanıcı aksiyon almalı |
| `deal.disputed` | Anlaşmazlık açıldı |
| `deal.urgent_action` | Buyer confirm receipt bekleniyor 7gün |
| `subscription.expired` | Hesap free tier'a düştü |
| `shipment.cancelled` | Carrier iptal, aksiyon gerekli |

---

## 6. Digest Batching

### Cron Jobs

```csharp
[Quartz.JobKey("DailyDigest")]
public sealed class DailyDigestJob : IJob
{
    // Every day at 08:00 user local time (Faz 2 timezone-aware)
    public async Task Execute(...)
    {
        // Group by userId, build summary email
        // Send aggregated digest
        // Mark items dispatched
    }
}

[Quartz.JobKey("WeeklyDigest")]
public sealed class WeeklyDigestJob : IJob { /* Monday 09:00 */ }
```

Faz 1 single timezone (UTC); Faz 2 per-user timezone bucketing (Backlog #147).

---

## 7. SignalR `/notifications/hub`

### Auth

JWT Bearer `?access_token=<jwt>` query string (SignalR standard).

```csharp
[Authorize]
public sealed class NotificationsHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = _currentUser.GetUserId()!.Value;
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId}");
        
        var roles = _currentUser.GetRoles();
        if (roles.Contains("moderator") || roles.Contains("admin"))
            await Groups.AddToGroupAsync(Context.ConnectionId, "admin-moderation");
        if (roles.Contains("admin"))
            await Groups.AddToGroupAsync(Context.ConnectionId, "superadmin");
    }
}
```

### Server → Client Events

- `NotificationReceived` { id, category, priority, title, body, icon, actionUrl, createdAt }
- `UnreadCountChanged` { total }
- `AccountStatusChanged` { newStatus, reason? }
- `ListingStatusChanged`, `OfferStatusChanged`, `DealStatusChanged`, `SubscriptionStatusChanged`
- `ForcedDisconnect` { reason } — admin suspend cascade

**Connection lifetime ≠ token lifetime:** WS connection long-lived, server explicit `ForcedDisconnect` ile drop edilir (suspension/logout cascade).

**Auto-reconnect:** SignalR `withAutomaticReconnect([0, 2000, 5000, 10000, 30000])`.

**Missed messages (Faz 1):** Client reconnect → `GET /me/notifications/unread-since=` pull. Faz 2: SignalR Streams + Last-Event-Id replay buffer.

---

## 8. Marketing Opt-in KVKK Cross-Check

```csharp
// Marketing template dispatch evaluator
if (template.IsMarketing)
{
    var hasConsent = await _identityRead.HasActiveConsentAsync(
        userId, ConsentType.MarketingEmail, ct);
    if (!hasConsent)
    {
        _logger.LogDebug("Marketing notification skipped — no consent");
        return;
    }
}
```

**Çift kontrol:**
1. NotificationPreference (`channelPrefs[Marketing][Email]=true`) — kullanıcı opt-in
2. Identity.UserConsent.MarketingEmail — KVKK consent

İkisi de true ise gönderilir. Identity consent revoke → Notifications prefs kalsa da blok (KVKK priority).

---

## 9. Realtime Push Catalog (80 Event)

80 Public event'ten ~58'i realtime push'a sahip; detay [06-api-contract.md / Madde 6](../06-api-contract.md#madde-6--realtime-push-catalog).

| Hub | Push Sayısı |
|---|---|
| `/notifications/hub` | ~54 |
| `/messaging/hub` | 3 (MessageSent/Read/ConversationStarted + TypingIndicator pure WS) |
| admin-moderation group | ~8 |

---

## 10. Template Seed (Faz 1)

~30 template × 4 locale × 2-3 channel = **~250 row**.

```
auth.*           — register welcome, email_verified, password_changed, security_alert
offer.*          — submitted, accepted, rejected, countered, withdrawn, expired
deal.*           — created, paid, in_preparation, in_transit, delivered, completed, 
                    cancelled, disputed, resolved, urgent_action
shipment.*       — created, picked_up, delivered, cancelled
verification.*   — submitted, approved, rejected, suspended (seller, vet, carrier)
subscription.*   — activated, renewed, expired, cancelled, upgraded, invoice_paid, 
                    invoice_failed, boost_activated
payment.*        — method_added, failed, refunded
review.*         — received_review, reply_received
marketing.*      — weekly_digest, new_features (Faz 2 daha çok)
system.*         — maintenance, terms_updated, force_update
```

Seed: TR + EN tam, AR + RU "varsa" (TranslationHelper fallback "en").
Dosya: `Tools/SeedRunner/SeedData/notifications/templates.json`.

---

## 11. Cross-Modül Erişim

### Tükettiği

| Modül | Method |
|---|---|
| Identity | `GetUserSummaryAsync` (locale, timezone, name) |
| Identity | `HasActiveConsentAsync(MarketingEmail)` |
| Identity | `GetActivePushTokensAsync` |
| Catalog | `GetCurrencyAsync` (Money format) |
| **Tüm modüller (event)** | RabbitMQ Public event consume |

### Sunduğu

```csharp
public interface INotificationsReadService
{
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken ct);
    Task<NotificationPreferenceSummary> GetPreferencesAsync(Guid userId, CancellationToken ct);
}

public interface IAdminNotificationsCommands
{
    Task<Result> SendAdHocNotificationAsync(
        Guid recipientUserId, string templateKey, IReadOnlyDictionary<string, object> context,
        Guid adminUserId, CancellationToken ct);
    Task<Result> SaveTemplateAsync(NotificationTemplateDto dto, Guid adminUserId, CancellationToken ct);
    Task<Result> DeactivateTemplateAsync(Guid templateId, Guid adminUserId, CancellationToken ct);
}
```

---

## 12. Public Event'ler (3 — Delivery Feedback)

**Patch 5 düzeltmesi:** Producer = Notifications, Consumer = Identity.

| Event | Payload | Consumer |
|---|---|---|
| `EmailBounced` | UserId, Email, BounceType (hard/soft), Reason, BouncedAt | Identity (hard bounce → email_verified=false) |
| `SmsDeliveryFailed` | UserId, PhoneNumber, Reason, IsPermanent | Identity (permanent → phone flag) |
| `PushTokenInvalidated` | UserId, DeviceToken, Reason | Identity (UserDevice.PushToken null) |

**Internal:** NotificationPreferenceUpdated, InAppNotificationCreated/Read/Archived.

---

## 13. API Endpoint Inventory (16)

### Authenticated — InApp (5)

`GET /me/notifications?category=&status=&cursor=` + `/unread-count` + `POST /{id}/read` + `/read-all` + `/{id}/archive`.

### Authenticated — Preferences (2)

`GET/PATCH /me/notification-preferences`.

### Admin (5)

`GET /admin/notifications/templates?cursor=&channel=&category=` + `/{id}` + `PATCH /{id}` + `POST /send-adhoc` + `GET /delivery-attempts?userId=&cursor=`.

### WebSocket (1)

`WS /notifications/hub`.

### Webhook (3)

`POST /notifications/webhooks/email-bounce` + `/sms-status` + `/push-feedback`.

---

## 14. Discovered Backlog

| # | Konu | Hedef |
|---|---|---|
| 142 | Görev 3 Communication Matrix Notifications producer (kapandı) | ✓ |
| 143 | Email provider seçimi (Brevo vs Resend) | Karar 7 / External Faz 1 |
| 144 | Push provider FCM v1 API + APNS HTTP/2 token rotation | Karar 7 / Infrastructure |
| 145 | Twilio SMS country routing (cost optimization) | Karar 7 |
| 146 | Template variable engine (basit `{{var}}` Faz 1; Razor/Liquid Faz 2) | Karar 5 / Notifications Faz 2 |
| 147 | Per-user timezone digest run (Faz 2 bucket scheduler) | Karar 7 |
| 148 | Notification rate limit per user (flood guard, abuse detection) | Karar 7 / Anti-abuse |
| 149 | WhatsApp Business API Faz 2 (DeliveryChannel.WhatsApp) | Karar 7 / Channel expansion |
| 150 | Delivery attempt observability (Sentry failure alerts) | Karar 7 / Observability |

---

## 15. Özet Tablo

| Konu | Karar |
|---|---|
| AR sayısı | 2 (NotificationPreference, InAppNotification) |
| Diğer entity | NotificationTemplate (admin CRUD), DeliveryAttempt (append-only), DigestQueueItem |
| Public events | 3 (delivery feedback — producer Notifications) |
| Categories | 7 (Message/Offer/Sale/Verification/System/Marketing/Shipment) |
| Channels | 4 (Email/Sms/Push/InApp); WhatsApp Faz 2 |
| DND | Time window + critical override |
| Digest | instant/daily/weekly/never per-category |
| Dedup | 5 min window per (userId + templateKey + context) hash |
| Marketing | NotificationPreference + Identity.UserConsent çift kontrol |
| Hub | `/notifications/hub` JWT query auth |
| Templates | ~30 × 4 locale × 2-3 channel = ~250 row seed |
| InApp retention | 90 days |
| Endpoint | 16 (5 InApp + 2 Prefs + 5 Admin + 1 WS + 3 webhook) |
