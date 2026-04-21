using FastEndpoints;
using FluentValidation;

namespace Livestock.Features.Products;

public class CreateProductValidator : Validator<CreateProductRequest>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(500);
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(1);
        RuleFor(x => x.CurrencyCode).NotEmpty().MaximumLength(10);
    }
}

public class UpdateProductValidator : Validator<UpdateProductRequest>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(500);
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(1);
        RuleFor(x => x.CurrencyCode).NotEmpty().MaximumLength(10);
    }
}

public class RejectProductValidator : Validator<RejectProductRequest>
{
    public RejectProductValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(1000);
    }
}
