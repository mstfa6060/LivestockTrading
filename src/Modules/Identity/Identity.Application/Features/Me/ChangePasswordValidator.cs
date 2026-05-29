using FluentValidation;

namespace LivestockTrading.Identity.Application.Features.Me;

public sealed class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.Dto.CurrentPassword)
            .NotEmpty().WithMessage("CurrentPassword is required.");

        // HashedPassword Domain guard: 8-256 char. Validator min-bound surfaces
        // client-friendly INVALID_REQUEST rather than USER_RULE_VIOLATION.
        RuleFor(x => x.Dto.NewPassword)
            .NotEmpty().WithMessage("NewPassword is required.")
            .MinimumLength(8).WithMessage("NewPassword must be at least 8 characters.");
    }
}
