using FastEndpoints;
using FluentValidation;

namespace Livestock.Features.Reviews;

public class CreateProductReviewValidator : Validator<CreateProductReviewRequest>
{
    public CreateProductReviewValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
        RuleFor(x => x.Comment).MaximumLength(2000);
    }
}

public class CreateSellerReviewValidator : Validator<CreateSellerReviewRequest>
{
    public CreateSellerReviewValidator()
    {
        RuleFor(x => x.SellerId).NotEmpty();
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
        RuleFor(x => x.Comment).MaximumLength(2000);
    }
}

public class CreateTransporterReviewValidator : Validator<CreateTransporterReviewRequest>
{
    public CreateTransporterReviewValidator()
    {
        RuleFor(x => x.TransporterId).NotEmpty();
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
        RuleFor(x => x.Comment).MaximumLength(2000);
    }
}

public class UpdateProductReviewValidator : Validator<UpdateProductReviewRequest>
{
    public UpdateProductReviewValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
        RuleFor(x => x.Comment).MaximumLength(2000);
    }
}

public class UpdateSellerReviewValidator : Validator<UpdateSellerReviewRequest>
{
    public UpdateSellerReviewValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
        RuleFor(x => x.Comment).MaximumLength(2000);
    }
}

public class UpdateTransporterReviewValidator : Validator<UpdateTransporterReviewRequest>
{
    public UpdateTransporterReviewValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
        RuleFor(x => x.Comment).MaximumLength(2000);
    }
}

public class DeleteReviewValidator : Validator<DeleteReviewRequest>
{
    public DeleteReviewValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
