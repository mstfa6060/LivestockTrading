using FluentValidation;

namespace LivestockTrading.Identity.Application.Features.Auth;

public sealed class ForgotPasswordValidator : AbstractValidator<ForgotPasswordCommand>
{
    private static readonly string[] AllowedMethods = ["email", "phone"];

    public ForgotPasswordValidator()
    {
        RuleFor(x => x.Dto.Method)
            .NotEmpty().WithMessage("Method is required.")
            .Must(m => AllowedMethods.Contains(m))
            .WithMessage("Method must be 'email' or 'phone'.");

        RuleFor(x => x.Dto.Identifier)
            .NotEmpty().WithMessage("Identifier is required.");
        // Format kontrolu Domain EmailAddress/PhoneNumber ctor'unda (5 emsal handler bu idiom'da);
        // validator defansif NotEmpty ile sinirli.
    }
}
