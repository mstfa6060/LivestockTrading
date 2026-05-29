namespace LivestockTrading.Identity.Application.Features.Auth;

/// <summary>
/// Login request body — 3-method discriminator (plan-doc §4). DeviceFingerprint
/// and Platform are required (no anonymous device login in Faz 1); the validator
/// enforces non-empty values to keep User.RegisterDevice's non-null guards safe.
/// IpAddress and UserAgent are lifted from HttpContext by the endpoint and ride
/// on the command rather than the request body.
/// </summary>
public sealed record LoginRequest(
    string Method,
    string Identifier,
    string Password,
    bool RememberMe,
    string DeviceFingerprint,
    string Platform);

public sealed record LoginCommand(LoginRequest Dto, string IpAddress, string UserAgent);
