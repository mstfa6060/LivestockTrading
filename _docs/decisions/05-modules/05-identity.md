# Karar 5 / Identity Modülü (v2)

**Status:** FINAL (v1 → v2 revize)
**Wave:** 2

## İlişkili Kararlar

- **Üst:** [Karar 2](../02-modules-list.md), [Karar 3a — AR](../03-domain-patterns.md#bölüm-1-aggregate-root-listesi-3a)
- **Patch:** [Patch 1 — Identity v2](../05-patch.md) — UserPreferences (ReadReceiptsEnabled + TypingIndicatorEnabled), PendingEmail field, GDPR data export
- **Frontend:** `frontend-api-inventory.md` Identity (31 endpoint — 3-method login, KVKK, phone OTP, OAuth)
- **v1 → v2 değişimi:** e-Devlet/TARSİM SSO TAMAMEN İPTAL; 3-method login (email/phone/nationalId); KVKK consent; phone OTP flow; GDPR data export; `/social/` → `/oauth/` rename; PendingEmail change flow; /me/sessions enrichment (GeoIP + UA parse); VetProfile manual verification (AccountType=Vet self-declared, role grant Accounts.VetVerified event sonrası)

---

## 1. Modülün Rolü ve Sınırları

### Sahip

| Konsept | Sahiplik |
|---|---|
| Authentication credentials (password, refresh token, social link, phone OTP) | Identity |
| User identity (email, name, phone, **TC kimlik no**) | Identity |
| **AccountType** (Producer/Trader/Vet/Buyer — register, immutable) | Identity |
| **KVKK consents** (terms, ministry data share, marketing) | Identity |
| User preferences (locale, currency, country, timezone, **ReadReceiptsEnabled**, **TypingIndicatorEnabled**) | Identity |
| User devices (auth session + push token — Backlog #6 kararı) | Identity |
| User roles (cross-modül RBAC source of truth) | Identity |
| OpenIddict OAuth2/OIDC server | Identity |
| Email/phone verification flows | Identity |
| **GDPR data export orchestration** (IDataExportContributor pattern) | Identity |

### Sahip Olmayan

| Konsept | Sahibi |
|---|---|
| Seller business profile, farms | Accounts |
| Carrier persona | Carrier |
| Vet professional verification data (license, diploma) | Accounts (VetProfile AR) |
| Notification channel preferences | Notifications |
| Subscription | Subscription |
| Avatar physical storage | Shared IFileStorage → MinIO |

---

## 2. Aggregate Roots

### `User` AR

```csharp
public class User
{
    public Guid Id { get; private set; }                              // Guid v7
    public EmailAddress Email { get; private set; }
    public EmailAddress? PendingEmail { get; private set; }           // v2 — email change flow
    public DateTimeOffset? PendingEmailRequestedAt { get; private set; }
    public HashedPassword? Password { get; private set; }              // social-only user'da null
    public PersonName Name { get; private set; }
    public PhoneNumber? Phone { get; private set; }
    public NationalId? NationalId { get; private set; }                // v2 — TR TC kimlik opsiyonel
    public AccountType AccountType { get; private set; }                // v2 — IMMUTABLE
    public UserPreferences Preferences { get; private set; }
    public string? AvatarUrl { get; private set; }
    
    public UserStatus Status { get; private set; }
    public DateTimeOffset? EmailVerifiedAt { get; private set; }
    public DateTimeOffset? PhoneVerifiedAt { get; private set; }
    public DateTimeOffset? SuspendedAt { get; private set; }
    public string? SuspendedReason { get; private set; }
    public Guid? SuspendedByUserId { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public DateTimeOffset? AnonymizedAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public DateTimeOffset? LastLoginAt { get; private set; }
    
    // Faz 2 placeholder
    public string? TotpSecretEncrypted { get; private set; }
    public bool TwoFactorEnabled { get; private set; }
    
    // Child collections
    private readonly List<RefreshToken> _refreshTokens = new();
    private readonly List<UserDevice> _devices = new();
    private readonly List<UserExternalLogin> _externalLogins = new();
    private readonly List<UserRole> _roles = new();
    private readonly List<UserConsent> _consents = new();              // v2 — KVKK
    
    // Factories
    public static User RegisterWithPassword(
        EmailAddress email, HashedPassword password, PersonName name,
        PhoneNumber? phone, NationalId? nationalId, AccountType accountType,
        UserPreferences preferences, IReadOnlyList<ConsentGrant> consents,
        string ipAddress, string userAgent)
    {
        var user = new User { Id = Guid.CreateVersion7(), /* ... */ };
        
        // Default role grant by AccountType (Identity v2 Q3)
        user.GrantRole("buyer", grantedByUserId: null);
        if (accountType == AccountType.Vet)
        {
            // VetProfile manual verification gerekir — initial role sadece buyer
            // 'vet' role Accounts.VetVerified event'inde grant edilir
        }
        // Producer/Trader → seller role Accounts.SellerVerified event sonrası
        
        foreach (var c in consents)
            user.RecordConsent(c, ipAddress, userAgent);
        
        return user;
        // Public event: UserRegistered
    }
    
    public static User RegisterWithSocial(...)  { /* EmailVerifiedAt = now */ }
    
    // Consent (v2)
    public void RecordConsent(ConsentGrant grant, string ipAddress, string userAgent) { ... }
    public void RevokeConsent(ConsentType type) 
    { 
        // TermsAndPrivacy revoke = auto delete flow (KVKK)
        // MinistryDataShare revoke = Accounts vaccine-sync disable (Faz 2)
    }
    public bool HasActiveConsent(ConsentType type, string version) { ... }
    
    // NationalId (v2)
    public void AssignNationalId(NationalId nationalId)
    {
        if (NationalId is not null) throw new DomainException("NationalId already set");
        NationalId = nationalId;
    }
    
    // Email change (v2 — patch)
    public EmailVerificationToken RequestEmailChange(EmailAddress newEmail, TimeSpan ttl) { ... }
    public void ConfirmEmailChange(EmailAddress confirmedNewEmail) { ... }
    public void CancelPendingEmailChange() { ... }
    
    // Lifecycle
    public void VerifyEmail() { /* Public event: UserEmailVerified */ }
    public void VerifyPhone() { /* Internal event */ }
    public void ChangePassword(HashedPassword newPassword) { /* Public event: UserPasswordChanged */ }
    public void Suspend(string reason, Guid actorUserId) 
    { 
        Status = UserStatus.Suspended;
        // Revoke all refresh tokens + JWT blacklist
        RevokeAllRefreshTokens(RevocationReason.UserSuspended);
        // Public event: UserSuspended
    }
    public void Reactivate(Guid actorUserId) { /* Public event: UserReactivated */ }
    public void RequestDeletion() { /* PendingDeletion, 30-day grace */ }
    public void CancelDeletion() { /* restore */ }
    public void Anonymize() { /* GDPR — PII null, FK preserve */ }
    
    // Preferences
    public void UpdatePreferences(UserPreferences newPrefs) { /* Internal: UserPreferencesChanged */ }
    
    // Devices
    public UserDevice RegisterDevice(string platform, string? pushToken, string deviceFingerprint) { ... }
    public void InvalidatePushToken(Guid deviceId) { ... }
    public void RemoveDevice(Guid deviceId) { ... }
    
    // Roles
    public void GrantRole(string role, Guid? grantedByUserId) { /* Internal: UserRoleGranted */ }
    public void RevokeRole(string role, Guid revokedByUserId) { /* Internal: UserRoleRevoked */ }
    
    // RefreshTokens
    public RefreshToken IssueRefreshToken(Guid deviceId, TimeSpan ttl) { ... }
    public void RotateRefreshToken(Guid oldTokenId, RefreshToken newToken) { ... }
    public void RevokeRefreshToken(Guid tokenId, RevocationReason reason) { ... }
    public void RevokeAllRefreshTokens(RevocationReason reason) { ... }
}

public enum UserStatus
{
    EmailUnverified = 1, Active = 2, Suspended = 3, 
    PendingDeletion = 4, Deleted = 5
}

public enum AccountType
{
    Producer = 1,   // Üretici — seller onboarding bekliyor
    Trader = 2,     // Tüccar — seller onboarding bekliyor  
    Vet = 3,        // Veteriner — manual verification (Identity Q3)
    Buyer = 4       // Default
}
```

---

## 3. Child Entities (User AR İçinde)

### `RefreshToken`

```csharp
public class RefreshToken
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid DeviceId { get; private set; }
    public string TokenHash { get; private set; }         // SHA-256
    public Guid? ParentTokenId { get; private set; }      // rotation chain
    public Guid FamilyId { get; private set; }            // reuse detection
    public DateTimeOffset IssuedAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public RevocationReason? RevocationReason { get; private set; }
    public string? ReplacedByTokenId { get; private set; }
}

public enum RevocationReason
{
    Logout = 1, Rotation = 2, Reuse = 3,
    UserSuspended = 4, UserDeleted = 5, PasswordChanged = 6, AdminRevoked = 7
}
```

**TTL:** Access 15dk; Refresh 30 gün (rememberMe → 90 gün).
**Family detection:** Token reuse → tüm family revoke.

### `UserDevice`

```csharp
public class UserDevice
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Platform { get; private set; }          // "web", "android", "ios"
    public string? UserAgent { get; private set; }
    public string DeviceFingerprint { get; private set; } // hashed
    public string? PushToken { get; private set; }        // FCM/APNs
    public DateTimeOffset RegisteredAt { get; private set; }
    public DateTimeOffset LastSeenAt { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset? PushTokenInvalidatedAt { get; private set; }
}
```

### `UserExternalLogin`, `UserRole`

```csharp
public class UserExternalLogin
{
    public Guid Id, UserId;
    public string Provider;       // "google", "apple"
    public string ExternalId;     // sub claim
    public DateTimeOffset LinkedAt;
}

public class UserRole
{
    public Guid Id, UserId;
    public string Role;           // "buyer", "seller", "carrier", "vet", "moderator", "admin"
    public DateTimeOffset GrantedAt;
    public Guid? GrantedByUserId;
    public DateTimeOffset? RevokedAt;
    public Guid? RevokedByUserId;
}
```

### `UserConsent` (v2 YENİ — KVKK)

```csharp
public class UserConsent
{
    public Guid Id, UserId;
    public ConsentType Type;
    public string Version;                 // örn. "2026.01"
    public bool Granted;
    public DateTimeOffset GrantedAt;
    public DateTimeOffset? RevokedAt;
    public string IpAddress, UserAgent;    // audit
}

public enum ConsentType
{
    TermsAndPrivacy = 1,        // ZORUNLU register'da
    MinistryDataShare = 2,      // ZORUNLU register'da (Faz 2 Accounts vaccine-sync)
    MarketingEmail = 3,         // OPSİYONEL, kullanıcı toggle
}
```

### `PhoneVerificationTicket` (v2 YENİ — Identity içinde, User AR'ından bağımsız)

```csharp
public class PhoneVerificationTicket
{
    public Guid Id;
    public Guid? UserId;                   // null = register flow
    public PhoneNumber Phone;
    public PhonePurpose Purpose;
    public byte[] CodeHash;                // SHA-256 of 6-digit OTP
    public DateTimeOffset IssuedAt, ExpiresAt;   // 5 dakika
    public DateTimeOffset? ConsumedAt;
    public int AttemptCount;               // max 5
    public string IpAddress;
}

public enum PhonePurpose
{
    Register = 1, ResetPassword = 2, ChangePhone = 3,
    LoginPhoneOtp = 4    // Faz 2 — passwordless phone login
}
```

---

## 4. 3-Method Login Discriminator (v2)

```csharp
public sealed record LoginCommand(
    string Method,           // "email" | "phone" | "nationalId"
    string Identifier,
    string Password,
    bool? RememberMe,
    LoginContext Context);

// Validator per method:
// - email: RFC 5322 + Catalog domain check
// - phone: E.164 format (libphonenumber-csharp)
// - nationalId: 11-digit + TC algoritmik check
```

User lookup `Users.Email` / `Users.Phone.E164` / `Users.NationalId.Value` üzerinden.

---

## 5. Cross-Modül Erişim

### `ICurrentUserService` (Shared/) — Request-Scoped

```csharp
public interface ICurrentUserService
{
    bool IsAuthenticated { get; }
    Guid? GetUserId();
    string? GetEmail();
    string? GetDisplayName();
    string? GetGivenName();
    string? GetFamilyName();
    string GetLocale();
    string GetCurrencyCode();
    string GetCountryCode();
    Guid? GetDeviceId();
    AccountType GetAccountType();                  // v2
    
    IReadOnlyList<string> GetRoles();
    bool IsInRole(string role);
    bool IsInAnyRole(params string[] roles);
    
    bool IsEmailVerified();
    bool IsPhoneVerified();
    
    Guid? GetImpersonationSessionId();             // Admin module
    Guid? GetActualUserId();                       // act claim — impersonation context
}
```

JWT claim'lerden okuyor — DB hit yok, O(1).

### `IIdentityReadService` (Shared/)

```csharp
public interface IIdentityReadService
{
    Task<UserSummary?> GetUserSummaryAsync(Guid userId, CancellationToken ct);
    Task<IReadOnlyList<UserSummary>> GetUserSummariesAsync(IReadOnlyList<Guid> userIds, CancellationToken ct);
    
    Task<bool> UserExistsAsync(Guid userId, CancellationToken ct);
    Task<bool> UserHasRoleAsync(Guid userId, string role, CancellationToken ct);
    Task<bool> UserHasAnyRoleAsync(Guid userId, IReadOnlyList<string> roles, CancellationToken ct);
    Task<AccountType?> GetAccountTypeAsync(Guid userId, CancellationToken ct);
    
    Task<IReadOnlyList<DeviceInfo>> GetActiveDevicesAsync(Guid userId, CancellationToken ct);
    Task<IReadOnlyList<string>> GetActivePushTokensAsync(Guid userId, CancellationToken ct);
    
    Task<bool> HasActiveConsentAsync(Guid userId, ConsentType type, CancellationToken ct);
    Task<UserPreferences> GetUserPreferencesAsync(Guid userId, CancellationToken ct);
}
```

**Cache TTL:** UserSummary 5dk, ActiveDevices 1dk, Role check 30sn, Consent 5dk.

### `IAdminUserCommands` (Shared/Contracts/Identity/Admin/)

```csharp
public interface IAdminUserCommands
{
    Task<Result> SuspendAsync(Guid userId, string reason, Guid actorId, CancellationToken ct);
    Task<Result> ReactivateAsync(Guid userId, Guid actorId, CancellationToken ct);
    Task<Result> GrantRoleAsync(Guid userId, string role, Guid actorId, CancellationToken ct);
    Task<Result> RevokeRoleAsync(Guid userId, string role, Guid actorId, CancellationToken ct);
    Task<Result> ForceLogoutAsync(Guid userId, Guid actorId, CancellationToken ct);
    Task<Result> ImpersonateAsync(Guid targetUserId, Guid adminUserId, string justification, CancellationToken ct);
}
```

---

## 6. Public Event'ler

| Event | Payload | Consumer |
|---|---|---|
| `UserRegistered` | UserId, Email, Locale, Currency, CountryCode | Notifications (welcome mail) |
| `UserEmailVerified` | UserId, Email, VerifiedAt | Notifications |
| `UserPasswordChanged` | UserId, ChangedAt, IpAddress | Notifications (security email) |
| `UserSuspended` | UserId, Reason, SuspendedUntil?, ActorUserId | Accounts (Seller/Carrier auto-suspend), Notifications |
| `UserReactivated` | UserId, ActorUserId | Accounts, Notifications |
| `UserDeleted` | UserId, DeletedAt | **9 consumer** GDPR cascade (Accounts, Carrier, Listings, Marketplace, Messaging, Subscription, Notifications, Admin, internal) |

**Internal:** UserLoggedIn, UserPreferencesChanged, RefreshTokenRotated/Revoked, UserPendingDeletion, UserConsentRecorded, UserRoleGranted/Revoked.

**Identity Consume (Notifications producer 3 event):**
- `EmailBounced` → email_verified=false (hard bounce)
- `SmsDeliveryFailed` → phone flag
- `PushTokenInvalidated` → UserDevice.PushToken=null

---

## 7. JWT Claim Yapısı

```json
{
  "iss": "https://livestock-trading.com",
  "aud": "livestock-api",
  "sub": "01J5C-USER-UUID",
  "jti": "01J5D-TOKEN-UUID",
  "iat": 1746788400,
  "exp": 1746789300,
  
  "email": "user@example.com",
  "email_verified": true,
  "phone_verified": true,
  "name": "Mustafa Ocak",
  "given_name": "Mustafa",
  "family_name": "Ocak",
  
  "account_type": "producer",
  "roles": ["buyer", "seller"],
  
  "country_code": "TR",
  "locale": "tr",
  "currency_code": "TRY",
  
  "device_id": "01J5E-DEVICE-UUID"
}
```

**RS256 + key rotation** (90 gün, OpenIddict overlap 2-cert). **jti blacklist** Redis (suspension/logout/password change).

**Impersonation context:** `sub=targetUserId`, `act=adminUserId` (RFC 8693), `imp=true`, `exp=4h`.

---

## 8. RBAC

| Role | Kim Verir |
|---|---|
| `buyer` | System (her register'da auto) |
| `seller` | System (Accounts.SellerVerified event) |
| `carrier` | System (Carrier.CarrierVerified event) |
| `vet` | System (Accounts.VetVerified event — manual admin approval) |
| `moderator` | Admin manuel |
| `admin` | Admin manuel / AdminBootstrap CLI |

**Multi-role:** Tek user birden fazla role taşıyabilir (`roles[]` array).

**AccountType vs Roles:** AccountType register intent (immutable); Roles dynamic permissions.

---

## 9. OpenIddict

**Flow:** Authorization Code + PKCE (mandatory).

**Clients:**
- `livestock-web` (Public SPA)
- `livestock-mobile` (Public, deep link)
- `livestock-admin` (Public)

**Scopes:** `openid`, `profile`, `email`, `livestock_api`.

**Endpoints:** `/connect/authorize`, `/connect/token`, `/connect/userinfo`, `/connect/logout`, `/.well-known/jwks.json`.

**REST Shim:** `/identity/auth/*` (login/register/refresh) — frontend ergonomi için redirect dance bypass.

---

## 10. OAuth Provider (Google + Apple)

**Apple Sign In zorunluluğu:** App Store Guidelines 4.8 — iOS Google login destekliyorsa Apple Sign In **mandatory**.

**Email collision:** Auto-link (email match → existing user'a provider eklenir).

**Apple email relay:** Privacy relay email kabul; Faz 2 deactivation webhook.

**SİLİNDİ (v2):** e-Devlet SSO, TARSİM/Bakanlık SSO — Faz 1 + Faz 2 dahil tamamen iptal.

---

## 11. GDPR Data Export

```csharp
// POST /identity/users/me/data-export { format }
// Response 202: { jobId, estimatedReadyAt }

// Async Quartz job:
public sealed class DataExportWorker : IConsumer<DataExportRequested>
{
    private readonly IEnumerable<IDataExportContributor> _contributors;
    
    public async Task Consume(...)
    {
        var bundle = new DataExportBundle(evt.UserId);
        bundle.AddSection("identity", await CollectIdentityData(evt.UserId, ct));
        
        foreach (var contributor in _contributors)   // Per modül cross-modül
        {
            var section = await contributor.ExportAsync(evt.UserId, ct);
            bundle.AddSection(contributor.ModuleName, section);
        }
        
        var bytes = JsonSerializer.SerializeToUtf8Bytes(bundle);
        var encrypted = await _gpg.EncryptAsync(bytes);
        var signedUrl = await _storage.GenerateSignedDownloadUrlAsync(
            "data-exports", key, TimeSpan.FromDays(3), ct);
        await _email.SendDataExportReadyAsync(evt.UserId, signedUrl, ct);
    }
}

// Shared/Contracts/DataExport/
public interface IDataExportContributor
{
    string ModuleName { get; }
    Task<object> ExportAsync(Guid userId, CancellationToken ct);
}
```

Per modül implement (Catalog hariç — user-specific data yok).

---

## 12. API Endpoint Inventory

### Public (15 endpoint)

| Method | Path |
|---|---|
| POST | `/identity/auth/login` (3-method discriminator) |
| POST | `/identity/auth/refresh` |
| POST | `/identity/auth/logout` |
| POST | `/identity/auth/register` (accountType + consents) |
| POST | `/identity/auth/phone/send-code` (v2) |
| POST | `/identity/auth/phone/verify` (v2) |
| POST | `/identity/auth/password/forgot` |
| POST | `/identity/auth/password/reset` |
| POST | `/identity/auth/email/send-verify` |
| POST | `/identity/auth/email/verify` |
| POST | `/identity/auth/oauth/google` (v2 — rename) |
| POST | `/identity/auth/oauth/apple` (v2 — rename) |
| GET | `/connect/authorize` |
| POST | `/connect/token` |
| GET | `/connect/userinfo` + `/.well-known/...` |

### Authenticated (16 endpoint)

| Method | Path |
|---|---|
| GET | `/identity/users/me` |
| PATCH | `/identity/users/me` (firstName/lastName split + email change requestPattern) |
| POST | `/identity/users/me/password` |
| POST | `/identity/users/me/avatar` (multipart → IFileStorage) |
| GET | `/identity/users/me/sessions` (enriched: UA parse + GeoIP) |
| DELETE | `/identity/users/me/sessions/{id}` |
| POST | `/identity/users/me/data-export` (v2 GDPR) |
| POST | `/identity/users/me/account/delete-request` |
| POST | `/identity/users/me/account/restore` |
| POST | `/identity/users/me/external-logins` |
| DELETE | `/identity/users/me/external-logins/{id}` |
| POST | `/identity/users/me/devices` |
| PATCH | `/identity/users/me/devices/{id}/push-token` |
| DELETE | `/identity/users/me/devices/{id}` |
| PATCH | `/identity/users/me/preferences` |
| PATCH | `/identity/users/me/consents` (v2 — MarketingEmail toggle + revoke flows) |

### Admin (9 endpoint)

| Method | Path |
|---|---|
| GET | `/admin/users?cursor=...&status=&role=&account_type=&search=` |
| GET | `/admin/users/{id}` |
| POST | `/admin/users/{id}/suspend` |
| POST | `/admin/users/{id}/reactivate` |
| POST | `/admin/users/{id}/grant-role` |
| POST | `/admin/users/{id}/revoke-role` |
| POST | `/admin/users/{id}/force-logout` |
| GET | `/admin/users/{id}/audit` |
| GET | `/admin/users/{id}/sessions` |

**Toplam: 15 + 16 + 9 = 40 endpoint.**

---

## 13. Faz 2 Placeholder

| Field | Faz 1 | Faz 2 |
|---|---|---|
| `User.TotpSecretEncrypted`, `TwoFactorEnabled` | Sütun var, feature pasif | 2FA setup/verify endpoints |
| Passkey/WebAuthn | yok | OpenIddict WebAuthn extension |
| Apple email relay deactivation webhook | skeleton | Real handler |
| NationalId NVI sync | self-declared, verified_at null | NVI API entegrasyonu |
| `LoginPhoneOtp` purpose | enum'da var, endpoint yok | Passwordless phone login |
| Consent versioning re-prompt | single version | Versionlu doc upgrade → UI re-consent |

---

## 14. Discovered Backlog

| # | Konu | Hedef |
|---|---|---|
| 6 | DeviceToken ownership = Identity (kapandı) | ✓ |
| 49 | Admin role policy (kapandı) | ✓ |
| 53 | 2FA Faz 2 | Karar 5/Identity Faz 2 |
| 54 | Passkey/WebAuthn Faz 2 | Faz 2 |
| 56 | JWT cert management — vault store, rotation cron, monitoring | Karar 7 / Operations |
| 57 | Apple email relay deactivation webhook | Faz 2 |
| 58 | Admin audit log inventory | Karar 7 / Operations |
| 59 | MassTransit transactional outbox setup | Karar 7 / Infrastructure |
| 60 | Email/SMS provider integration (MailKit Brevo + Twilio + retry + delivery feedback) | Karar 5 / Notifications |
| 61 | Geo-IP service (MaxMind GeoLite2) | Karar 7 / Infrastructure |
| 73 | Phone OTP rate limit catalog (Twilio cost) | Karar 7 / Anti-abuse |
| 74 | GDPR Data Export retention (3 gün signed URL, job history) | Karar 7 / GDPR |
| 75 | NationalId TR-spesifik validator (Faz 2 multi-country) | Faz 2 |
| 76 | Consent versioning workflow UX | Karar 6 / API contract |
| 77 | GeoLite2 license + cache strategy | Karar 7 / Infrastructure |
| 78 | DisplayName Seller enrichment kuralı | Karar 5 / Accounts |
| 79 | Apple Sign In key management (`.p8` storage, rotation) | Karar 7 / Operations |
| 80 | Pending email cleanup cron (24h+ → reset) | Karar 5 / Identity ops |
| 81 | "Email değiştirildi" security mail eski adrese | Karar 5 / Notifications |
| 141 | UserPreferences.ReadReceiptsEnabled + TypingIndicatorEnabled schema (Messaging patch) | ✓ Bu doc'ta yansıdı |

---

## 15. Özet Tablo

| Konu | Karar |
|---|---|
| AR sayısı | 1 (User; içinde RefreshToken, UserDevice, UserExternalLogin, UserRole, UserConsent, PhoneVerificationTicket) |
| Public events | 6 lifecycle + 3 consume (delivery feedback) = 9 |
| AccountType | 4 enum (Producer/Trader/Vet/Buyer) — IMMUTABLE register'da |
| RBAC | 6 role (buyer default, seller/carrier/vet system-grant, moderator/admin manual); multi-role |
| Login | 3-method discriminator (email/phone/nationalId) |
| OAuth | Google + Apple `/oauth/*` (v2 rename); e-Devlet/TARSİM SİLİNDİ |
| KVKK | UserConsent entity (terms_privacy/ministry/marketing); register'da zorunlu çift onay |
| GDPR | Data export (Quartz async + IDataExportContributor); 30-day grace delete + anonymize |
| JWT | RS256 + key rotation (90 gün); jti blacklist Redis |
| Cross-modül | `ICurrentUserService` (JWT) + `IIdentityReadService` (cached) |
| Endpoint | 40 (15 public + 16 auth + 9 admin) |
