using FastEndpoints;
using FluentValidation;

namespace Livestock.Features.Sellers;

public class BecomeSellerValidator : Validator<BecomeSellerRequest>
{
    public BecomeSellerValidator()
    {
        RuleFor(x => x.BusinessName).NotEmpty().MaximumLength(300);
        RuleFor(x => x.PhoneNumber).MaximumLength(30);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
    }
}

public class SuspendSellerValidator : Validator<SuspendSellerRequest>
{
    public SuspendSellerValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(1000);
    }
}

public class NearbySellersValidator : Validator<NearbySellersRequest>
{
    public NearbySellersValidator()
    {
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90);
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180);
        RuleFor(x => x.Limit).InclusiveBetween(1, 50);
        // Cap at 1000 km — anything wider is almost certainly a bug or a
        // global query that should hit /Sellers/All instead. Lower bound > 0
        // keeps ST_DWithin meaningful.
        RuleFor(x => x.RadiusKm).GreaterThan(0).LessThanOrEqualTo(1000);
        RuleFor(x => x.CountryCode).Length(2).When(x => !string.IsNullOrEmpty(x.CountryCode));
    }
}
