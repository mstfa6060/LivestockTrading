using FastEndpoints;
using FluentValidation;

namespace Iam.Features.Auth.SendEmailOtp;

public sealed class SendEmailOtpValidator : Validator<SendEmailOtpRequest>
{
    public SendEmailOtpValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta zorunludur.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");
    }
}
