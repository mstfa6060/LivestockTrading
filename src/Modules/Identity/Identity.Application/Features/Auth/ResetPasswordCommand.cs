namespace LivestockTrading.Identity.Application.Features.Auth;

/// <summary>
/// POST /identity/auth/password/reset — consumes the channel credential
/// (email link token OR phone OTP) and applies the new password. Method
/// discriminates the channel; Credential is the raw verification value to
/// hash and FixedTimeEquals against the active ticket. On success the user
/// password is replaced and all active refresh tokens are revoked
/// (ChangePasswordHandler cascade emsali). Unlike forgot, reset does NOT
/// hide identifier validity: failure surfaces a generic INVALID_OR_EXPIRED
/// error so identifier existence is not leaked but the client knows the
/// reset attempt failed.
/// </summary>
public sealed record ResetPasswordRequest(string Method, string Identifier, string Credential, string NewPassword);

public sealed record ResetPasswordCommand(ResetPasswordRequest Dto, string IpAddress);
