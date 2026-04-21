using Livestock.Domain.Enums;

namespace Livestock.Features.Sellers;

public record SellerListItem(Guid Id, Guid UserId, string BusinessName, SellerStatus Status, double AverageRating, int ReviewCount, DateTime CreatedAt);
public record SellerDetail(Guid Id, Guid UserId, string BusinessName, string? Description, string? PhoneNumber, string? Email, string? WebsiteUrl, string? TaxNumber, string? LogoUrl, SellerStatus Status, double AverageRating, int ReviewCount, DateTime? VerifiedAt, DateTime CreatedAt);
public record BecomeSellerRequest(string BusinessName, string? Description, string? PhoneNumber, string? Email, string? WebsiteUrl, string? TaxNumber);
public record UpdateSellerRequest(string BusinessName, string? Description, string? PhoneNumber, string? Email, string? WebsiteUrl, string? TaxNumber);
public record GetSellerRequest(Guid Id);
public record GetSellerByUserIdRequest(Guid UserId);
public record GetNearbySellersRequest(double Latitude, double Longitude, double RadiusKm = 50, int Limit = 20);
public record VerifySellerRequest(Guid Id);
public record SuspendSellerRequest(Guid Id, string Reason);
