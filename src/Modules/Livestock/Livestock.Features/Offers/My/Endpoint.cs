using FastEndpoints;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;

namespace Livestock.Features.Offers.My;

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
