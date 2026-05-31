using FluentValidation;

namespace LivestockTrading.Identity.Application.Features.AdminUsers;

public sealed class SuspendUserValidator : AbstractValidator<SuspendUserCommand>
{
    public SuspendUserValidator()
    {
        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty).WithMessage("UserId is required.");

        // Reason NotEmpty <-> Domain User.Suspend IsNullOrWhiteSpace(reason) guard.
        // MaximumLength YOK — Domain const tanimsiz, KAYDET-19 hardcode YASAK.
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Suspension reason is required.");

        RuleFor(x => x.ActorAdminId)
            .NotEqual(Guid.Empty).WithMessage("ActorAdminId is required.");
    }
}
