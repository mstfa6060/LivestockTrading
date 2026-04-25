using FastEndpoints;
using FluentValidation;

namespace Livestock.Features.Products.Reject;

public class RejectProductValidator : Validator<RejectProductRequest>
{
    public RejectProductValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(1000);
    }
}
