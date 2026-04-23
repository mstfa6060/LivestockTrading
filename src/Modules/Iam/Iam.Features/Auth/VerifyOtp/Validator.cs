using FastEndpoints;
using FluentValidation;

namespace Iam.Features.Auth.VerifyOtp;

public sealed class VerifyOtpValidator : Validator<VerifyOtpRequest>
{
    public VerifyOtpValidator()
    {
        RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Telefon numarası zorunludur.");
        RuleFor(x => x.OtpCode)
            .NotEmpty().WithMessage("OTP kodu zorunludur.")
            .Length(6).WithMessage("OTP kodu 6 haneli olmalıdır.")
            .Matches("^[0-9]+$").WithMessage("OTP kodu sadece rakamlardan oluşmalıdır.");
    }
}
