using Livestock.Domain.Enums;

namespace Livestock.Features.AnimalInfos;

public record AnimalInfoDetail(
    Guid Id, Guid ProductId,
    string Species, string? Breed,
    AnimalGender Gender, int? AgeMonths, decimal? WeightKg,
    HealthStatus HealthStatus, AnimalPurpose Purpose,
    bool IsVaccinated, bool HasHealthCertificate,
    string? EarTagNumber, string? PassportNumber, string? Color,
    bool IsPregnant, string? BreedingHistory, DateTime CreatedAt);

public record CreateAnimalInfoRequest(
    Guid ProductId,
    string Species, string? Breed,
    AnimalGender Gender, int? AgeMonths, decimal? WeightKg,
    HealthStatus HealthStatus, AnimalPurpose Purpose,
    bool IsVaccinated, bool HasHealthCertificate,
    string? EarTagNumber, string? PassportNumber, string? Color,
    bool IsPregnant, string? BreedingHistory);

public record UpdateAnimalInfoRequest(
    Guid Id,
    string Species, string? Breed,
    AnimalGender Gender, int? AgeMonths, decimal? WeightKg,
    HealthStatus HealthStatus, AnimalPurpose Purpose,
    bool IsVaccinated, bool HasHealthCertificate,
    string? EarTagNumber, string? PassportNumber, string? Color,
    bool IsPregnant, string? BreedingHistory);

public record GetAnimalInfoRequest(Guid Id);
public record GetAnimalInfoByProductRequest(Guid ProductId);
public record DeleteAnimalInfoRequest(Guid Id);
