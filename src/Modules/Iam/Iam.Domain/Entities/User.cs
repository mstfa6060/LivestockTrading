using Iam.Domain.Enums;

namespace Iam.Domain.Entities;

public class User : BaseEntity
{
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string Surname { get; set; } = default!;
    public string? PasswordHash { get; set; }
    public string? PasswordSalt { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsPhoneVerified { get; set; }
    public bool EmailConfirmed { get; set; }
    public string? LastOtpCode { get; set; }
    public DateTime? LastOtpSentAt { get; set; }
    public DateTime? LastOtpVerifiedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? BirthDate { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public int? CountryId { get; set; }
    public string? Language { get; set; }
    public string? PreferredCurrencyCode { get; set; }
    public Guid? BucketId { get; set; }
    public UserSources UserSource { get; set; } = UserSources.Native;
    public string? AuthProvider { get; set; }
    public string? ProviderKey { get; set; }
    public string? Description { get; set; }
    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpiresAt { get; set; }

    public Country? Country { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = [];
    public ICollection<AppRefreshToken> RefreshTokens { get; set; } = [];
}
