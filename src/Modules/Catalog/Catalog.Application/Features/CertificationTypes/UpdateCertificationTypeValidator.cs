using FluentValidation;

namespace LivestockTrading.Catalog.Application.Features.CertificationTypes;

public sealed class UpdateCertificationTypeValidator : AbstractValidator<UpdateCertificationTypeCommand>
{
    public UpdateCertificationTypeValidator()
    {
        RuleFor(x => x.CertificationTypeId)
            .GreaterThan(0).WithMessage("CertificationTypeId must be positive.");

        RuleFor(x => x.Dto.NameTranslations)
            .NotNull().WithMessage("NameTranslations object is required.");

        // 'en' locale presence ↔ Wave 1 Domain Translations en-locale guard
        RuleFor(x => x.Dto.NameTranslations)
            .Must(name => name is not null && name.TryGet("en", out _))
            .WithMessage("NameTranslations must include 'en' locale.");

        RuleFor(x => x.Dto.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("DisplayOrder must be non-negative.");

        RuleFor(x => x.ActorAdminId)
            .NotEqual(Guid.Empty).WithMessage("ActorAdminId is required.");
    }
}
