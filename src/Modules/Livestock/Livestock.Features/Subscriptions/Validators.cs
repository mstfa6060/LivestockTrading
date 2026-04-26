using FluentValidation;
using Livestock.Domain.Errors;

namespace Livestock.Features.Subscriptions;

public class SubscribeValidator : AbstractValidator<SubscribeRequest>
{
    public SubscribeValidator()
    {
        RuleFor(x => x.PlanId).NotEmpty();
        RuleFor(x => x.Receipt)
            .NotEmpty()
            .WithMessage(LivestockErrors.SubscriptionErrors.SubscriptionReceiptRequired);
        RuleFor(x => x.StoreTransactionId)
            .NotEmpty()
            .WithMessage(LivestockErrors.SubscriptionErrors.SubscriptionStoreTransactionIdRequired);
    }
}
