using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Enums;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;
using Shared.Contracts.Events.Livestock;
using Shared.Infrastructure.Messaging;

namespace Livestock.Features.Offers.Create;

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
