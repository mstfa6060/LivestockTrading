using FluentValidation;
using Shared.Contracts.Identity;

namespace LivestockTrading.Identity.Application.Features.Me;

public sealed class UpdateConsentsValidator : AbstractValidator<UpdateConsentsCommand>
{
    public UpdateConsentsValidator()
    {
        RuleFor(x => x.Dto.Changes)
            .NotEmpty().WithMessage("Consent changes list must not be empty.");

        // Zorunlu KVKK consent geri cekme korumasi (plan-doc 05-identity §3 satir
        // 273-274 — TermsAndPrivacy + MinistryDataShare register'da zorunlu, ayni
        // invariant ongoing toggle akisinda da gecerli). Domain RevokeConsent
        // guard'siz oldugundan koruma Application katmaninda.
        RuleForEach(x => x.Dto.Changes)
            .Must(c => !((c.Type == ConsentType.TermsAndPrivacy || c.Type == ConsentType.MinistryDataShare) && !c.Granted))
            .WithMessage("Zorunlu KVKK onaylari geri cekilemez: TermsAndPrivacy, MinistryDataShare.");
    }
}
