using FluentValidation;
using Shared.Contracts.Identity;

namespace LivestockTrading.Identity.Application.Features.Auth;

public sealed class RegisterWithSocialValidator : AbstractValidator<RegisterWithSocialCommand>
{
    public RegisterWithSocialValidator()
    {
        RuleFor(x => x.Dto.Provider)
            .NotEmpty().WithMessage("Provider is required.")
            .Must(p => p is "google" or "apple")
            .WithMessage("Provider must be one of: google, apple.");

        RuleFor(x => x.Dto.IdToken)
            .NotEmpty().WithMessage("IdToken is required.");

        RuleFor(x => x.Dto.AccountType)
            .IsInEnum().WithMessage("AccountType must be a valid enum value.");

        RuleFor(x => x.Dto.Preferences)
            .NotNull().WithMessage("Preferences is required.");

        RuleFor(x => x.Dto.Consents)
            .NotNull().WithMessage("Consents is required.");

        // KVKK zorunlu consent kurali (plan-doc 05-identity §3 satir 273-274 doc-literal:
        // TermsAndPrivacy + MinistryDataShare register'da ZORUNLU). Password register
        // emsali simetrik — sosyal register de aynisini ister.
        RuleFor(x => x.Dto.Consents)
            .Must(consents => consents is not null
                && consents.Any(c => c.Type == ConsentType.TermsAndPrivacy && c.Granted)
                && consents.Any(c => c.Type == ConsentType.MinistryDataShare && c.Granted))
            .WithMessage("Zorunlu KVKK onaylari eksik: TermsAndPrivacy, MinistryDataShare.");

        RuleFor(x => x.Dto.DeviceFingerprint)
            .NotEmpty().WithMessage("DeviceFingerprint is required.");

        RuleFor(x => x.Dto.Platform)
            .NotEmpty().WithMessage("Platform is required.");
    }
}
