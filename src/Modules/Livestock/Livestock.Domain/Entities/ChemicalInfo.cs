using Livestock.Domain.Enums;

namespace Livestock.Domain.Entities;

public class ChemicalInfo : BaseEntity
{
    public Guid ProductId { get; set; }
    public ChemicalType ChemicalType { get; set; }
    public ToxicityLevel ToxicityLevel { get; set; }
    public string? ActiveIngredient { get; set; }
    public string? Concentration { get; set; }
    public string? TargetPest { get; set; }
    public string? TargetCrop { get; set; }
    public string? ApplicationMethod { get; set; }
    public string? SafetyPrecautions { get; set; }
    public string? StorageConditions { get; set; }
    public int? WithholdingPeriodDays { get; set; }
    public string? RegistrationNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public decimal? VolumeOrWeightPerUnit { get; set; }
    public string? Unit { get; set; }

    public Product Product { get; set; } = null!;
}
