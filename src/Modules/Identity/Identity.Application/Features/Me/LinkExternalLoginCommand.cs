namespace LivestockTrading.Identity.Application.Features.Me;

/// <summary>
/// POST /identity/users/me/external-logins — attach a new OAuth provider login
/// to the authenticated user. Provider must be one of google or apple (validator
/// enforced); IdToken is dispatched to IExternalLoginValidator, which is Faz 1
/// NoOp until W4.3 wires real Google JWKS and Apple ES256 verification.
/// </summary>
public sealed record LinkExternalLoginRequest(string Provider, string IdToken);

public sealed record LinkExternalLoginCommand(Guid UserId, LinkExternalLoginRequest Dto);
