namespace LivestockTrading.Identity.Application.Features.AdminUsers;

/// <summary>
/// Command for admin-initiated force-logout. Revokes all active refresh tokens
/// for the target user via User.RevokeAllRefreshTokens (no dedicated Domain
/// method exists; handler delegates to the existing cascade). RevocationReason
/// is AdminRevoked. Idempotent: no active tokens means no event raised. Id-only
/// operation; no DTO body. ActorAdminId is the admin's user id (sub claim).
/// </summary>
public sealed record ForceLogoutUserCommand(Guid UserId, Guid ActorAdminId);
