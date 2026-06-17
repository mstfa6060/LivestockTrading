namespace Iam.Features.Auth.ForgotPassword;

public sealed record ForgotPasswordRequest(string Email);

public sealed record ForgotPasswordResponse(bool Success, string Message);
