using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Definitions.Base.Entity;



namespace Common.Definitions.Domain.Entities;

public class User : BaseEntity
{
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string Surname { get; set; }

    [MaxLength(256)]
    public string Email { get; set; }

    public bool IsActive { get; set; }

    // Kimlik doğrulama için eklenen alanlar
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
    public bool EmailConfirmed { get; set; }
    public string EmailConfirmationToken { get; set; }
    public string PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpiry { get; set; }

    // OAuth sağlayıcıları desteği
    public string AuthProvider { get; set; }
    public string ProviderKey { get; set; }
    public string AuthAccessToken { get; set; }

    // Kullanıcının tercihleri
    public string Language { get; set; }
    public double VisibilityRadiusKm { get; set; } = 50.0; // Default 50 km radius for visibility

    // En son bilinen lokasyon
    public double? LastKnownLatitude { get; set; }
    public double? LastKnownLongitude { get; set; }
    public DateTime? LastLocationUpdate { get; set; }

    public UserSources UserSource { get; set; }


    public DateTime BirthDate { get; set; }
    public string City { get; set; }
    public string District { get; set; }
    public string Description { get; set; }
    public bool IsAvailable { get; set; }

    [MaxLength(32)]
    public string PhoneNumber { get; set; }
    public string BucketId { get; set; } // BucketId for profile picture

    public string LastOtpCode { get; set; }
    public DateTime? LastOtpSentAt { get; set; }
    public DateTime? LastOtpVerifiedAt { get; set; }
    public bool IsPhoneVerified { get; set; } = false;

    /// <summary>
    /// Kullanıcının ülkesi (FK -> Countries)
    /// </summary>
    public int CountryId { get; set; }

    /// <summary>
    /// Navigation property - Ülke
    /// </summary>
    [ForeignKey("CountryId")]
    public Country Country { get; set; }

}

public enum UserSources
{
    Manual = 0,
    Google = 1,
    Apple = 2,
    Unknown = 99 // <<< Bunu ekle
}

