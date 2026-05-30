using Shared.Contracts.Identity;

namespace LivestockTrading.Identity.Application.Features.Auth;

/// <summary>
/// Register-with-social request body — also covers the auto-link path
/// (plan-doc §10): if the validated email already belongs to a user, the
/// provider is attached to that account instead of creating a new one.
/// Consents reuse the ConsentGrantDto record defined alongside the password
/// register command (same namespace, no duplicate declaration). DeviceFingerprint
/// and Platform are required for the Login-style token issuance flow.
/// </summary>
public sealed record RegisterWithSocialRequest(
    string Provider,
    string IdToken,
    AccountType AccountType,
    UserPreferences Preferences,
    IReadOnlyList<ConsentGrantDto> Consents,
    string DeviceFingerprint,
    string Platform);

public sealed record RegisterWithSocialCommand(
    RegisterWithSocialRequest Dto,
    string IpAddress,
    string UserAgent);
