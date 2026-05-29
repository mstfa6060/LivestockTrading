using FluentValidation;

namespace LivestockTrading.Identity.Application.Features.Auth;

public sealed class SendEmailVerifyValidator : AbstractValidator<SendEmailVerifyCommand>
{
    public SendEmailVerifyValidator()
    {
        RuleFor(x => x.Dto.Email)
            .NotEmpty().WithMessage("Email is required.");
    }
}
