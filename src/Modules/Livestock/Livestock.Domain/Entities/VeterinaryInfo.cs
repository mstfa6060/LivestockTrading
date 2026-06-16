using Livestock.Domain.Enums;

namespace Livestock.Domain.Entities;

public class VeterinaryInfo : BaseEntity
{
    public Guid ProductId { get; set; }
    public VeterinaryProductType ProductType { get; set; }
    public AdministrationRoute AdministrationRoute { get; set; }
    public string? ActiveSubstance { get; set; }
    public string? Concentration { get; set; }
    public string? TargetSpecies { get; set; }
    public string? Indications { get; set; }
    public string? Contraindications { get; set; }
    public string? Dosage { get; set; }
    public int? WithholdingPeriodDays { get; set; }
    public bool RequiresPrescription { get; set; }
    public string? StorageConditions { get; set; }
    public string? RegistrationNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Manufacturer { get; set; }

    public Product Product { get; set; } = null!;
}
