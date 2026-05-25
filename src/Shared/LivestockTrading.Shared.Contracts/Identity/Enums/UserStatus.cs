namespace Shared.Contracts.Identity;

/// <summary>Kullanıcı hesap durumu (05-identity §2 satır 170-174).</summary>
public enum UserStatus
{
    EmailUnverified = 1,
    Active = 2,
    Suspended = 3,
    PendingDeletion = 4,
    Deleted = 5,
}
