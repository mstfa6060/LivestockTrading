using FastEndpoints;
using FluentValidation;
using Livestock.Domain.Errors;

namespace Livestock.Features.ShippingRates;

public class CreateRateValidator : Validator<CreateRateRequest>
{
    public CreateRateValidator()
    {
        RuleFor(x => x.ShippingZoneId).NotEmpty();
        RuleFor(x => x.ShippingCost).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CurrencyCode).NotEmpty().Length(3, 10);
        RuleFor(x => x.EstimatedDeliveryDays).GreaterThan(0).When(x => x.EstimatedDeliveryDays.HasValue);
        RuleFor(x => x)
            .Must(WeightRangeValid)
            .WithMessage(LivestockErrors.ShippingErrors.RateInvalidWeightRange);
    }

    private static bool WeightRangeValid(CreateRateRequest req)
        => req.MinWeight is null || req.MaxWeight is null || req.MinWeight <= req.MaxWeight;
}

public class UpdateRateValidator : Validator<UpdateRateRequest>
{
    public UpdateRateValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ShippingZoneId).NotEmpty();
        RuleFor(x => x.ShippingCost).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CurrencyCode).NotEmpty().Length(3, 10);
        RuleFor(x => x.EstimatedDeliveryDays).GreaterThan(0).When(x => x.EstimatedDeliveryDays.HasValue);
        RuleFor(x => x)
            .Must(WeightRangeValid)
            .WithMessage(LivestockErrors.ShippingErrors.RateInvalidWeightRange);
    }

    private static bool WeightRangeValid(UpdateRateRequest req)
        => req.MinWeight is null || req.MaxWeight is null || req.MinWeight <= req.MaxWeight;
}

public class DeleteRateValidator : Validator<DeleteRateRequest>
{
    public DeleteRateValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
