namespace Livestock.Features.Vaccinations;

public record VaccinationDetail(
    Guid Id, Guid AnimalInfoId,
    string VaccineName, string? Manufacturer, string? BatchNumber,
    DateTime AdministeredAt, DateTime? NextDueAt,
    string? VetName, string? Notes, DateTime CreatedAt);

public record CreateVaccinationRequest(
    Guid AnimalInfoId,
    string VaccineName, string? Manufacturer, string? BatchNumber,
    DateTime AdministeredAt, DateTime? NextDueAt,
    string? VetName, string? Notes);

public record UpdateVaccinationRequest(
    Guid Id,
    string VaccineName, string? Manufacturer, string? BatchNumber,
    DateTime AdministeredAt, DateTime? NextDueAt,
    string? VetName, string? Notes);

public record GetVaccinationsRequest(Guid AnimalInfoId);
public record GetVaccinationRequest(Guid Id);
public record DeleteVaccinationRequest(Guid Id);
