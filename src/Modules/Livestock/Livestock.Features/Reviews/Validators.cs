using FastEndpoints;
using FluentValidation;

namespace Livestock.Features.Reviews;

public class CreateProductReviewValidator : Validator<CreateProductReviewRequest>
{
    public CreateProductReviewValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
    }
}

public class CreateSellerReviewValidator : Validator<CreateSellerReviewRequest>
{
    public CreateSellerReviewValidator()
    {
        RuleFor(x => x.SellerId).NotEmpty();
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
    }
}

public class CreateTransporterReviewValidator : Validator<CreateTransporterReviewRequest>
{
    public CreateTransporterReviewValidator()
    {
        RuleFor(x => x.TransporterId).NotEmpty();
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
    }
}
