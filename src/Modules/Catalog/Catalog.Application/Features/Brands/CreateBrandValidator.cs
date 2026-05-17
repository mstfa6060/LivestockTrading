using FluentValidation;

namespace LivestockTrading.Catalog.Application.Features.Brands;

public sealed class CreateBrandValidator : AbstractValidator<CreateBrandCommand>
{
    public CreateBrandValidator()
    {
        RuleFor(x => x.Dto.Slug)
            .NotEmpty().WithMessage("Slug is required.");

        RuleFor(x => x.Dto.Name)
            .NotNull().WithMessage("Name translations object is required.");

        // 'en' locale presence ↔ Wave 1 Domain name.TryGet("en", out _) guard
        RuleFor(x => x.Dto.Name)
            .Must(name => name is not null && name.TryGet("en", out _))
            .WithMessage("Name must include 'en' locale.");

        RuleFor(x => x.Dto.CategoryCodes)
            .NotNull().WithMessage("CategoryCodes is required.");

        RuleFor(x => x.Dto.CategoryCodes)
            .Must(codes => codes is not null && codes.Count > 0)
            .WithMessage("At least one CategoryCode is required.");

        RuleForEach(x => x.Dto.CategoryCodes)
            .NotEmpty().WithMessage("CategoryCode must not be empty.");

        RuleFor(x => x.Dto.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("DisplayOrder must be non-negative.");

        RuleFor(x => x.ActorAdminId)
            .NotEqual(Guid.Empty).WithMessage("ActorAdminId is required.");
    }
}
