using Livestock.Domain.Enums;

namespace Livestock.Features.Admin.Transporters;

public record PendingTransporterItem(
    Guid Id,
    Guid UserId,
    string CompanyName,
    string? Email,
    string? PhoneNumber,
    string? LicenseNumber,
    TransporterStatus Status,
    DateTime CreatedAt);

public record RejectTransporterRequest(Guid Id, string Reason);
