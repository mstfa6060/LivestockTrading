namespace LivestockTrading.Identity.Application.Features.AdminUsers;

/// <summary>
/// Command for admin-initiated user suspension (Active -> Suspended).
/// Domain User.Suspend cascades refresh-token revoke + raises UserSuspended event.
/// Reason is required; ActorAdminId is the admin's user id (sub claim).
/// </summary>
public sealed record SuspendUserRequest(string Reason);

public sealed record SuspendUserCommand(Guid UserId, string Reason, Guid ActorAdminId);
