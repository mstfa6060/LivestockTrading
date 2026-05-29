using Shared.Contracts.Identity;

namespace LivestockTrading.Identity.Application.Features.Auth;

/// <summary>
/// Register-with-password request body. KVKK consents are required (plan-doc
/// §2 satir 87 — TermsAndPrivacy + MinistryDataShare zorunlu) and ride as a
/// list keyed by ConsentType + version. Preferences uses the Shared.Contracts
/// UserPreferences record directly (no separate Dto needed — the record is the
/// cross-module shape and the Domain property type both). Phone and NationalId
/// are optional; Domain VO ctors enforce E.164 and TC checksum validation.
/// </summary>
public sealed record RegisterWithPasswordRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? Phone,
    string? NationalId,
    AccountType AccountType,
    UserPreferences Preferences,
    IReadOnlyList<ConsentGrantDto> Consents);

/// <summary>
/// Consent grant request shape — mirrors Domain ConsentGrant value object but
/// is defined here as a transport DTO so the request body deserializes cleanly
/// without exposing Domain types to clients.
/// </summary>
public sealed record ConsentGrantDto(ConsentType Type, string Version, bool Granted);

public sealed record RegisterWithPasswordCommand(
    RegisterWithPasswordRequest Dto,
    string IpAddress,
    string UserAgent);
