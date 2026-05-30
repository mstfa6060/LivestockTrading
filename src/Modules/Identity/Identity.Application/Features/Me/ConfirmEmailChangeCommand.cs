namespace LivestockTrading.Identity.Application.Features.Me;

/// <summary>
/// POST /identity/users/me/email-change/confirm — consumes the email-change
/// verification token issued by RequestEmailChange. Email field in the body
/// MUST equal User.PendingEmail (defense-in-depth: token possession alone is
/// not enough — confirms the correct mailbox was reached). Domain
/// ConfirmEmailChange performs FixedTimeEquals on the SHA-256 hash and raises
/// UserEmailVerified on success. Token raw is hashed by
/// IEmailVerificationTokenGenerator.Hash (FromHexString → SHA256, D.1c
/// contract) before being passed to Domain.
/// </summary>
public sealed record ConfirmEmailChangeRequest(string Email, string Token);

public sealed record ConfirmEmailChangeCommand(Guid UserId, ConfirmEmailChangeRequest Dto, string IpAddress);
