using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Enums;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;
using Shared.Contracts.Events.Livestock;
using Shared.Infrastructure.Messaging;

namespace Livestock.Features.Offers;

public class GetMyOffersEndpoint(LivestockDbContext db, IUserContext user) : EndpointWithoutRequest<List<OfferListItem>>
{
    public override void Configure()
    {
        Post("/livestocktrading/Offers/My");
        Tags("Offers");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var offers = await db.Offers
            .AsNoTracking()
            .Include(o => o.Product)
            .Where(o => o.BuyerUserId == user.UserId || o.SellerId == (db.Sellers.AsNoTracking().Where(s => s.UserId == user.UserId).Select(s => s.Id).FirstOrDefault()))
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OfferListItem(o.Id, o.ProductId, o.Product.Title, o.BuyerUserId, o.SellerId, o.OfferedPrice, o.CurrencyCode, o.Quantity, o.Status, o.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(offers, 200, ct);
    }
}

public class GetOfferEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<GetOfferRequest, OfferDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/Offers/Detail");
        Tags("Offers");
    }

    public override async Task HandleAsync(GetOfferRequest req, CancellationToken ct)
    {
        var offer = await db.Offers.AsNoTracking().Include(o => o.Product).FirstOrDefaultAsync(o => o.Id == req.Id, ct);
        if (offer is null)
        {
            AddError(LivestockErrors.OfferErrors.OfferNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        var seller = await db.Sellers.AsNoTracking().FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (offer.BuyerUserId != user.UserId && (seller is null || offer.SellerId != seller.Id))
        {
            AddError(LivestockErrors.Common.Unauthorized);
            await SendErrorsAsync(403, ct);
            return;
        }

        await SendAsync(new OfferDetail(offer.Id, offer.ProductId, offer.Product.Title, offer.BuyerUserId, offer.SellerId, offer.OfferedPrice, offer.CurrencyCode, offer.Quantity, offer.Note, offer.Status, offer.CounterPrice, offer.CounterNote, offer.ExpiresAt, offer.CreatedAt), 200, ct);
    }
}

public class CreateOfferEndpoint(LivestockDbContext db, IUserContext user, IEventPublisher publisher) : Endpoint<CreateOfferRequest, OfferDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/Offers/Create");
        Tags("Offers");
    }

    public override async Task HandleAsync(CreateOfferRequest req, CancellationToken ct)
    {
        var product = await db.Products.Include(p => p.Seller).FirstOrDefaultAsync(p => p.Id == req.ProductId, ct);
        if (product is null || product.Status != ProductStatus.Active)
        {
            AddError(LivestockErrors.ProductErrors.ProductNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        var offer = new Offer
        {
            ProductId = req.ProductId,
            BuyerUserId = user.UserId,
            SellerId = product.SellerId,
            OfferedPrice = req.OfferedPrice,
            CurrencyCode = req.CurrencyCode,
            Quantity = req.Quantity,
            Note = req.Note,
            Status = OfferStatus.Pending,
            ExpiresAt = DateTime.UtcNow.AddDays(3)
        };

        db.Offers.Add(offer);
        await db.SaveChangesAsync(ct);

        await publisher.PublishAsync(OfferCreatedEvent.Subject, new OfferCreatedEvent
        {
            OfferId = offer.Id,
            ProductId = offer.ProductId,
            BuyerUserId = offer.BuyerUserId,
            SellerId = offer.SellerId,
            OfferedPrice = offer.OfferedPrice,
            CurrencyCode = offer.CurrencyCode,
            Quantity = offer.Quantity
        }, ct);

        await SendAsync(new OfferDetail(offer.Id, offer.ProductId, product.Title, offer.BuyerUserId, offer.SellerId, offer.OfferedPrice, offer.CurrencyCode, offer.Quantity, offer.Note, offer.Status, null, null, offer.ExpiresAt, offer.CreatedAt), 201, ct);
    }
}

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

public class RejectOfferEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<RejectOfferRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/livestocktrading/Offers/Reject");
        Tags("Offers");
    }

    public override async Task HandleAsync(RejectOfferRequest req, CancellationToken ct)
    {
        var offer = await db.Offers.FirstOrDefaultAsync(o => o.Id == req.Id, ct);
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

        if (offer.Status != OfferStatus.Pending)
        {
            AddError(LivestockErrors.OfferErrors.OfferAlreadyProcessed);
            await SendErrorsAsync(409, ct);
            return;
        }

        offer.Status = OfferStatus.Rejected;
        offer.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}
