namespace LivestockTrading.Identity.Application.Features.AdminUsers;

/// <summary>
/// Command for admin-initiated role grant. Domain User.GrantRole is idempotent
/// (already-active role returns silently with no event raise); UserRoleGranted
/// raised on first grant. Role is a string per 05-identity RBAC table (buyer,
/// seller, carrier, vet, moderator, admin). ActorAdminId is the admin's user
/// id (sub claim).
/// </summary>
public sealed record GrantUserRoleRequest(string Role);

public sealed record GrantUserRoleCommand(Guid UserId, string Role, Guid ActorAdminId);
