namespace Iam.Features.Auth.VerifyOtp;

public sealed record VerifyOtpRequest(string PhoneNumber, string OtpCode);

public sealed record VerifyOtpResponse(bool Success);
