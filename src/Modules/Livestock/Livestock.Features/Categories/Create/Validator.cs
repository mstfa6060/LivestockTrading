using FastEndpoints;
using FluentValidation;

namespace Livestock.Features.Categories.Create;

public class CreateCategoryValidator : Validator<CreateCategoryRequest>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(200).Matches(@"^[a-z0-9-]+$");
    }
}
