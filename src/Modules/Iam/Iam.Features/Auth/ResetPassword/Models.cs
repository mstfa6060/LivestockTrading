namespace Iam.Features.Auth.ResetPassword;

public sealed record ResetPasswordRequest(string Email, string Token, string NewPassword);

public sealed record ResetPasswordResponse(bool Success);
