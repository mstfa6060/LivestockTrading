namespace LivestockTrading.Identity.Application.Features.AdminUsers;

/// <summary>
/// Command for admin-initiated user reactivation (Suspended -> Active).
/// Domain User.Reactivate guard: only Suspended status accepted (throw on others).
/// Id-only operation; no DTO body. ActorAdminId is the admin's user id (sub claim).
/// </summary>
public sealed record ReactivateUserCommand(Guid UserId, Guid ActorAdminId);
