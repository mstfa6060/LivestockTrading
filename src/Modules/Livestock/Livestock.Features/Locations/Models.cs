using Livestock.Domain.Enums;

namespace Livestock.Features.Locations;

public record LocationDetail(
    Guid Id, string CountryCode, string? CountryName,
    string? State, string? City, string? District, string? PostalCode,
    string? AddressLine, double? Latitude, double? Longitude,
    LocationType LocationType, Guid OwnerId, string OwnerType, DateTime CreatedAt);

public record CreateLocationRequest(
    string CountryCode, string? CountryName,
    string? State, string? City, string? District, string? PostalCode,
    string? AddressLine, double? Latitude, double? Longitude,
    LocationType LocationType, Guid OwnerId, string OwnerType);

public record UpdateLocationRequest(
    Guid Id,
    string CountryCode, string? CountryName,
    string? State, string? City, string? District, string? PostalCode,
    string? AddressLine, double? Latitude, double? Longitude,
    LocationType LocationType);

public record GetLocationsByOwnerRequest(Guid OwnerId, string OwnerType);
public record GetLocationRequest(Guid Id);
public record DeleteLocationRequest(Guid Id);
