namespace Iam.Features.Auth.VerifyEmailOtp;

public sealed record VerifyEmailOtpRequest(string Email, string OtpCode);

public sealed record VerifyEmailOtpResponse(bool Success);
