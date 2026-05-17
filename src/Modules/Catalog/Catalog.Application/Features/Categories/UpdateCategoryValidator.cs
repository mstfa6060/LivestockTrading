using FluentValidation;

namespace LivestockTrading.Catalog.Application.Features.Categories;

public sealed class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryValidator()
    {
        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("CategoryId must be positive.");

        RuleFor(x => x.Dto.Name)
            .NotNull().WithMessage("Name translations object is required.");

        // 'en' locale presence ↔ Wave 1 Domain UpdateTranslations name.TryGet("en", out _) guard
        RuleFor(x => x.Dto.Name)
            .Must(name => name is not null && name.TryGet("en", out _))
            .WithMessage("Name must include 'en' locale.");

        RuleFor(x => x.Dto.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("DisplayOrder must be non-negative.");

        RuleFor(x => x.ActorAdminId)
            .NotEqual(Guid.Empty).WithMessage("ActorAdminId is required.");
    }
}
