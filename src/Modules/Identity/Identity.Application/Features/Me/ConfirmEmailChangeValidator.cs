using FluentValidation;

namespace LivestockTrading.Identity.Application.Features.Me;

public sealed class ConfirmEmailChangeValidator : AbstractValidator<ConfirmEmailChangeCommand>
{
    public ConfirmEmailChangeValidator()
    {
        RuleFor(x => x.Dto.Email)
            .NotEmpty().WithMessage("Email is required.");

        // Domain raw = Convert.ToHexString(32 byte) = 64-char hex (D.1c kontrati);
        // defansif client-friendly INVALID_REQUEST yerine Domain USER_RULE_VIOLATION.
        RuleFor(x => x.Dto.Token)
            .NotEmpty().WithMessage("Token is required.")
            .Length(64).WithMessage("Token must be 64 characters.");
    }
}
