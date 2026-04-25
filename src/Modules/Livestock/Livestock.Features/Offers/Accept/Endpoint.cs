using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Enums;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;
using Shared.Contracts.Events.Livestock;
using Shared.Infrastructure.Messaging;

namespace Livestock.Features.Offers.Accept;

public class AcceptOfferEndpoint(LivestockDbContext db, IUserContext user, IEventPublisher publisher) : Endpoint<AcceptOfferRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/livestocktrading/Offers/Accept");
        Tags("Offers");
    }

    public override async Task HandleAsync(AcceptOfferRequest req, CancellationToken ct)
    {
        var offer = await db.Offers.Include(o => o.Product).FirstOrDefaultAsync(o => o.Id == req.Id, ct);
        if (offer is null)
        {
            AddError(LivestockErrors.OfferErrors.OfferNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (seller is null || offer.SellerId != seller.Id)
        {
            AddError(LivestockErrors.Common.Unauthorized);
            await SendErrorsAsync(403, ct);
            return;
        }

        if (offer.Status != OfferStatus.Pending && offer.Status != OfferStatus.Countered)
        {
            AddError(LivestockErrors.OfferErrors.OfferAlreadyProcessed);
            await SendErrorsAsync(409, ct);
            return;
        }

        offer.Status = OfferStatus.Accepted;
        offer.UpdatedAt = DateTime.UtcNow;

        var deal = new Deal
        {
            OfferId = offer.Id,
            ProductId = offer.ProductId,
            BuyerUserId = offer.BuyerUserId,
            SellerId = offer.SellerId,
            AgreePrice = offer.CounterPrice ?? offer.OfferedPrice,
            CurrencyCode = offer.CurrencyCode,
            Quantity = offer.Quantity,
            Status = DealStatus.Agreed
        };
        db.Deals.Add(deal);

        // Reduce stock; mark as out of stock when quantity reaches zero
        offer.Product.Quantity = Math.Max(0, offer.Product.Quantity - deal.Quantity);
        if (offer.Product.Quantity == 0)
        {
            offer.Product.Status = ProductStatus.OutOfStock;
        }
        offer.Product.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);

        await publisher.PublishAsync(OfferAcceptedEvent.Subject, new OfferAcceptedEvent
        {
            OfferId = offer.Id,
            DealId = deal.Id,
            ProductId = offer.ProductId,
            BuyerUserId = offer.BuyerUserId,
            SellerId = offer.SellerId,
            AgreePrice = deal.AgreePrice,
            CurrencyCode = deal.CurrencyCode
        }, ct);

        await SendNoContentAsync(ct);
    }
}
