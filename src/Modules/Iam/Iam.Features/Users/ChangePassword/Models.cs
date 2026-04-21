namespace Iam.Features.Users.ChangePassword;

public record ChangePasswordRequest(string CurrentPassword, string NewPassword, string ConfirmNewPassword);
public record ChangePasswordResponse(bool Success);
