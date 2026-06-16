using Livestock.Domain.Enums;

namespace Livestock.Features.VeterinaryInfos;

public record VeterinaryInfoDetail(
    Guid Id, Guid ProductId,
    VeterinaryProductType ProductType, AdministrationRoute AdministrationRoute,
    string? ActiveSubstance, string? Concentration,
    string? TargetSpecies, string? Indications, string? Contraindications,
    string? Dosage, int? WithholdingPeriodDays,
    bool RequiresPrescription, string? StorageConditions,
    string? RegistrationNumber, DateTime? ExpiryDate, string? Manufacturer, DateTime CreatedAt);

public record CreateVeterinaryInfoRequest(
    Guid ProductId,
    VeterinaryProductType ProductType, AdministrationRoute AdministrationRoute,
    string? ActiveSubstance, string? Concentration,
    string? TargetSpecies, string? Indications, string? Contraindications,
    string? Dosage, int? WithholdingPeriodDays,
    bool RequiresPrescription, string? StorageConditions,
    string? RegistrationNumber, DateTime? ExpiryDate, string? Manufacturer);

public record UpdateVeterinaryInfoRequest(
    Guid Id,
    VeterinaryProductType ProductType, AdministrationRoute AdministrationRoute,
    string? ActiveSubstance, string? Concentration,
    string? TargetSpecies, string? Indications, string? Contraindications,
    string? Dosage, int? WithholdingPeriodDays,
    bool RequiresPrescription, string? StorageConditions,
    string? RegistrationNumber, DateTime? ExpiryDate, string? Manufacturer);

public record GetVeterinaryInfoByProductRequest(Guid ProductId);
public record GetVeterinaryInfoRequest(Guid Id);
public record DeleteVeterinaryInfoRequest(Guid Id);
