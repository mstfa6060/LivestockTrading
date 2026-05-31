using FluentValidation;

namespace LivestockTrading.Identity.Application.Features.Auth;

public sealed class SendPhoneOtpValidator : AbstractValidator<SendPhoneOtpCommand>
{
    public SendPhoneOtpValidator()
    {
        // Format kontrolu Domain PhoneNumber ctor'unda; validator defansif NotEmpty
        // (ForgotPasswordValidator Identifier hafif emsali).
        RuleFor(x => x.Dto.Phone)
            .NotEmpty().WithMessage("Phone is required.");
    }
}
