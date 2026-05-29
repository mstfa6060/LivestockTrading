namespace LivestockTrading.Identity.Application.Features.Me;

/// <summary>
/// PATCH /identity/users/me/devices/{deviceId}/push-token — set or clear the
/// FCM/APNs push token on a device. PushToken null clears the token (Domain
/// UpdateDevicePushToken accepts null); no validator is wired because the body
/// shape has no required fields beyond the optional token itself.
/// </summary>
public sealed record UpdatePushTokenRequest(string? PushToken);

public sealed record UpdatePushTokenCommand(Guid UserId, Guid DeviceId, UpdatePushTokenRequest Dto);
