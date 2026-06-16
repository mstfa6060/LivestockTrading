namespace Livestock.Domain.Entities;

public class Vaccination : BaseEntity
{
    public Guid AnimalInfoId { get; set; }
    public string VaccineName { get; set; } = string.Empty;
    public string? Manufacturer { get; set; }
    public string? BatchNumber { get; set; }
    public DateTime AdministeredAt { get; set; }
    public DateTime? NextDueAt { get; set; }
    public string? VetName { get; set; }
    public string? Notes { get; set; }

    public AnimalInfo AnimalInfo { get; set; } = null!;
}
