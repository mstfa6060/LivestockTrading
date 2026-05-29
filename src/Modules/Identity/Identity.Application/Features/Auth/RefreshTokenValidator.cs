using FluentValidation;

namespace LivestockTrading.Identity.Application.Features.Auth;

public sealed class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.Dto.RefreshToken)
            .NotEmpty().WithMessage("RefreshToken is required.");
    }
}
