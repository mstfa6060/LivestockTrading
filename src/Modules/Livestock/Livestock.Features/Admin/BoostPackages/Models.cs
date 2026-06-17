using Livestock.Domain.Enums;

namespace Livestock.Features.Admin.BoostPackages;

public record BoostPackageAdminItem(
    Guid Id,
    string Name,
    string? Description,
    BoostType BoostType,
    int DurationDays,
    decimal Price,
    string CurrencyCode,
    string? AppStoreProductId,
    string? PlayStoreProductId,
    int MultiplierFactor,
    bool IsActive,
    DateTime CreatedAt);

public record CreateBoostPackageRequest(
    string Name,
    string? Description,
    BoostType BoostType,
    int DurationDays,
    decimal Price,
    string CurrencyCode,
    string? AppStoreProductId,
    string? PlayStoreProductId,
    int MultiplierFactor = 1);

public record UpdateBoostPackageRequest(
    Guid Id,
    string Name,
    string? Description,
    int DurationDays,
    decimal Price,
    string CurrencyCode,
    string? AppStoreProductId,
    string? PlayStoreProductId,
    int MultiplierFactor,
    bool IsActive);

public record GetBoostPackageAdminRequest(Guid Id);
public record DeleteBoostPackageRequest(Guid Id);
