using FastEndpoints;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Livestock.Features.Products.Detail;

public class GetProductEndpoint(LivestockDbContext db) : Endpoint<GetProductRequest, ProductDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/Products/Detail");
        AllowAnonymous();
        Tags("Products");
    }

    public override async Task HandleAsync(GetProductRequest req, CancellationToken ct)
    {
        var p = await db.Products
            .AsNoTracking()
            .Include(x => x.Seller)
            .Include(x => x.Category)
            .Include(x => x.Brand)
            .Include(x => x.Farm)
            .Include(x => x.Location)
            .FirstOrDefaultAsync(x => x.Id == req.Id, ct);

        if (p is null)
        {
            AddError(LivestockErrors.ProductErrors.ProductNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        await SendAsync(new ProductDetail(
            p.Id, p.Title, p.Slug, p.Description, p.Price, p.CurrencyCode, p.Quantity, p.Unit,
            p.Status, p.Condition, p.IsNegotiable, p.IsFeatured,
            p.SellerId, p.Seller.BusinessName,
            p.CategoryId, p.Category.Name,
            p.BrandId, p.Brand?.Name,
            p.FarmId, p.Farm?.Name,
            p.LocationId, p.Location?.CountryCode, p.Location?.City,
            p.Location?.Latitude, p.Location?.Longitude,
            p.AverageRating, p.ReviewCount, p.ViewCount, p.PublishedAt, p.CreatedAt,
            p.BucketId), 200, ct);
    }
}
