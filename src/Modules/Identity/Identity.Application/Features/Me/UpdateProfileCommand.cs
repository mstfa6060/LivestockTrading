namespace LivestockTrading.Identity.Application.Features.Me;

/// <summary>
/// PATCH /identity/users/me request body — firstName/lastName always overwrite,
/// nationalId is set-once (Domain AssignNationalId throws if already set; the
/// handler surfaces that as USER_RULE_VIOLATION). Email change rides on a
/// separate endpoint (W4.2.C scope-out, B-W4.2-C-3 reconcile).
/// </summary>
public sealed record UpdateProfileRequest(string FirstName, string LastName, string? NationalId);

public sealed record UpdateProfileCommand(Guid UserId, UpdateProfileRequest Dto);
