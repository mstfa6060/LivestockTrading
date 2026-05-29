namespace LivestockTrading.Identity.Application.Features.Me;

/// <summary>
/// DELETE /identity/users/me/devices/{deviceId} — drop a device from the user's
/// device list. Domain RemoveDevice raises DomainException for unknown ids,
/// which the handler surfaces as USER_RULE_VIOLATION.
/// </summary>
public sealed record RemoveDeviceCommand(Guid UserId, Guid DeviceId);
