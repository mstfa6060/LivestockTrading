using FluentValidation;

namespace LivestockTrading.Identity.Application.Features.Me;

public sealed class RequestEmailChangeValidator : AbstractValidator<RequestEmailChangeCommand>
{
    public RequestEmailChangeValidator()
    {
        // Format kontrolu Domain EmailAddress ctor'unda (5 emsal handler ile birebir);
        // validator defensive NotEmpty ile sinirli.
        RuleFor(x => x.Dto.NewEmail)
            .NotEmpty().WithMessage("NewEmail is required.");
    }
}
