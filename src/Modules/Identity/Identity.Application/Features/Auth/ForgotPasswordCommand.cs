namespace LivestockTrading.Identity.Application.Features.Auth;

/// <summary>
/// POST /identity/auth/password/forgot — initiates a password-reset flow over
/// the chosen channel (email link or phone OTP). Method discriminates the
/// channel (Login 3-method emsali — "email" | "phone"; nationalId is NOT a
/// reset channel since it has no out-of-band delivery). Identifier carries the
/// email address or E.164 phone number. The handler is enumeration-safe: it
/// returns Success regardless of whether the identifier resolves to a real
/// user (SendEmailVerifyHandler enumeration-protection emsali).
/// </summary>
public sealed record ForgotPasswordRequest(string Method, string Identifier);

public sealed record ForgotPasswordCommand(ForgotPasswordRequest Dto, string IpAddress);
