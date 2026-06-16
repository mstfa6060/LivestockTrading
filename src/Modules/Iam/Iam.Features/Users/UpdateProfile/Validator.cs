using FastEndpoints;
using FluentValidation;

namespace Iam.Features.Users.UpdateProfile;

public sealed class UpdateProfileValidator : Validator<UpdateProfileRequest>
{
    public UpdateProfileValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Ad zorunludur.")
            .MaximumLength(100);

        RuleFor(x => x.Surname)
            .NotEmpty().WithMessage("Soyad zorunludur.")
            .MaximumLength(100);
    }
}
