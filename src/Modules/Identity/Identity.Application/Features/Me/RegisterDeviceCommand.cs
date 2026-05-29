namespace LivestockTrading.Identity.Application.Features.Me;

/// <summary>
/// POST /identity/users/me/devices — register a new device for the user.
/// UserAgent rides on the command (lifted from the request header by the
/// endpoint) so it can be stored alongside the device fingerprint without
/// trusting body input. PushToken is optional; mobile clients send it on
/// registration, web clients usually omit it.
/// </summary>
public sealed record RegisterDeviceRequest(string Platform, string DeviceFingerprint, string? PushToken);

public sealed record RegisterDeviceCommand(Guid UserId, RegisterDeviceRequest Dto, string? UserAgent);
