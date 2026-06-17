namespace Iam.Features.Auth.SendOtp;

public sealed record SendOtpRequest(string PhoneNumber);

public sealed record SendOtpResponse(bool Success, string Message);
