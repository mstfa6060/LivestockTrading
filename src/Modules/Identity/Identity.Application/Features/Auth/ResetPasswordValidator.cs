using FluentValidation;

namespace LivestockTrading.Identity.Application.Features.Auth;

public sealed class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
{
    private static readonly string[] AllowedMethods = ["email", "phone"];

    public ResetPasswordValidator()
    {
        RuleFor(x => x.Dto.Method)
            .NotEmpty().WithMessage("Method is required.")
            .Must(m => AllowedMethods.Contains(m))
            .WithMessage("Method must be 'email' or 'phone'.");

        RuleFor(x => x.Dto.Identifier)
            .NotEmpty().WithMessage("Identifier is required.");

        // Credential length conditional (email=64 hex, phone=6 numeric) handler/Domain TryConsume
        // tarafindan yakalanir; validator hafif tut.
        RuleFor(x => x.Dto.Credential)
            .NotEmpty().WithMessage("Credential is required.");

        // HashedPassword Domain guard: 8-256 char. Validator min-bound surfaces
        // client-friendly INVALID_REQUEST rather than USER_RULE_VIOLATION.
        // (RegisterWithPasswordValidator.cs:15-17 emsali, ayni mesaj.)
        RuleFor(x => x.Dto.NewPassword)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.");
    }
}
