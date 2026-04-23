namespace Livestock.Domain.Entities;

public class HealthRecord : BaseEntity
{
    public Guid AnimalInfoId { get; set; }
    public string Diagnosis { get; set; } = string.Empty;
    public string? Treatment { get; set; }
    public string? VetName { get; set; }
    public string? VetClinic { get; set; }
    public DateTime TreatmentDate { get; set; }
    public string? Notes { get; set; }

    public AnimalInfo AnimalInfo { get; set; } = null!;
}
