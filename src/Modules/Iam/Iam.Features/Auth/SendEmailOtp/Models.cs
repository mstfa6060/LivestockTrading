namespace Iam.Features.Auth.SendEmailOtp;

public sealed record SendEmailOtpRequest(string Email);

public sealed record SendEmailOtpResponse(bool Success, string Message);
