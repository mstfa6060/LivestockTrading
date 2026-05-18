using FluentValidation;

namespace LivestockTrading.Catalog.Application.Features.Currencies;

public sealed class ToggleCurrencyActiveValidator : AbstractValidator<ToggleCurrencyActiveCommand>
{
    public ToggleCurrencyActiveValidator()
    {
        RuleFor(x => x.CurrencyId)
            .GreaterThan(0).WithMessage("CurrencyId must be positive.");

        RuleFor(x => x.ActorAdminId)
            .NotEqual(Guid.Empty).WithMessage("ActorAdminId is required.");
    }
}
