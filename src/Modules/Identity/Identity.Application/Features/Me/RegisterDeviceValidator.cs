using FluentValidation;

namespace LivestockTrading.Identity.Application.Features.Me;

public sealed class RegisterDeviceValidator : AbstractValidator<RegisterDeviceCommand>
{
    public RegisterDeviceValidator()
    {
        RuleFor(x => x.Dto.Platform)
            .NotEmpty().WithMessage("Platform is required.");

        RuleFor(x => x.Dto.DeviceFingerprint)
            .NotEmpty().WithMessage("DeviceFingerprint is required.");
    }
}
