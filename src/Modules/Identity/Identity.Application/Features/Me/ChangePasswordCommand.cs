namespace LivestockTrading.Identity.Application.Features.Me;

/// <summary>
/// POST /identity/users/me/password — change-password request. Current password
/// verify happens in the handler (Domain ChangePassword only accepts the new
/// hash, no proof-of-current check), and refresh tokens are revoked inline on
/// success (Domain ChangePassword does not cascade). IpAddress comes from the
/// endpoint so the UserPasswordChanged event carries the originating IP.
/// </summary>
public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword);

public sealed record ChangePasswordCommand(Guid UserId, ChangePasswordRequest Dto, string IpAddress);
