namespace LivestockTrading.Identity.Application.Features.Auth;

/// <summary>
/// POST /identity/auth/phone/send-code — issues a 6-digit OTP for phone-number
/// verification (Register purpose; ChangePhone + LoginPhoneOtp are Faz 2 per
/// decision B-W4.2-D2-1 / Karar Z). The handler is enumeration-safe: invalid
/// phone format or unknown phone resolves to Success without revealing whether
/// the number is registered (Forgot password enumeration-protection emsali).
/// </summary>
public sealed record SendPhoneOtpRequest(string Phone);

public sealed record SendPhoneOtpCommand(SendPhoneOtpRequest Dto, string IpAddress);
