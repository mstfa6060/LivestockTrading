using Livestock.Domain.Enums;

namespace Livestock.Features.Farms;

public record FarmListItem(Guid Id, Guid SellerId, string Name, FarmType FarmType, double? AreaHectares, int? CapacityHead, bool IsOrganic, DateTime CreatedAt);
public record FarmDetail(Guid Id, Guid SellerId, string Name, string? Description, FarmType FarmType, double? AreaHectares, int? CapacityHead, string? CertificationInfo, bool IsOrganic, string? WebsiteUrl, string? PhoneNumber, Guid? BucketId, DateTime CreatedAt);
public record CreateFarmRequest(string Name, string? Description, FarmType FarmType, double? AreaHectares, int? CapacityHead, string? CertificationInfo, bool IsOrganic, string? WebsiteUrl, string? PhoneNumber);
public record UpdateFarmRequest(Guid Id, string Name, string? Description, FarmType FarmType, double? AreaHectares, int? CapacityHead, string? CertificationInfo, bool IsOrganic, string? WebsiteUrl, string? PhoneNumber);
public record DeleteFarmRequest(Guid Id);
public record GetFarmRequest(Guid Id);
