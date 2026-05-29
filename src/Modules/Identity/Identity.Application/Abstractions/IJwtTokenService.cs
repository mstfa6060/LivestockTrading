using LivestockTrading.Identity.Domain.Aggregates;

namespace LivestockTrading.Identity.Application.Abstractions;

/// <summary>
/// Application port for JWT access token lifecycle.
/// Concrete implementation lives in Identity.Infrastructure (Wave 4 W4.3) and
/// performs RS256 signing with 90-day key rotation plus jti blacklist lookups
/// against Redis (plan-doc 05-identity §7). The OpenIddict /connect/* endpoint
/// surface is intentionally out of W4.2 scope and will be wired in a later
/// sub-batch — this port covers the REST shim issuance path only.
/// </summary>
public interface IJwtTokenService
{
    Task<AccessToken> IssueAsync(User user, Guid deviceId, CancellationToken ct);
    Task<bool> IsRevokedAsync(string jti, CancellationToken ct);
    Task RevokeAsync(string jti, DateTimeOffset until, CancellationToken ct);
}

public sealed record AccessToken(string Value, string Jti, DateTimeOffset ExpiresAt);
