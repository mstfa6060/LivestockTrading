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
