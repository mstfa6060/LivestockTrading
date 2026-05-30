using LivestockTrading.Identity.Application.Abstractions;
using LivestockTrading.Identity.Application.Common.Mappers;
using LivestockTrading.Identity.Domain.Aggregates;

namespace LivestockTrading.Identity.Application.Features.Auth;

/// <summary>
/// Shared token issuance helper for fresh-session flows (Login + Social register).
/// Refresh-token rotation lives in its own handler because it preserves the
/// existing family id; the cases that share this helper all start a new family.
/// </summary>
internal static class TokenIssuance
{
    public static async Task<LoginResponse> IssueLoginTokensAsync(
        User user,
        Guid deviceId,
        bool rememberMe,
        IRefreshTokenGenerator rtGen,
        IJwtTokenService jwt,
        DateTimeOffset now,
        CancellationToken ct)
    {
        var pair = rtGen.Generate();
        var familyId = Guid.CreateVersion7();
        var ttl = rememberMe ? TimeSpan.FromDays(90) : TimeSpan.FromDays(30);
        user.IssueRefreshToken(deviceId, pair.Hash, familyId, ttl, now);

        var access = await jwt.IssueAsync(user, deviceId, ct);

        return new LoginResponse(
            access.Value,
            access.ExpiresAt,
            pair.Raw,
            now.Add(ttl),
            user.ToSummary());
    }
}
