namespace LivestockTrading.Identity.Application.Features.Me;

/// <summary>
/// DELETE /identity/users/me/external-logins/{externalLoginId} — detach an
/// OAuth provider from the user. Domain UnlinkExternalLogin guards against
/// removing the last remaining sign-in method (password null + single external
/// login → DomainException surfaces as USER_RULE_VIOLATION).
/// </summary>
public sealed record UnlinkExternalLoginCommand(Guid UserId, Guid ExternalLoginId);
