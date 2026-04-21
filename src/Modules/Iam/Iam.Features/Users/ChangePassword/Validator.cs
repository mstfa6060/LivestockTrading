using FastEndpoints;
using FluentValidation;

namespace Iam.Features.Users.ChangePassword;

public sealed class ChangePasswordValidator : Validator<ChangePasswordRequest>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty().WithMessage("Current password is required.");
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(6).WithMessage("New password must be at least 6 characters.");
        RuleFor(x => x.ConfirmNewPassword).NotEmpty().WithMessage("Password confirmation is required.");
    }
}
