using Livestock.Domain.Enums;

namespace Livestock.Features.Admin.Sellers;

public record PendingSellerItem(
    Guid Id,
    Guid UserId,
    string BusinessName,
    string? Email,
    string? PhoneNumber,
    string? TaxNumber,
    SellerStatus Status,
    DateTime CreatedAt);

public record RejectSellerRequest(Guid Id, string Reason);
