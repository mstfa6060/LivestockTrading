using Livestock.Domain.Enums;

namespace Livestock.Domain.Entities;

public class FeedInfo : BaseEntity
{
    public Guid ProductId { get; set; }
    public FeedType FeedType { get; set; }
    public FeedForm FeedForm { get; set; }
    public string? TargetSpecies { get; set; }
    public decimal? ProteinPercentage { get; set; }
    public decimal? FatPercentage { get; set; }
    public decimal? FiberPercentage { get; set; }
    public decimal? MoisturePercentage { get; set; }
    public string? Ingredients { get; set; }
    public string? Additives { get; set; }
    public DateTime? ManufactureDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? CertificationInfo { get; set; }
    public bool IsOrganic { get; set; }
    public decimal? PackageSizeKg { get; set; }

    public Product Product { get; set; } = null!;
}
