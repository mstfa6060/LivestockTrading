namespace LivestockTrading.Identity.Application.Features.Auth;

/// <summary>
/// POST /identity/auth/email/verify — consumes a verification token and marks
/// the user's email as verified. Token validity (consumed/expired/max-attempt)
/// is checked by EmailVerificationTicket.TryConsume; ticket DomainExceptions
/// surface as USER_RULE_VIOLATION since they semantically describe a stale or
/// abused ticket.
/// </summary>
public sealed record VerifyEmailRequest(string Email, string Token);

public sealed record VerifyEmailCommand(VerifyEmailRequest Dto);
