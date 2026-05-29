namespace LivestockTrading.Identity.Domain.Enums;

/// <summary>Refresh token revocation reason — RefreshToken child entity Status property + audit log.</summary>
public enum RevocationReason
{
    Logout = 1,
    Rotation = 2,
    Reuse = 3,
    UserSuspended = 4,
    UserDeleted = 5,
    PasswordChanged = 6,
    AdminRevoked = 7,
}
