using FluentValidation;

namespace LivestockTrading.Identity.Application.Features.AdminUsers;

public sealed class RevokeUserRoleValidator : AbstractValidator<RevokeUserRoleCommand>
{
    public RevokeUserRoleValidator()
    {
        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty).WithMessage("UserId is required.");

        // Role NotEmpty client-friendly INVALID_REQUEST; Domain RevokeRole
        // throw aktif-role-yok (USER_RULE_VIOLATION). Hardcoded role-list YOK
        // (KAYDET-19; RBAC plan-doc 05-identity:451-460 string-based).
        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required.");

        RuleFor(x => x.ActorAdminId)
            .NotEqual(Guid.Empty).WithMessage("ActorAdminId is required.");
    }
}
