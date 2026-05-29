using FluentValidation;

namespace LivestockTrading.Identity.Application.Features.Me;

public sealed class LinkExternalLoginValidator : AbstractValidator<LinkExternalLoginCommand>
{
    public LinkExternalLoginValidator()
    {
        RuleFor(x => x.Dto.Provider)
            .NotEmpty().WithMessage("Provider is required.")
            .Must(p => p is "google" or "apple")
            .WithMessage("Provider must be one of: google, apple.");

        RuleFor(x => x.Dto.IdToken)
            .NotEmpty().WithMessage("IdToken is required.");
    }
}
