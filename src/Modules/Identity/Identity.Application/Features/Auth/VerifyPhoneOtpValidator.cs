using FluentValidation;

namespace LivestockTrading.Identity.Application.Features.Auth;

public sealed class VerifyPhoneOtpValidator : AbstractValidator<VerifyPhoneOtpCommand>
{
    public VerifyPhoneOtpValidator()
    {
        RuleFor(x => x.Dto.Phone)
            .NotEmpty().WithMessage("Phone is required.");

        // Code length conditional (6-digit) handler/TryConsume tarafindan yakalanir;
        // validator hafif tut (ResetPasswordValidator Credential emsali).
        RuleFor(x => x.Dto.Code)
            .NotEmpty().WithMessage("Code is required.");
    }
}
