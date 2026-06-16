using FastEndpoints;
using FluentValidation;

namespace Livestock.Features.ShippingZones;

public class CreateZoneValidator : Validator<CreateZoneRequest>
{
    public CreateZoneValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.CountryCodes).NotEmpty().MaximumLength(2000);
    }
}

public class UpdateZoneValidator : Validator<UpdateZoneRequest>
{
    public UpdateZoneValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.CountryCodes).NotEmpty().MaximumLength(2000);
    }
}

public class DeleteZoneValidator : Validator<DeleteZoneRequest>
{
    public DeleteZoneValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
