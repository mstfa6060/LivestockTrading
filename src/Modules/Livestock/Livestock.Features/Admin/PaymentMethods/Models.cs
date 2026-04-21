namespace Livestock.Features.Admin.PaymentMethods;

public record PaymentMethodItem(
    Guid Id,
    string Name,
    string Code,
    string? Description,
    string? IconUrl,
    bool IsActive,
    int SortOrder,
    DateTime CreatedAt);

public record GetPaymentMethodRequest(Guid Id);

public record CreatePaymentMethodRequest(
    string Name,
    string Code,
    string? Description,
    string? IconUrl,
    bool IsActive = true,
    int SortOrder = 0);

public record UpdatePaymentMethodRequest(
    Guid Id,
    string Name,
    string? Description,
    string? IconUrl,
    bool IsActive,
    int SortOrder);

public record DeletePaymentMethodRequest(Guid Id);
