using Livestock.Domain.Enums;

namespace Livestock.Features.FeedInfos;

public record FeedInfoDetail(
    Guid Id, Guid ProductId,
    FeedType FeedType, FeedForm FeedForm,
    string? TargetSpecies,
    decimal? ProteinPercentage, decimal? FatPercentage,
    decimal? FiberPercentage, decimal? MoisturePercentage,
    string? Ingredients, string? Additives,
    DateTime? ManufactureDate, DateTime? ExpiryDate,
    string? CertificationInfo, bool IsOrganic,
    decimal? PackageSizeKg, DateTime CreatedAt);

public record CreateFeedInfoRequest(
    Guid ProductId,
    FeedType FeedType, FeedForm FeedForm,
    string? TargetSpecies,
    decimal? ProteinPercentage, decimal? FatPercentage,
    decimal? FiberPercentage, decimal? MoisturePercentage,
    string? Ingredients, string? Additives,
    DateTime? ManufactureDate, DateTime? ExpiryDate,
    string? CertificationInfo, bool IsOrganic,
    decimal? PackageSizeKg);

public record UpdateFeedInfoRequest(
    Guid Id,
    FeedType FeedType, FeedForm FeedForm,
    string? TargetSpecies,
    decimal? ProteinPercentage, decimal? FatPercentage,
    decimal? FiberPercentage, decimal? MoisturePercentage,
    string? Ingredients, string? Additives,
    DateTime? ManufactureDate, DateTime? ExpiryDate,
    string? CertificationInfo, bool IsOrganic,
    decimal? PackageSizeKg);

public record GetFeedInfoByProductRequest(Guid ProductId);
public record GetFeedInfoRequest(Guid Id);
public record DeleteFeedInfoRequest(Guid Id);
