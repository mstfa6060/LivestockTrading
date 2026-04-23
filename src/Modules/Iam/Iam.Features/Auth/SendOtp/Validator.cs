using FastEndpoints;
using FluentValidation;

namespace Iam.Features.Auth.SendOtp;

public sealed class SendOtpValidator : Validator<SendOtpRequest>
{
    public SendOtpValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Telefon numarası zorunludur.")
            .Matches(@"^\+?[0-9]{7,15}$").WithMessage("Geçerli bir telefon numarası giriniz.");
    }
}
