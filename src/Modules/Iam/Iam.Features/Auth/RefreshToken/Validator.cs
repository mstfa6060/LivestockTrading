using FastEndpoints;
using FluentValidation;

namespace Iam.Features.Auth.RefreshToken;

public sealed class RefreshTokenValidator : Validator<RefreshTokenRequest>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Refresh token zorunludur.");
    }
}
