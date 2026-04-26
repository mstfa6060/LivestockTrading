using Livestock.Domain.Enums;

namespace Livestock.Domain.Entities;

public class SeedInfo : BaseEntity
{
    public Guid ProductId { get; set; }
    public SeedType SeedType { get; set; }
    public string? Variety { get; set; }
    public string? ScientificName { get; set; }
    public decimal? GerminationRatePercent { get; set; }
    public int? DaysToMaturity { get; set; }
    public decimal? PackageSizeGrams { get; set; }
    public DateTime? HarvestDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsHybrid { get; set; }
    public bool IsOrganic { get; set; }
    public bool IsGmoFree { get; set; }
    public string? SuitableClimate { get; set; }
    public string? PlantingInstructions { get; set; }
    public string? CertificationInfo { get; set; }

    public Product Product { get; set; } = null!;
}
