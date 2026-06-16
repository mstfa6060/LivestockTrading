using Livestock.Domain.Enums;

namespace Livestock.Features.ChemicalInfos;

public record ChemicalInfoDetail(
    Guid Id, Guid ProductId,
    ChemicalType ChemicalType, ToxicityLevel ToxicityLevel,
    string? ActiveIngredient, string? Concentration,
    string? TargetPest, string? TargetCrop,
    string? ApplicationMethod, string? SafetyPrecautions,
    string? StorageConditions, int? WithholdingPeriodDays,
    string? RegistrationNumber, DateTime? ExpiryDate,
    decimal? VolumeOrWeightPerUnit, string? Unit, DateTime CreatedAt);

public record CreateChemicalInfoRequest(
    Guid ProductId,
    ChemicalType ChemicalType, ToxicityLevel ToxicityLevel,
    string? ActiveIngredient, string? Concentration,
    string? TargetPest, string? TargetCrop,
    string? ApplicationMethod, string? SafetyPrecautions,
    string? StorageConditions, int? WithholdingPeriodDays,
    string? RegistrationNumber, DateTime? ExpiryDate,
    decimal? VolumeOrWeightPerUnit, string? Unit);

public record UpdateChemicalInfoRequest(
    Guid Id,
    ChemicalType ChemicalType, ToxicityLevel ToxicityLevel,
    string? ActiveIngredient, string? Concentration,
    string? TargetPest, string? TargetCrop,
    string? ApplicationMethod, string? SafetyPrecautions,
    string? StorageConditions, int? WithholdingPeriodDays,
    string? RegistrationNumber, DateTime? ExpiryDate,
    decimal? VolumeOrWeightPerUnit, string? Unit);

public record GetChemicalInfoByProductRequest(Guid ProductId);
public record GetChemicalInfoRequest(Guid Id);
public record DeleteChemicalInfoRequest(Guid Id);
