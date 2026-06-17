using FastEndpoints;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;

namespace Livestock.Features.Products.Update;

public class UpdateProductEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<UpdateProductRequest, ProductDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/Products/Update");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("Products");
    }

    public override async Task HandleAsync(UpdateProductRequest req, CancellationToken ct)
    {
        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        var product = await db.Products.Include(p => p.Seller).Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == req.Id, ct);

        if (product is null)
        {
            AddError(LivestockErrors.ProductErrors.ProductNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        if (seller is null || product.SellerId != seller.Id)
        {
            AddError(LivestockErrors.ProductErrors.ProductNotOwnedBySeller);
            await SendErrorsAsync(403, ct);
            return;
        }

        product.CategoryId = req.CategoryId;
        product.BrandId = req.BrandId;
        product.FarmId = req.FarmId;
        product.Title = req.Title;
        product.Slug = req.Slug;
        product.Description = req.Description;
        product.Price = req.Price;
        product.CurrencyCode = req.CurrencyCode;
        product.Quantity = req.Quantity;
        product.Unit = req.Unit;
        product.Condition = req.Condition;
        product.IsNegotiable = req.IsNegotiable;
        product.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        await SendAsync(new ProductDetail(
            product.Id, product.Title, product.Slug, product.Description, product.Price,
            product.CurrencyCode, product.Quantity, product.Unit, product.Status, product.Condition,
            product.IsNegotiable, product.IsFeatured,
            product.SellerId, product.Seller.BusinessName,
            product.CategoryId, product.Category.Name,
            product.BrandId, null, product.FarmId, null,
            null, null, null, null, null,
            product.AverageRating, product.ReviewCount, product.ViewCount, product.PublishedAt, product.CreatedAt,
            product.BucketId), 200, ct);
    }
}
