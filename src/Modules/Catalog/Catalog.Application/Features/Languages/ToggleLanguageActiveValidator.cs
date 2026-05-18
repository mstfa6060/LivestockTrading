using FluentValidation;

namespace LivestockTrading.Catalog.Application.Features.Languages;

public sealed class ToggleLanguageActiveValidator : AbstractValidator<ToggleLanguageActiveCommand>
{
    public ToggleLanguageActiveValidator()
    {
        RuleFor(x => x.LanguageId)
            .GreaterThan(0).WithMessage("LanguageId must be positive.");

        RuleFor(x => x.ActorAdminId)
            .NotEqual(Guid.Empty).WithMessage("ActorAdminId is required.");
    }
}
