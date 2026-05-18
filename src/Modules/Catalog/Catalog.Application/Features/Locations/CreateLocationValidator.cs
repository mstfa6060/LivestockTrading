using FluentValidation;

namespace LivestockTrading.Catalog.Application.Features.Locations;

public sealed class CreateLocationValidator : AbstractValidator<CreateLocationCommand>
{
    public CreateLocationValidator()
    {
        RuleFor(x => x.Dto.Code)
            .NotEmpty().WithMessage("Code is required.");

        RuleFor(x => x.Dto.Level)
            .IsInEnum().WithMessage("Level must be a valid LocationLevel.");

        RuleFor(x => x.Dto.CountryCode)
            .NotEmpty().WithMessage("CountryCode is required.");

        RuleFor(x => x.Dto.Name)
            .NotNull().WithMessage("Name translations object is required.");

        // 'en' locale presence ↔ Wave 1 Domain Translations en-locale guard
        RuleFor(x => x.Dto.Name)
            .Must(name => name is not null && name.TryGet("en", out _))
            .WithMessage("Name must include 'en' locale.");

        RuleFor(x => x.Dto.Population)
            .GreaterThanOrEqualTo(0).WithMessage("Population must be non-negative.");

        RuleFor(x => x.Dto.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("DisplayOrder must be non-negative.");

        RuleFor(x => x.Dto.ParentId)
            .GreaterThan(0).When(x => x.Dto.ParentId.HasValue)
            .WithMessage("ParentId must be positive when provided.");

        RuleFor(x => x.ActorAdminId)
            .NotEqual(Guid.Empty).WithMessage("ActorAdminId is required.");
    }
}
