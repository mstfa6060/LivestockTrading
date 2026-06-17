using FastEndpoints;
using FluentValidation;

namespace Livestock.Features.ShippingCarriers;

public class CreateCarrierValidator : Validator<CreateCarrierRequest>
{
    public CreateCarrierValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Website).MaximumLength(500);
        RuleFor(x => x.TrackingUrlTemplate).MaximumLength(1000);
        RuleFor(x => x.SupportedCountries).MaximumLength(2000);
    }
}

public class UpdateCarrierValidator : Validator<UpdateCarrierRequest>
{
    public UpdateCarrierValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Website).MaximumLength(500);
        RuleFor(x => x.TrackingUrlTemplate).MaximumLength(1000);
        RuleFor(x => x.SupportedCountries).MaximumLength(2000);
    }
}

public class DeleteCarrierValidator : Validator<DeleteCarrierRequest>
{
    public DeleteCarrierValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
