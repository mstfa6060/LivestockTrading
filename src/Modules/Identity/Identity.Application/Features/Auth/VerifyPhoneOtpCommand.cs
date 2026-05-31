namespace LivestockTrading.Identity.Application.Features.Auth;

/// <summary>
/// POST /identity/auth/phone/verify — consumes a 6-digit OTP issued by
/// SendPhoneOtp (Register purpose). Public/anonymous: phone-number ownership is
/// proven by the OTP itself (only the SIM holder can read the SMS). On success
/// the matching User (if any) gets User.VerifyPhone called; if no User is
/// linked yet (anonymous pre-register flow) the consumed ticket alone is the
/// proof — register can later associate the verified phone. Failure surfaces a
/// generic INVALID_OR_EXPIRED to avoid leaking ticket existence (Reset emsali).
/// </summary>
public sealed record VerifyPhoneOtpRequest(string Phone, string Code);

public sealed record VerifyPhoneOtpCommand(VerifyPhoneOtpRequest Dto, string IpAddress);
