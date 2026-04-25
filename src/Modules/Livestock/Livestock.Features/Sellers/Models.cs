using Livestock.Domain.Enums;

namespace Livestock.Features.Sellers;

public record SellerListItem(Guid Id, Guid UserId, string BusinessName, SellerStatus Status, bool IsActive, bool IsVerified, double AverageRating, int ReviewCount, DateTime CreatedAt);
public record SellerDetail(Guid Id, Guid UserId, string BusinessName, string? Description, string? PhoneNumber, string? Email, string? WebsiteUrl, string? TaxNumber, string? LogoUrl, SellerStatus Status, bool IsActive, bool IsVerified, double AverageRating, int ReviewCount, DateTime? VerifiedAt, DateTime CreatedAt);
public record BecomeSellerRequest(string BusinessName, string? Description, string? PhoneNumber, string? Email, string? WebsiteUrl, string? TaxNumber);
public record UpdateSellerRequest(string BusinessName, string? Description, string? PhoneNumber, string? Email, string? WebsiteUrl, string? TaxNumber);
public record GetSellerRequest(Guid Id);
public record VerifySellerRequest(Guid Id);
public record SuspendSellerRequest(Guid Id, string Reason);

public record NearbySellersRequest(
    double Latitude,
    double Longitude,
    string? CountryCode,
    int Limit = 10,
    double RadiusKm = 50.0);

public record NearbySellerItem(
    Guid SellerId,
    Guid UserId,
    string BusinessName,
    string? LogoUrl,
    bool IsVerified,
    SellerStatus Status,
    double AverageRating,
    int ReviewCount,
    string? City,
    string CountryCode,
    double Latitude,
    double Longitude,
    double DistanceKm);
