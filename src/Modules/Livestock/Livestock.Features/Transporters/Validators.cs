using FastEndpoints;
using FluentValidation;

namespace Livestock.Features.Transporters;

public class BecomeTransporterValidator : Validator<BecomeTransporterRequest>
{
    public BecomeTransporterValidator()
    {
        RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(300);
        RuleFor(x => x.PhoneNumber).MaximumLength(30).When(x => x.PhoneNumber != null);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
    }
}

public class SuspendTransporterValidator : Validator<SuspendTransporterRequest>
{
    public SuspendTransporterValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(1000);
    }
}
