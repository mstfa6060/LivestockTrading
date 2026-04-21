using Livestock.Domain.Enums;

namespace Livestock.Features.Sellers;

public record SellerListItem(Guid Id, Guid UserId, string BusinessName, SellerStatus Status, double AverageRating, int ReviewCount, DateTime CreatedAt);
public record SellerDetail(Guid Id, Guid UserId, string BusinessName, string? Description, string? PhoneNumber, string? Email, string? WebsiteUrl, string? TaxNumber, string? LogoUrl, SellerStatus Status, double AverageRating, int ReviewCount, DateTime? VerifiedAt, DateTime CreatedAt);
public record BecomeSellerRequest(string BusinessName, string? Description, string? PhoneNumber, string? Email, string? WebsiteUrl, string? TaxNumber);
public record UpdateSellerRequest(string BusinessName, string? Description, string? PhoneNumber, string? Email, string? WebsiteUrl, string? TaxNumber);
public record GetSellerRequest(Guid Id);
public record VerifySellerRequest(Guid Id);
public record SuspendSellerRequest(Guid Id, string Reason);
public record DeleteSellerRequest(Guid Id);
public record GetNearbySellersRequest(double Lat, double Lng, double RadiusKm = 50);
public record NearbySeller(Guid Id, Guid UserId, string BusinessName, SellerStatus Status, double AverageRating, int ReviewCount, double DistanceKm, double Latitude, double Longitude, DateTime CreatedAt);
