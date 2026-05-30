namespace LivestockTrading.Identity.Application.Features.Me;

/// <summary>
/// POST /identity/users/me/email-change/request — initiate email-change flow.
/// Domain User.RequestEmailChange self-generates the opaque token (32-byte RNG
/// hex), hashes it (SHA-256 of the hex-decoded bytes) onto User.PendingEmailTokenHash,
/// and returns the raw value to be mailed. The handler must NOT echo the raw
/// token in the HTTP response — it travels only via IEmailSender. IpAddress is
/// carried for parity with other /me mutations; Domain currently does not store
/// it for this flow but the field is reserved for future audit.
/// </summary>
public sealed record RequestEmailChangeRequest(string NewEmail);

public sealed record RequestEmailChangeCommand(Guid UserId, RequestEmailChangeRequest Dto, string IpAddress);
