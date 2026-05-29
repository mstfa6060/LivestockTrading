using FluentValidation;

namespace LivestockTrading.Identity.Application.Features.Me;

public sealed class UpdateProfileValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileValidator()
    {
        RuleFor(x => x.Dto.FirstName)
            .NotEmpty().WithMessage("FirstName is required.");

        RuleFor(x => x.Dto.LastName)
            .NotEmpty().WithMessage("LastName is required.");

        // NationalId format (11 digits + TC checksum) handled by Domain VO ctor;
        // null is valid here and signals "leave unchanged".
    }
}
