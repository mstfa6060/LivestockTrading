using Livestock.Domain.Enums;

namespace Livestock.Features.SeedInfos;

public record SeedInfoDetail(
    Guid Id, Guid ProductId,
    SeedType SeedType, string? Variety, string? ScientificName,
    decimal? GerminationRatePercent, int? DaysToMaturity,
    decimal? PackageSizeGrams,
    DateTime? HarvestDate, DateTime? ExpiryDate,
    bool IsHybrid, bool IsOrganic, bool IsGmoFree,
    string? SuitableClimate, string? PlantingInstructions,
    string? CertificationInfo, DateTime CreatedAt);

public record CreateSeedInfoRequest(
    Guid ProductId,
    SeedType SeedType, string? Variety, string? ScientificName,
    decimal? GerminationRatePercent, int? DaysToMaturity,
    decimal? PackageSizeGrams,
    DateTime? HarvestDate, DateTime? ExpiryDate,
    bool IsHybrid, bool IsOrganic, bool IsGmoFree,
    string? SuitableClimate, string? PlantingInstructions,
    string? CertificationInfo);

public record UpdateSeedInfoRequest(
    Guid Id,
    SeedType SeedType, string? Variety, string? ScientificName,
    decimal? GerminationRatePercent, int? DaysToMaturity,
    decimal? PackageSizeGrams,
    DateTime? HarvestDate, DateTime? ExpiryDate,
    bool IsHybrid, bool IsOrganic, bool IsGmoFree,
    string? SuitableClimate, string? PlantingInstructions,
    string? CertificationInfo);

public record GetSeedInfoByProductRequest(Guid ProductId);
public record GetSeedInfoRequest(Guid Id);
public record DeleteSeedInfoRequest(Guid Id);
