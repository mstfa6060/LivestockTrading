using FluentValidation;
using Shared.Contracts.Identity;

namespace LivestockTrading.Identity.Application.Features.Auth;

public sealed class RegisterWithPasswordValidator : AbstractValidator<RegisterWithPasswordCommand>
{
    public RegisterWithPasswordValidator()
    {
        RuleFor(x => x.Dto.Email)
            .NotEmpty().WithMessage("Email is required.");

        // HashedPassword Domain guard: 8-256 char. Validator min-bound surfaces
        // client-friendly INVALID_REQUEST rather than USER_RULE_VIOLATION.
        RuleFor(x => x.Dto.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.");

        RuleFor(x => x.Dto.FirstName)
            .NotEmpty().WithMessage("FirstName is required.");

        RuleFor(x => x.Dto.LastName)
            .NotEmpty().WithMessage("LastName is required.");

        RuleFor(x => x.Dto.AccountType)
            .IsInEnum().WithMessage("AccountType must be a valid enum value.");

        RuleFor(x => x.Dto.Preferences)
            .NotNull().WithMessage("Preferences is required.");

        RuleFor(x => x.Dto.Consents)
            .NotNull().WithMessage("Consents is required.");

        // KVKK zorunlu consent kurali (plan-doc 05-identity §3 satir 273-274 doc-literal:
        // TermsAndPrivacy + MinistryDataShare register'da ZORUNLU). MarketingEmail opsiyonel.
        // Granted == true kontrolu dahil — consent kaydi var ama granted=false ise gecersiz.
        RuleFor(x => x.Dto.Consents)
            .Must(consents => consents is not null
                && consents.Any(c => c.Type == ConsentType.TermsAndPrivacy && c.Granted)
                && consents.Any(c => c.Type == ConsentType.MinistryDataShare && c.Granted))
            .WithMessage("Zorunlu KVKK onaylari eksik: TermsAndPrivacy, MinistryDataShare.");
    }
}
