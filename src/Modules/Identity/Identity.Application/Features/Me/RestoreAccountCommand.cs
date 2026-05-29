namespace LivestockTrading.Identity.Application.Features.Me;

/// <summary>
/// POST /identity/users/me/account/restore — cancels a pending deletion and
/// returns the user to Active status (Domain CancelDeletion, guarded against
/// non-pending-deletion callers).
/// </summary>
public sealed record RestoreAccountCommand(Guid UserId);
