using FastEndpoints;
using FluentValidation;

namespace Iam.Features.Auth.Logout;

public sealed class LogoutValidator : Validator<LogoutRequest>
{
    public LogoutValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token zorunludur.");
    }
}
