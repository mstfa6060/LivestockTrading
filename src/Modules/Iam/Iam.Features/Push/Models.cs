using Iam.Domain.Enums;

namespace Iam.Features.Push;

public sealed record RegisterPushTokenRequest(
    string PushToken,
    string DeviceId,
    string AppName,
    UserPushPlatform Platform);

public sealed record RegisterPushTokenResponse(bool Success);

public sealed record RevokePushTokenRequest(string DeviceId);

public sealed record RevokePushTokenResponse(bool Success);
