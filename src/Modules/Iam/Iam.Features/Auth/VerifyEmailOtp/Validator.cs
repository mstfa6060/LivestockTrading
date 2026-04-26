using FastEndpoints;
using FluentValidation;

namespace Iam.Features.Auth.VerifyEmailOtp;

public sealed class VerifyEmailOtpValidator : Validator<VerifyEmailOtpRequest>
{
    public VerifyEmailOtpValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta zorunludur.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");
        RuleFor(x => x.OtpCode)
            .NotEmpty().WithMessage("OTP kodu zorunludur.")
            .Length(6).WithMessage("OTP kodu 6 haneli olmalıdır.")
            .Matches("^[0-9]+$").WithMessage("OTP kodu sadece rakamlardan oluşmalıdır.");
    }
}
