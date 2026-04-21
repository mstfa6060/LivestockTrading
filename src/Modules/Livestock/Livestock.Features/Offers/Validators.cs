using FastEndpoints;
using FluentValidation;

namespace Livestock.Features.Offers;

public class CreateOfferValidator : Validator<CreateOfferRequest>
{
    public CreateOfferValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.OfferedPrice).GreaterThan(0);
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(1);
        RuleFor(x => x.CurrencyCode).NotEmpty().MaximumLength(10);
    }
}

public class CounterOfferValidator : Validator<CounterOfferRequest>
{
    public CounterOfferValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.CounterPrice).GreaterThan(0);
    }
}
