using FluentValidation;

namespace Iam.Features.Push;

public class RegisterPushTokenValidator : AbstractValidator<RegisterPushTokenRequest>
{
    public RegisterPushTokenValidator()
    {
        RuleFor(x => x.PushToken).NotEmpty().MaximumLength(1024);
        RuleFor(x => x.DeviceId).NotEmpty().MaximumLength(200);
        RuleFor(x => x.AppName).NotEmpty().MaximumLength(100);
    }
}

public class RevokePushTokenValidator : AbstractValidator<RevokePushTokenRequest>
{
    public RevokePushTokenValidator()
    {
        RuleFor(x => x.DeviceId).NotEmpty().MaximumLength(200);
    }
}
