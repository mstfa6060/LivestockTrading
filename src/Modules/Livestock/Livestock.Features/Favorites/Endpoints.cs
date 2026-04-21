using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;

namespace Livestock.Features.Favorites;

public class GetMyFavoritesEndpoint(LivestockDbContext db, IUserContext user) : EndpointWithoutRequest<List<FavoriteItem>>
{
    public override void Configure()
    {
        Get("/Favorites");
        Tags("Favorites");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var favorites = await db.FavoriteProducts
            .AsNoTracking()
            .Include(f => f.Product)
            .Where(f => f.UserId == user.UserId)
            .OrderByDescending(f => f.CreatedAt)
            .Select(f => new FavoriteItem(f.Id, f.ProductId, f.Product.Title, f.Product.Slug, f.Product.Price, f.Product.CurrencyCode, f.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(favorites, 200, ct);
    }
}

public class AddFavoriteEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<AddFavoriteRequest, FavoriteItem>
{
    public override void Configure()
    {
        Post("/Favorites");
        Tags("Favorites");
    }

    public override async Task HandleAsync(AddFavoriteRequest req, CancellationToken ct)
    {
        var product = await db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == req.ProductId, ct);
        if (product is null)
        {
            AddError(LivestockErrors.ProductErrors.ProductNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        var existing = await db.FavoriteProducts.FirstOrDefaultAsync(f => f.UserId == user.UserId && f.ProductId == req.ProductId, ct);
        if (existing is not null)
        {
            await SendAsync(new FavoriteItem(existing.Id, existing.ProductId, product.Title, product.Slug, product.Price, product.CurrencyCode, existing.CreatedAt), 200, ct);
            return;
        }

        var favorite = new FavoriteProduct { UserId = user.UserId, ProductId = req.ProductId };
        db.FavoriteProducts.Add(favorite);
        await db.SaveChangesAsync(ct);

        await SendAsync(new FavoriteItem(favorite.Id, favorite.ProductId, product.Title, product.Slug, product.Price, product.CurrencyCode, favorite.CreatedAt), 201, ct);
    }
}

public class RemoveFavoriteEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<RemoveFavoriteRequest, EmptyResponse>
{
    public override void Configure()
    {
        Delete("/Favorites/{ProductId}");
        Tags("Favorites");
    }

    public override async Task HandleAsync(RemoveFavoriteRequest req, CancellationToken ct)
    {
        var favorite = await db.FavoriteProducts.FirstOrDefaultAsync(f => f.UserId == user.UserId && f.ProductId == req.ProductId, ct);
        if (favorite is null)
        {
            await SendNoContentAsync(ct);
            return;
        }

        db.FavoriteProducts.Remove(favorite);
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}
