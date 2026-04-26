using FastEndpoints;
using FluentValidation;

namespace Livestock.Features.Transport;

public class CreateTransportRequestValidator : Validator<CreateTransportRequestRequest>
{
    public CreateTransportRequestValidator()
    {
        RuleFor(x => x.PickupCountryCode).NotEmpty().MaximumLength(5);
        RuleFor(x => x.PickupCity).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DeliveryCountryCode).NotEmpty().MaximumLength(5);
        RuleFor(x => x.DeliveryCity).NotEmpty().MaximumLength(100);
    }
}

public class CreateTransportOfferValidator : Validator<CreateTransportOfferRequest>
{
    public CreateTransportOfferValidator()
    {
        RuleFor(x => x.TransportRequestId).NotEmpty();
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.CurrencyCode).NotEmpty().MaximumLength(10);
    }
}
