using FastEndpoints;
using FluentValidation;

namespace Livestock.Features.Farms;

public class CreateFarmValidator : Validator<CreateFarmRequest>
{
    public CreateFarmValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(300);
        RuleFor(x => x.PhoneNumber).MaximumLength(30).When(x => x.PhoneNumber != null);
    }
}

public class UpdateFarmValidator : Validator<UpdateFarmRequest>
{
    public UpdateFarmValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(300);
        RuleFor(x => x.PhoneNumber).MaximumLength(30).When(x => x.PhoneNumber != null);
    }
}
