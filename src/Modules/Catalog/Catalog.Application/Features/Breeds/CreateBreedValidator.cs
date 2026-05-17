using FluentValidation;

namespace LivestockTrading.Catalog.Application.Features.Breeds;

public sealed class CreateBreedValidator : AbstractValidator<CreateBreedCommand>
{
    public CreateBreedValidator()
    {
        // NotEmpty ↔ Wave 1 Domain Breed.Create IsNullOrWhiteSpace(code) guard
        RuleFor(x => x.Dto.Code)
            .NotEmpty().WithMessage("Code is required.");

        RuleFor(x => x.Dto.CategoryCode)
            .NotEmpty().WithMessage("CategoryCode is required.");

        RuleFor(x => x.Dto.Name)
            .NotNull().WithMessage("Name translations object is required.");

        // 'en' locale presence ↔ Wave 1 Domain name.TryGet("en", out _) guard
        RuleFor(x => x.Dto.Name)
            .Must(name => name is not null && name.TryGet("en", out _))
            .WithMessage("Name must include 'en' locale.");

        RuleFor(x => x.Dto.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("DisplayOrder must be non-negative.");

        RuleFor(x => x.ActorAdminId)
            .NotEqual(Guid.Empty).WithMessage("ActorAdminId is required.");
    }
}
