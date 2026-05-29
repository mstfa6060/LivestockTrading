namespace LivestockTrading.Identity.Domain.Aggregates;

using System.Security.Cryptography;
using LivestockTrading.Identity.Domain.Entities;
using LivestockTrading.Identity.Domain.Enums;
using LivestockTrading.Identity.Domain.Events.Internal;
using LivestockTrading.Identity.Domain.Events.Public;
using LivestockTrading.Identity.Domain.ValueObjects;
using Shared.Contracts.Identity;
using Shared.Domain;

/// <summary>
/// User Aggregate Root - Identity modulu ana AR. 25 property + 5 child collection + 27 behavior metot (H.2-H.4'te eklenecek).
/// Factory: RegisterWithPassword + RegisterWithSocial (H.2). Behavior: email/password/status (H.3) + role/consent/device/token (H.4).
/// </summary>
public sealed class User : AggregateRoot
{
    // === Identity ===
    public Guid Id { get; private set; }

    // === Profile ===
    public EmailAddress Email { get; private set; }
    public EmailAddress? PendingEmail { get; private set; }
    public DateTimeOffset? PendingEmailRequestedAt { get; private set; }
    public DateTimeOffset? PendingEmailExpiresAt { get; private set; }
    public HashedPassword? Password { get; private set; }
    public PersonName Name { get; private set; }
    public PhoneNumber? Phone { get; private set; }
    public NationalId? NationalId { get; private set; }
    public AccountType AccountType { get; private set; }
    public UserPreferences Preferences { get; private set; }
    public string? AvatarUrl { get; private set; }

    // === Status ===
    public UserStatus Status { get; private set; }
    public DateTimeOffset? EmailVerifiedAt { get; private set; }
    public DateTimeOffset? PhoneVerifiedAt { get; private set; }
    public DateTimeOffset? SuspendedAt { get; private set; }
    public string? SuspendedReason { get; private set; }
    public Guid? SuspendedByUserId { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public DateTimeOffset? AnonymizedAt { get; private set; }
    public DateTimeOffset? PendingDeletionRequestedAt { get; private set; }

    // === Audit ===
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public DateTimeOffset? LastLoginAt { get; private set; }

    // === Faz 2 (2FA) ===
    public string? TotpSecretEncrypted { get; private set; }
    public bool TwoFactorEnabled { get; private set; }

    // === Child Collections ===
    private readonly List<RefreshToken> _refreshTokens = new();
    private readonly List<UserDevice> _devices = new();
    private readonly List<UserExternalLogin> _externalLogins = new();
    private readonly List<UserRole> _roles = new();
    private readonly List<UserConsent> _consents = new();

    public IReadOnlyList<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();
    public IReadOnlyList<UserDevice> Devices => _devices.AsReadOnly();
    public IReadOnlyList<UserExternalLogin> ExternalLogins => _externalLogins.AsReadOnly();
    public IReadOnlyList<UserRole> Roles => _roles.AsReadOnly();
    public IReadOnlyList<UserConsent> Consents => _consents.AsReadOnly();

    // === EF Core ctor ===
    private User()
    {
        Email = default!;
        Name = default!;
        Preferences = default!;
    }

    // === Factories ===

    public static User RegisterWithPassword(
        EmailAddress email,
        HashedPassword password,
        PersonName name,
        PhoneNumber? phone,
        NationalId? nationalId,
        AccountType accountType,
        UserPreferences preferences,
        IReadOnlyList<ConsentGrant> consents,
        string ipAddress,
        string userAgent,
        DateTimeOffset now)
    {
        var user = new User
        {
            Id = Guid.CreateVersion7(),
            Email = email,
            Password = password,
            Name = name,
            Phone = phone,
            NationalId = nationalId,
            AccountType = accountType,
            Preferences = preferences,
            Status = UserStatus.EmailUnverified,
            CreatedAt = now,
            UpdatedAt = now,
        };

        user._roles.Add(new UserRole(user.Id, "buyer", now, grantedByUserId: null));

        foreach (var c in consents)
        {
            user._consents.Add(new UserConsent(
                user.Id, c.Type, c.Version, c.Granted, now, ipAddress, userAgent));
        }

        user.Raise(new UserRegistered(
            user.Id,
            email.Value,
            preferences.Locale,
            preferences.CurrencyCode,
            preferences.CountryCode));

        return user;
    }

    public static User RegisterWithSocial(
        EmailAddress email,
        PersonName name,
        string provider,
        string externalId,
        AccountType accountType,
        UserPreferences preferences,
        IReadOnlyList<ConsentGrant> consents,
        string ipAddress,
        string userAgent,
        DateTimeOffset now)
    {
        var user = new User
        {
            Id = Guid.CreateVersion7(),
            Email = email,
            Password = null,
            Name = name,
            AccountType = accountType,
            Preferences = preferences,
            Status = UserStatus.Active,
            EmailVerifiedAt = now,
            CreatedAt = now,
            UpdatedAt = now,
        };

        user._externalLogins.Add(new UserExternalLogin(user.Id, provider, externalId, now));
        user._roles.Add(new UserRole(user.Id, "buyer", now, grantedByUserId: null));

        foreach (var c in consents)
        {
            user._consents.Add(new UserConsent(
                user.Id, c.Type, c.Version, c.Granted, now, ipAddress, userAgent));
        }

        user.Raise(new UserRegistered(
            user.Id,
            email.Value,
            preferences.Locale,
            preferences.CurrencyCode,
            preferences.CountryCode));

        return user;
    }

    // === Behavior: Profile/Email/Password ===

    public void AssignNationalId(NationalId nationalId, DateTimeOffset now)
    {
        if (NationalId is not null)
            throw new DomainException("NationalId zaten atanmis, degistirilemez.");
        NationalId = nationalId;
        UpdatedAt = now;
    }

    public string RequestEmailChange(EmailAddress newEmail, TimeSpan ttl, DateTimeOffset now)
    {
        EnsureNotSuspendedOrDeleted();
        if (ttl <= TimeSpan.Zero)
            throw new DomainException("Ttl pozitif olmali.");
        PendingEmail = newEmail;
        PendingEmailRequestedAt = now;
        PendingEmailExpiresAt = now + ttl;
        UpdatedAt = now;
        return GenerateOpaqueToken();
    }

    public void ConfirmEmailChange(EmailAddress confirmedNewEmail, DateTimeOffset now)
    {
        if (PendingEmail is null)
            throw new DomainException("Pending email yok.");
        if (PendingEmailExpiresAt is null || now >= PendingEmailExpiresAt)
            throw new DomainException("Email change suresi dolmus.");
        if (!PendingEmail.Equals(confirmedNewEmail))
            throw new DomainException("Confirm email pending ile eslesmiyor.");
        Email = confirmedNewEmail;
        EmailVerifiedAt = now;
        PendingEmail = null;
        PendingEmailRequestedAt = null;
        PendingEmailExpiresAt = null;
        UpdatedAt = now;
        Raise(new UserEmailVerified(Id, Email.Value, now));
    }

    public void CancelPendingEmailChange(DateTimeOffset now)
    {
        PendingEmail = null;
        PendingEmailRequestedAt = null;
        PendingEmailExpiresAt = null;
        UpdatedAt = now;
    }

    public void VerifyEmail(DateTimeOffset now)
    {
        EmailVerifiedAt = now;
        if (Status == UserStatus.EmailUnverified)
            Status = UserStatus.Active;
        UpdatedAt = now;
        Raise(new UserEmailVerified(Id, Email.Value, now));
    }

    public void VerifyPhone(DateTimeOffset now)
    {
        PhoneVerifiedAt = now;
        UpdatedAt = now;
        // F-W4-22: plan-doc "internal event" der ama §6 listesinde UserPhoneVerified yok - 8 internal kilitli, event YOK.
    }

    public void ChangePassword(HashedPassword newPassword, string ipAddress, DateTimeOffset now)
    {
        EnsureNotSuspendedOrDeleted();
        Password = newPassword;
        UpdatedAt = now;
        Raise(new UserPasswordChanged(Id, now, ipAddress));
    }

    // === Behavior: Status ===

    public void Suspend(string reason, Guid actorUserId, DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Suspend reason bos olamaz.");
        Status = UserStatus.Suspended;
        SuspendedAt = now;
        SuspendedReason = reason;
        SuspendedByUserId = actorUserId;
        UpdatedAt = now;

        // Inline cascade: tum aktif refresh token revoke (RevokeAllRefreshTokens H.4'te ayri, burada inline)
        foreach (var token in _refreshTokens)
        {
            if (token.IsActive(now))
            {
                token.Revoke(RevocationReason.UserSuspended, now);
                Raise(new RefreshTokenRevoked(token.UserId, token.Id, token.FamilyId, RevocationReason.UserSuspended));
            }
        }

        Raise(new UserSuspended(Id, reason, SuspendedUntil: null, actorUserId));
    }

    public void Reactivate(Guid actorUserId, DateTimeOffset now)
    {
        if (Status != UserStatus.Suspended)
            throw new DomainException("Sadece suspended kullanici reactivate edilebilir.");
        Status = UserStatus.Active;
        SuspendedAt = null;
        SuspendedReason = null;
        SuspendedByUserId = null;
        UpdatedAt = now;
        Raise(new UserReactivated(Id, actorUserId));
    }

    public void RequestDeletion(DateTimeOffset now)
    {
        Status = UserStatus.PendingDeletion;
        PendingDeletionRequestedAt = now;
        UpdatedAt = now;
        var scheduledDeletionAt = now.AddDays(30);
        Raise(new UserPendingDeletion(Id, now, scheduledDeletionAt));
    }

    public void CancelDeletion(DateTimeOffset now)
    {
        if (Status != UserStatus.PendingDeletion)
            throw new DomainException("Sadece pending-deletion kullanici iptal edebilir.");
        Status = UserStatus.Active;
        PendingDeletionRequestedAt = null;
        UpdatedAt = now;
    }

    public void Anonymize(DateTimeOffset now)
    {
        AnonymizedAt = now;
        UpdatedAt = now;
        // GDPR PII temizleme Faz 2 - bu turda sadece AnonymizedAt isareti, event YOK.
    }

    public void FinalizeDeletion(DateTimeOffset now)
    {
        if (Status != UserStatus.PendingDeletion)
            throw new DomainException("Sadece pending-deletion kullanici finalize edilebilir.");
        Status = UserStatus.Deleted;
        DeletedAt = now;
        UpdatedAt = now;
        Raise(new UserDeleted(Id, now));
    }

    public void RecordLogin(Guid deviceId, string ipAddress, DateTimeOffset now)
    {
        if (Status != UserStatus.Active && Status != UserStatus.EmailUnverified)
            throw new DomainException("Sadece active veya email-unverified kullanici login yapabilir.");
        LastLoginAt = now;
        UpdatedAt = now;
        Raise(new UserLoggedIn(Id, deviceId, ipAddress));
    }

    public void UpdatePreferences(UserPreferences newPrefs, DateTimeOffset now)
    {
        EnsureNotSuspendedOrDeleted();
        Preferences = newPrefs;
        UpdatedAt = now;
        Raise(new UserPreferencesChanged(Id));
    }

    // === Behavior: Role ===

    public void GrantRole(string role, Guid? grantedByUserId, DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(role))
            throw new DomainException("Role bos olamaz.");

        if (_roles.Any(r => r.Role == role && r.RevokedAt is null))
            return; // H-10 idempotent

        _roles.Add(new UserRole(Id, role, now, grantedByUserId));
        UpdatedAt = now;
        Raise(new UserRoleGranted(Id, role, grantedByUserId));
    }

    public void RevokeRole(string role, Guid revokedByUserId, DateTimeOffset now)
    {
        var activeRole = _roles.FirstOrDefault(r => r.Role == role && r.RevokedAt is null);
        if (activeRole is null)
            throw new DomainException($"Aktif role bulunamadi: {role}");

        activeRole.Revoke(revokedByUserId, now);
        UpdatedAt = now;
        Raise(new UserRoleRevoked(Id, role, revokedByUserId));
    }

    // === Behavior: Consent ===

    public void RecordConsent(ConsentGrant grant, string ipAddress, string userAgent, DateTimeOffset now)
    {
        _consents.Add(new UserConsent(Id, grant.Type, grant.Version, grant.Granted, now, ipAddress, userAgent));
        UpdatedAt = now;
        Raise(new UserConsentRecorded(Id, grant.Type, grant.Version, grant.Granted, ipAddress));
    }

    public void RevokeConsent(ConsentType type, DateTimeOffset now)
    {
        var activeConsent = _consents.FirstOrDefault(c => c.Type == type && c.RevokedAt is null);
        if (activeConsent is null)
            throw new DomainException($"Aktif consent bulunamadi: {type}");

        activeConsent.Revoke(now);
        UpdatedAt = now;
        // Event YOK - B-W4.1-6 immutable audit: revoke entity mutation, ek event yok
    }

    public bool HasActiveConsent(ConsentType type, string version) =>
        _consents.Any(c => c.Type == type && c.Version == version && c.Granted && c.RevokedAt is null);

    // === Behavior: Device ===

    public UserDevice RegisterDevice(
        string platform,
        string? userAgent,
        string deviceFingerprint,
        string? pushToken,
        DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(platform))
            throw new DomainException("Platform bos olamaz.");
        if (string.IsNullOrWhiteSpace(deviceFingerprint))
            throw new DomainException("DeviceFingerprint bos olamaz.");

        // DUR-A: fiili ctor sirasi (userId, platform, userAgent, deviceFingerprint, pushToken, registeredAt)
        var device = new UserDevice(Id, platform, userAgent, deviceFingerprint, pushToken, now);
        _devices.Add(device);
        UpdatedAt = now;
        return device;
    }

    public void InvalidatePushToken(Guid deviceId, DateTimeOffset now)
    {
        var device = _devices.FirstOrDefault(d => d.Id == deviceId);
        if (device is null)
            throw new DomainException($"Device bulunamadi: {deviceId}");

        device.InvalidatePushToken(now);
        UpdatedAt = now;
    }

    public void RemoveDevice(Guid deviceId)
    {
        var device = _devices.FirstOrDefault(d => d.Id == deviceId);
        if (device is null)
            throw new DomainException($"Device bulunamadi: {deviceId}");

        _devices.Remove(device);
    }

    // === Behavior: Token ===

    public RefreshToken IssueRefreshToken(Guid deviceId, string tokenHash, Guid familyId, TimeSpan ttl, DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(tokenHash))
            throw new DomainException("TokenHash bos olamaz.");
        if (ttl <= TimeSpan.Zero)
            throw new DomainException("Ttl pozitif olmali.");

        // DUR-B: named-arg pattern, fiili sira (userId, deviceId, tokenHash, parentTokenId, familyId, issuedAt, expiresAt)
        // DUR-C: tokenHash string
        var token = new RefreshToken(
            userId: Id,
            deviceId: deviceId,
            tokenHash: tokenHash,
            parentTokenId: null,
            familyId: familyId,
            issuedAt: now,
            expiresAt: now + ttl);
        _refreshTokens.Add(token);
        UpdatedAt = now;
        return token;
    }

    public RefreshToken RotateRefreshToken(Guid oldTokenId, string newTokenHash, TimeSpan ttl, DateTimeOffset now)
    {
        var oldToken = _refreshTokens.FirstOrDefault(t => t.Id == oldTokenId);
        if (oldToken is null)
            throw new DomainException($"RefreshToken bulunamadi: {oldTokenId}");
        if (!oldToken.IsActive(now))
            throw new DomainException("Sadece aktif token rotate edilebilir.");
        if (string.IsNullOrWhiteSpace(newTokenHash))
            throw new DomainException("NewTokenHash bos olamaz.");
        if (ttl <= TimeSpan.Zero)
            throw new DomainException("Ttl pozitif olmali.");

        // Yeni token: ayni FamilyId, parent eski token, named-arg
        var newToken = new RefreshToken(
            userId: Id,
            deviceId: oldToken.DeviceId,
            tokenHash: newTokenHash,
            parentTokenId: oldToken.Id,
            familyId: oldToken.FamilyId,
            issuedAt: now,
            expiresAt: now + ttl);
        _refreshTokens.Add(newToken);

        // Eski revoke: Rotation reason, replacedBy yeni Id'sinin string'i (F-W4-16 plan-doc literal string)
        oldToken.Revoke(RevocationReason.Rotation, now, newToken.Id.ToString());

        UpdatedAt = now;
        Raise(new RefreshTokenRotated(Id, oldToken.Id, newToken.Id, oldToken.FamilyId, oldToken.DeviceId));
        return newToken;
    }

    public void RevokeRefreshToken(Guid tokenId, RevocationReason reason, DateTimeOffset now)
    {
        var token = _refreshTokens.FirstOrDefault(t => t.Id == tokenId);
        if (token is null)
            throw new DomainException($"RefreshToken bulunamadi: {tokenId}");
        if (!token.IsActive(now))
            return; // idempotent

        token.Revoke(reason, now);
        UpdatedAt = now;
        Raise(new RefreshTokenRevoked(Id, token.Id, token.FamilyId, reason));
    }

    public void RevokeAllRefreshTokens(RevocationReason reason, DateTimeOffset now)
    {
        // Suspend inline cascade'in public versiyonu (B-W4.1.F-3 N ayri event)
        foreach (var token in _refreshTokens)
        {
            if (token.IsActive(now))
            {
                token.Revoke(reason, now);
                Raise(new RefreshTokenRevoked(Id, token.Id, token.FamilyId, reason));
            }
        }
        UpdatedAt = now;
    }

    // === Private helpers ===

    private void EnsureNotSuspendedOrDeleted()
    {
        if (Status == UserStatus.Suspended)
            throw new DomainException("Suspended kullanici bu islemi yapamaz.");
        if (Status == UserStatus.Deleted || Status == UserStatus.PendingDeletion)
            throw new DomainException("Deleted/pending-deletion kullanici bu islemi yapamaz.");
    }

    private static string GenerateOpaqueToken() =>
        Convert.ToHexString(RandomNumberGenerator.GetBytes(32));

    // === Entity base ===
    protected override object IdentityValue => Id;
    protected override bool IsTransient => Id == Guid.Empty;

    // Behavior grup 2 (role/consent/device/token) H.4 turunda eklenecek.
}
