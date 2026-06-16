using Livestock.Domain.Enums;

namespace Livestock.Domain.Entities;

public class Farm : BaseEntity
{
    public Guid SellerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public FarmType FarmType { get; set; }
    public double? AreaHectares { get; set; }
    public int? CapacityHead { get; set; }
    public string? CertificationInfo { get; set; }
    public bool IsOrganic { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? PhoneNumber { get; set; }
    public Guid? BucketId { get; set; }

    public Seller Seller { get; set; } = null!;
    public ICollection<Product> Products { get; set; } = [];
}
