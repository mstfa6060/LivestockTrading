using FastEndpoints;
using FluentValidation;

namespace Livestock.Features.Products;

public class CreateProductValidator : Validator<CreateProductRequest>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(500);
        RuleFor(x => x.CategoryId).NotEmpty();

        // Accept either the canonical field (Price/Quantity/CurrencyCode) OR
        // the legacy frontend alias (BasePrice/StockQuantity/Currency). The
        // handler coalesces them; the validator has to recognise both paths
        // or it blocks the request before the handler ever runs.
        RuleFor(x => x)
            .Must(x => x.Price > 0 || (x.BasePrice ?? 0m) > 0)
            .WithName("price")
            .WithMessage("'price' must be greater than '0'.");

        RuleFor(x => x)
            .Must(x => x.Quantity >= 1 || (x.StockQuantity ?? 0) >= 1)
            .WithName("quantity")
            .WithMessage("'quantity' must be greater than or equal to '1'.");

        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.CurrencyCode) || !string.IsNullOrWhiteSpace(x.Currency))
            .WithName("currencyCode")
            .WithMessage("'currency code' must not be empty.");

        When(x => !string.IsNullOrEmpty(x.CurrencyCode),
            () => RuleFor(x => x.CurrencyCode!).MaximumLength(10));
        When(x => !string.IsNullOrEmpty(x.Currency),
            () => RuleFor(x => x.Currency!).MaximumLength(10));
    }
}

public class UpdateProductValidator : Validator<UpdateProductRequest>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(500);
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(1);
        RuleFor(x => x.CurrencyCode).NotEmpty().MaximumLength(10);
    }
}

public class RejectProductValidator : Validator<RejectProductRequest>
{
    public RejectProductValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(1000);
    }
}
