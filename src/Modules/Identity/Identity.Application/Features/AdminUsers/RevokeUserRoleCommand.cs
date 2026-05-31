namespace LivestockTrading.Identity.Application.Features.AdminUsers;

/// <summary>
/// Command for admin-initiated role revoke. Domain User.RevokeRole throws when
/// the role is not currently active on the user (no silent miss). UserRoleRevoked
/// raised on success. Role is a string per 05-identity RBAC table (B-W4.1-4 cift
/// event: GrantRole + RevokeRole iki ayri event). ActorAdminId is the admin's
/// user id (sub claim).
/// </summary>
public sealed record RevokeUserRoleRequest(string Role);

public sealed record RevokeUserRoleCommand(Guid UserId, string Role, Guid ActorAdminId);
