using Livestock.Domain.Enums;

namespace Livestock.Domain.Entities;

public class Seller : BaseEntity
{
    public Guid UserId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? TaxNumber { get; set; }
    public string? LogoUrl { get; set; }
    public SellerStatus Status { get; set; } = SellerStatus.PendingVerification;
    public string? SuspensionReason { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public Guid? BucketId { get; set; }
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }

    public ICollection<Farm> Farms { get; set; } = [];
    public ICollection<Product> Products { get; set; } = [];
    public ICollection<SellerSubscription> Subscriptions { get; set; } = [];
    public ICollection<SellerReview> Reviews { get; set; } = [];
}
