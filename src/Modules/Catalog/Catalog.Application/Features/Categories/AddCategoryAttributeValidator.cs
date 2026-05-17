using FluentValidation;
using Shared.Contracts.Catalog;

namespace LivestockTrading.Catalog.Application.Features.Categories;

public sealed class AddCategoryAttributeValidator : AbstractValidator<AddCategoryAttributeCommand>
{
    public AddCategoryAttributeValidator()
    {
        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("CategoryId must be positive.");

        // NotEmpty ↔ Wave 1 Domain Category.AddAttribute IsNullOrWhiteSpace(key) guard
        RuleFor(x => x.Dto.Key)
            .NotEmpty().WithMessage("Key is required.");

        RuleFor(x => x.Dto.ValueType)
            .IsInEnum().WithMessage("ValueType must be a valid AttributeValueType.");

        // Conditional ↔ Wave 1 Domain: ValueType == Enum → optionsJson required
        RuleFor(x => x.Dto.OptionsJson)
            .NotEmpty()
            .When(x => x.Dto.ValueType == AttributeValueType.Enum)
            .WithMessage("OptionsJson is required when ValueType is Enum.");

        RuleFor(x => x.Dto.Label)
            .NotNull().WithMessage("Label translations object is required.");

        // 'en' locale ↔ Wave 1 Domain Category.AddAttribute label.TryGet("en", out _) guard
        RuleFor(x => x.Dto.Label)
            .Must(label => label is not null && label.TryGet("en", out _))
            .WithMessage("Label must include 'en' locale.");

        RuleFor(x => x.Dto.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("DisplayOrder must be non-negative.");

        RuleFor(x => x.ActorAdminId)
            .NotEqual(Guid.Empty).WithMessage("ActorAdminId is required.");
    }
}
