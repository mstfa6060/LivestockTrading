namespace LivestockTrading.Identity.Application.Features.Me;

/// <summary>
/// DELETE /identity/users/me/email-change — cancels a pending email-change
/// request. No body — the operation is parameterless aside from the
/// authenticated UserId. Domain CancelPendingEmailChange unconditionally nulls
/// the PendingEmail* fields (idempotent no-op when nothing is pending), so
/// repeat calls are safe. IpAddress is carried for parity with other /me
/// mutations.
/// </summary>
public sealed record CancelEmailChangeCommand(Guid UserId, string IpAddress);
