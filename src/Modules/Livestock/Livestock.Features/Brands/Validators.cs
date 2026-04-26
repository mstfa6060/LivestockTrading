using FastEndpoints;
using FluentValidation;

namespace Livestock.Features.Brands;

public class CreateBrandValidator : Validator<CreateBrandRequest>
{
    public CreateBrandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(200).Matches(@"^[a-z0-9-]+$");
    }
}

public class UpdateBrandValidator : Validator<UpdateBrandRequest>
{
    public UpdateBrandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(200).Matches(@"^[a-z0-9-]+$");
    }
}
