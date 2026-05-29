using Shared.Contracts.Identity;

namespace LivestockTrading.Identity.Application.Features.Auth;

/// <summary>
/// Login/refresh response payload. Carries the freshly issued access token
/// (short-lived, 15dk per plan-doc §3), the opaque refresh token raw value
/// (long-lived, 30 or 90 days depending on rememberMe) and a cross-module
/// UserSummary projection for the calling client. Consumed by Login (W4.2.B.2)
/// and Refresh handlers — placed alongside auth features so the response shape
/// is co-located with the lifecycle that produces it.
/// </summary>
public sealed record LoginResponse(
    string AccessToken,
    DateTimeOffset AccessExpiresAt,
    string RefreshToken,
    DateTimeOffset RefreshExpiresAt,
    UserSummary User);
