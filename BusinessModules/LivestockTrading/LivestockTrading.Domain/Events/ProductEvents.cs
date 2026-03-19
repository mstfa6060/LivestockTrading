namespace LivestockTrading.Domain.Events;

/// <summary>
/// Event fired when a product is approved by a moderator
/// Used to trigger social media posting and seller notification
/// </summary>
public class ProductApprovedEvent : IDomainEvent
{
    public Guid ProductId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string ShortDescription { get; set; }
    public double BasePrice { get; set; }
    public string Currency { get; set; }
    public string CategoryName { get; set; }
    public string BrandName { get; set; }
    public string SellerBusinessName { get; set; }
    public Guid SellerId { get; set; }
    public string Slug { get; set; }

    /// <summary>Ülke kodu (ISO 3166-1 alpha-2)</summary>
    public string CountryCode { get; set; }

    /// <summary>Şehir</summary>
    public string City { get; set; }

    /// <summary>Cover image URL (MinIO public URL)</summary>
    public string CoverImageUrl { get; set; }

    /// <summary>MediaBucketId for fetching all images</summary>
    public string MediaBucketId { get; set; }

    public DateTime ApprovedAt { get; set; } = DateTime.UtcNow;
    public DateTime OccurredAt => ApprovedAt;
}

/// <summary>
/// Event fired when a new product is created and sent to moderation (PendingApproval).
/// Used to notify admin/moderator users about new products awaiting approval.
/// </summary>
public class ProductCreatedEvent : IDomainEvent
{
    public Guid ProductId { get; set; }
    public string Title { get; set; }
    public string ShortDescription { get; set; }
    public double BasePrice { get; set; }
    public string Currency { get; set; }
    public string SellerBusinessName { get; set; }
    public Guid SellerId { get; set; }
    public string Slug { get; set; }

    /// <summary>Admin/Moderator user IDs to notify (resolved at publish time)</summary>
    public List<Guid> TargetAdminUserIds { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime OccurredAt => CreatedAt;
}
