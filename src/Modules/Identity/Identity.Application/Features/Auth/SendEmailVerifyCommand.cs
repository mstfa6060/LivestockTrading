namespace LivestockTrading.Identity.Application.Features.Auth;

/// <summary>
/// POST /identity/auth/email/send-verify — issues an email verification token
/// and dispatches the verification email. To prevent email-enumeration, the
/// handler returns success even when the email is unknown or already verified
/// (no ticket persisted, no mail dispatched). Token TTL is 24 hours (doc has
/// no explicit value; email-link flows are routinely longer than phone OTP).
/// </summary>
public sealed record SendEmailVerifyRequest(string Email);

public sealed record SendEmailVerifyCommand(SendEmailVerifyRequest Dto, string IpAddress);
