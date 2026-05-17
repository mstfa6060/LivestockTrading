using FluentValidation;

namespace LivestockTrading.Catalog.Application.Features.Brands;

public sealed class RejectBrandValidator : AbstractValidator<RejectBrandCommand>
{
    public RejectBrandValidator()
    {
        RuleFor(x => x.BrandId)
            .NotEqual(Guid.Empty).WithMessage("BrandId is required.");

        // Reason NotEmpty ↔ Wave 1 Domain Brand.Reject IsNullOrWhiteSpace(reason) guard.
        // MaximumLength 500 defansif (doc-literal kural yok — Application yapısal sınır, Wave 3 retro).
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Rejection reason is required.")
            .MaximumLength(500).WithMessage("Rejection reason must not exceed 500 characters.");

        RuleFor(x => x.ActorAdminId)
            .NotEqual(Guid.Empty).WithMessage("ActorAdminId is required.");
    }
}
