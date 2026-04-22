using Livestock.Domain.Enums;

namespace Livestock.Features.Transporters;

public record TransporterListItem(Guid Id, Guid UserId, string CompanyName, TransporterStatus Status, bool IsActive, bool IsVerified, double AverageRating, int ReviewCount, DateTime CreatedAt);
public record TransporterDetail(Guid Id, Guid UserId, string CompanyName, string? Description, string? PhoneNumber, string? Email, string? WebsiteUrl, string? LicenseNumber, string? LogoUrl, TransporterStatus Status, bool IsActive, bool IsVerified, double AverageRating, int ReviewCount, DateTime? VerifiedAt, DateTime CreatedAt);
public record BecomeTransporterRequest(string CompanyName, string? Description, string? PhoneNumber, string? Email, string? WebsiteUrl, string? LicenseNumber);
public record UpdateTransporterRequest(string CompanyName, string? Description, string? PhoneNumber, string? Email, string? WebsiteUrl, string? LicenseNumber);
public record GetTransporterRequest(Guid Id);
public record VerifyTransporterRequest(Guid Id);
public record SuspendTransporterRequest(Guid Id, string Reason);
