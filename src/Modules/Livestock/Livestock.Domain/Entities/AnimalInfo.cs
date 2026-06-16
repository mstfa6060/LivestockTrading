using Livestock.Domain.Enums;

namespace Livestock.Domain.Entities;

public class AnimalInfo : BaseEntity
{
    public Guid ProductId { get; set; }
    public string Species { get; set; } = string.Empty;
    public string? Breed { get; set; }
    public AnimalGender Gender { get; set; }
    public int? AgeMonths { get; set; }
    public decimal? WeightKg { get; set; }
    public HealthStatus HealthStatus { get; set; } = HealthStatus.Healthy;
    public AnimalPurpose Purpose { get; set; }
    public bool IsVaccinated { get; set; }
    public bool HasHealthCertificate { get; set; }
    public string? EarTagNumber { get; set; }
    public string? PassportNumber { get; set; }
    public string? Color { get; set; }
    public bool IsPregnant { get; set; }
    public string? BreedingHistory { get; set; }

    public Product Product { get; set; } = null!;
    public ICollection<HealthRecord> HealthRecords { get; set; } = [];
    public ICollection<Vaccination> Vaccinations { get; set; } = [];
}
