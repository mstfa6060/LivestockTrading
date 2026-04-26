using Livestock.Domain.Enums;

namespace Livestock.Features.Locations;

public record LocationDetail(
    Guid Id, string CountryCode, string? CountryName,
    string? State, string? City, string? District, string? PostalCode,
    string? AddressLine, double? Latitude, double? Longitude,
    LocationType LocationType, Guid OwnerId, string OwnerType, DateTime CreatedAt);

// Additive legacy-compat: the old frontend client sends AddressLine1/
// AddressLine2 (not AddressLine), DistrictId (not District string), omits
// OwnerId / OwnerType entirely, and uses `Type` instead of `LocationType`.
// Handler normalizes these before inserting the Location row.
public record CreateLocationRequest(
    string CountryCode, string? CountryName,
    string? State, string? City, string? District, string? PostalCode,
    string? AddressLine, double? Latitude, double? Longitude,
    LocationType LocationType,
    Guid? OwnerId, string? OwnerType,
    string? AddressLine1, string? AddressLine2,
    int? DistrictId,
    LocationType? Type);

public record UpdateLocationRequest(
    Guid Id,
    string CountryCode, string? CountryName,
    string? State, string? City, string? District, string? PostalCode,
    string? AddressLine, double? Latitude, double? Longitude,
    LocationType LocationType);

public record GetLocationsByOwnerRequest(Guid OwnerId, string OwnerType);
public record GetLocationRequest(Guid Id);
public record DeleteLocationRequest(Guid Id);
