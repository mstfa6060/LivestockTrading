using FluentValidation;

namespace LivestockTrading.Identity.Application.Features.AdminUsers;

public sealed class GrantUserRoleValidator : AbstractValidator<GrantUserRoleCommand>
{
    public GrantUserRoleValidator()
    {
        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty).WithMessage("UserId is required.");

        // Role NotEmpty <-> Domain User.GrantRole IsNullOrWhiteSpace(role) guard.
        // Allowed-role enum check Domain'de yok (string, RBAC 6 role plan-doc 05-identity:451-460);
        // KAYDET-19 magic-string YASAK — Application'da hardcoded role-list YOK.
        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required.");

        RuleFor(x => x.ActorAdminId)
            .NotEqual(Guid.Empty).WithMessage("ActorAdminId is required.");
    }
}
