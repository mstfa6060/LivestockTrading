using Livestock.Domain.Enums;

namespace Livestock.Domain.Entities;

public class Product : BaseEntity
{
    public Guid SellerId { get; set; }
    public Guid CategoryId { get; set; }
    public Guid? BrandId { get; set; }
    public Guid? FarmId { get; set; }
    public Guid? LocationId { get; set; }
    public Guid? BucketId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public int Quantity { get; set; }
    public string? Unit { get; set; }
    public ProductStatus Status { get; set; } = ProductStatus.Draft;
    public ProductCondition Condition { get; set; } = ProductCondition.New;
    public string? RejectionReason { get; set; }
    public bool IsNegotiable { get; set; }
    public bool IsFeatured { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public int ViewCount { get; set; }
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }

    public Seller Seller { get; set; } = null!;
    public Category Category { get; set; } = null!;
    public Brand? Brand { get; set; }
    public Farm? Farm { get; set; }
    public Location? Location { get; set; }

    public ICollection<ProductVariant> Variants { get; set; } = [];
    public ICollection<FavoriteProduct> Favorites { get; set; } = [];
    public ICollection<ProductReview> Reviews { get; set; } = [];
    public ICollection<ProductReport> Reports { get; set; } = [];
    public ICollection<ProductBoost> Boosts { get; set; } = [];
    public ICollection<Offer> Offers { get; set; } = [];

    public AnimalInfo? AnimalInfo { get; set; }
    public FeedInfo? FeedInfo { get; set; }
    public ChemicalInfo? ChemicalInfo { get; set; }
    public MachineryInfo? MachineryInfo { get; set; }
    public SeedInfo? SeedInfo { get; set; }
    public VeterinaryInfo? VeterinaryInfo { get; set; }
}
