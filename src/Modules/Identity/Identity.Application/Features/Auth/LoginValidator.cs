using FluentValidation;

namespace LivestockTrading.Identity.Application.Features.Auth;

public sealed class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Dto.Method)
            .NotEmpty().WithMessage("Method is required.")
            .Must(m => m is "email" or "phone" or "nationalId")
            .WithMessage("Method must be one of: email, phone, nationalId.");

        RuleFor(x => x.Dto.Identifier)
            .NotEmpty().WithMessage("Identifier is required.");

        RuleFor(x => x.Dto.Password)
            .NotEmpty().WithMessage("Password is required.");

        RuleFor(x => x.Dto.DeviceFingerprint)
            .NotEmpty().WithMessage("DeviceFingerprint is required.");

        RuleFor(x => x.Dto.Platform)
            .NotEmpty().WithMessage("Platform is required.");
    }
}
