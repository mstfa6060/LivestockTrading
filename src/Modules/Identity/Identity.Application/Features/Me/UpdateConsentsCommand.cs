using Shared.Contracts.Identity;

namespace LivestockTrading.Identity.Application.Features.Me;

/// <summary>
/// PATCH /identity/users/me/consents request body — set of consent changes
/// to apply. Granted=true records a new consent grant (RecordConsent), Granted=false
/// revokes any active consent of that type (RevokeConsent). Mandatory KVKK
/// consent revoke (TermsAndPrivacy, MinistryDataShare) is blocked by the
/// validator — Domain itself does not enforce mandatory-consent invariants.
/// </summary>
public sealed record ConsentChangeDto(ConsentType Type, string Version, bool Granted);

public sealed record UpdateConsentsRequest(IReadOnlyList<ConsentChangeDto> Changes);

public sealed record UpdateConsentsCommand(
    Guid UserId,
    UpdateConsentsRequest Dto,
    string IpAddress,
    string UserAgent);
