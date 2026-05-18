using FluentValidation;

namespace LivestockTrading.Catalog.Application.Features.Countries;

public sealed class ToggleCountryActiveValidator : AbstractValidator<ToggleCountryActiveCommand>
{
    public ToggleCountryActiveValidator()
    {
        RuleFor(x => x.CountryId)
            .GreaterThan(0).WithMessage("CountryId must be positive.");

        RuleFor(x => x.ActorAdminId)
            .NotEqual(Guid.Empty).WithMessage("ActorAdminId is required.");
    }
}
