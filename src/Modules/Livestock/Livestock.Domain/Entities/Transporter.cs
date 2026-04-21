using Livestock.Domain.Enums;

namespace Livestock.Domain.Entities;

public class Transporter : BaseEntity
{
    public Guid UserId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? LicenseNumber { get; set; }
    public string? LogoUrl { get; set; }
    public TransporterStatus Status { get; set; } = TransporterStatus.PendingVerification;
    public string? SuspensionReason { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public Guid? BucketId { get; set; }
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public string? ServiceCountryCodes { get; set; }

    public ICollection<TransportRequest> TransportRequests { get; set; } = [];
    public ICollection<TransportOffer> TransportOffers { get; set; } = [];
    public ICollection<TransporterReview> Reviews { get; set; } = [];
    public ICollection<SellerSubscription> Subscriptions { get; set; } = [];
}
