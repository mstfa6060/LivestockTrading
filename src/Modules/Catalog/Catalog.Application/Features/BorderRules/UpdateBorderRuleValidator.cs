using FluentValidation;

namespace LivestockTrading.Catalog.Application.Features.BorderRules;

public sealed class UpdateBorderRuleValidator : AbstractValidator<UpdateBorderRuleCommand>
{
    public UpdateBorderRuleValidator()
    {
        RuleFor(x => x.BorderRuleId)
            .NotEqual(Guid.Empty).WithMessage("BorderRuleId is required.");

        RuleFor(x => x.Dto.Kind)
            .IsInEnum().WithMessage("Kind must be a valid BorderRuleKind.");

        RuleFor(x => x.Dto.RestrictionsJson)
            .NotEmpty().WithMessage("RestrictionsJson is required.");

        RuleFor(x => x.Dto.Notes)
            .NotNull().WithMessage("Notes translations object is required.");

        // 'en' locale presence ↔ Wave 1 Domain Translations en-locale guard
        RuleFor(x => x.Dto.Notes)
            .Must(notes => notes is not null && notes.TryGet("en", out _))
            .WithMessage("Notes must include 'en' locale.");

        RuleFor(x => x.ActorAdminId)
            .NotEqual(Guid.Empty).WithMessage("ActorAdminId is required.");
    }
}
