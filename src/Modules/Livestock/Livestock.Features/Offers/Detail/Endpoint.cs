using FastEndpoints;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;

namespace Livestock.Features.Offers.Detail;

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
