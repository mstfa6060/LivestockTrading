using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;

namespace Livestock.Features.ProductImages;

public class GetProductImagesEndpoint(LivestockDbContext db) : Endpoint<GetProductImagesRequest, List<ProductImageItem>>
{
    public override void Configure()
    {
        Get("/Products/{ProductId}/Images");
        AllowAnonymous();
        Tags("ProductImages");
    }

    public override async Task HandleAsync(GetProductImagesRequest req, CancellationToken ct)
    {
        var images = await db.ProductImages
            .AsNoTracking()
            .Where(i => i.ProductId == req.ProductId && !i.IsDeleted)
            .OrderBy(i => i.SortOrder)
            .Select(i => new ProductImageItem(i.Id, i.ProductId, i.ImageUrl, i.ThumbnailUrl, i.AltText, i.SortOrder, i.IsPrimary, i.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(images, 200, ct);
    }
}

public class GetProductImageEndpoint(LivestockDbContext db) : Endpoint<GetProductImageRequest, ProductImageItem>
{
    public override void Configure()
    {
        Get("/ProductImages/{Id}");
        AllowAnonymous();
        Tags("ProductImages");
    }

    public override async Task HandleAsync(GetProductImageRequest req, CancellationToken ct)
    {
        var i = await db.ProductImages.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (i is null)
        {
            AddError(LivestockErrors.ProductImageErrors.ProductImageNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        await SendAsync(new ProductImageItem(i.Id, i.ProductId, i.ImageUrl, i.ThumbnailUrl, i.AltText, i.SortOrder, i.IsPrimary, i.CreatedAt), 200, ct);
    }
}

public class CreateProductImageEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<CreateProductImageRequest, ProductImageItem>
{
    public override void Configure()
    {
        Post("/ProductImages");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("ProductImages");
    }

    public override async Task HandleAsync(CreateProductImageRequest req, CancellationToken ct)
    {
        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == req.ProductId, ct);
        if (product is null)
        {
            AddError(LivestockErrors.ProductErrors.ProductNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        var seller = await db.Sellers.AsNoTracking().FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (!user.IsInRole("LivestockTrading.Admin") && (seller is null || product.SellerId != seller.Id))
        {
            AddError(LivestockErrors.ProductImageErrors.ProductImageNotOwnedBySeller);
            await SendErrorsAsync(403, ct);
            return;
        }

        if (req.IsPrimary)
        {
            await db.ProductImages
                .Where(i => i.ProductId == req.ProductId && i.IsPrimary)
                .ExecuteUpdateAsync(s => s.SetProperty(i => i.IsPrimary, false), ct);
        }

        var image = new ProductImage
        {
            ProductId = req.ProductId,
            ImageUrl = req.ImageUrl,
            ThumbnailUrl = req.ThumbnailUrl,
            AltText = req.AltText,
            SortOrder = req.SortOrder,
            IsPrimary = req.IsPrimary
        };

        db.ProductImages.Add(image);
        await db.SaveChangesAsync(ct);

        await SendAsync(new ProductImageItem(image.Id, image.ProductId, image.ImageUrl, image.ThumbnailUrl, image.AltText, image.SortOrder, image.IsPrimary, image.CreatedAt), 201, ct);
    }
}

public class DeleteProductImageEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<DeleteProductImageRequest, EmptyResponse>
{
    public override void Configure()
    {
        Delete("/ProductImages/{Id}");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("ProductImages");
    }

    public override async Task HandleAsync(DeleteProductImageRequest req, CancellationToken ct)
    {
        var image = await db.ProductImages.Include(i => i.Product).FirstOrDefaultAsync(i => i.Id == req.Id && !i.IsDeleted, ct);
        if (image is null)
        {
            AddError(LivestockErrors.ProductImageErrors.ProductImageNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        var seller = await db.Sellers.AsNoTracking().FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (!user.IsInRole("LivestockTrading.Admin") && (seller is null || image.Product.SellerId != seller.Id))
        {
            AddError(LivestockErrors.ProductImageErrors.ProductImageNotOwnedBySeller);
            await SendErrorsAsync(403, ct);
            return;
        }

        image.IsDeleted = true;
        image.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}

public class SetPrimaryImageEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<SetPrimaryImageRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/ProductImages/{Id}/SetPrimary");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("ProductImages");
    }

    public override async Task HandleAsync(SetPrimaryImageRequest req, CancellationToken ct)
    {
        var image = await db.ProductImages.Include(i => i.Product).FirstOrDefaultAsync(i => i.Id == req.Id && !i.IsDeleted, ct);
        if (image is null)
        {
            AddError(LivestockErrors.ProductImageErrors.ProductImageNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        var seller = await db.Sellers.AsNoTracking().FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (!user.IsInRole("LivestockTrading.Admin") && (seller is null || image.Product.SellerId != seller.Id))
        {
            AddError(LivestockErrors.ProductImageErrors.ProductImageNotOwnedBySeller);
            await SendErrorsAsync(403, ct);
            return;
        }

        await db.ProductImages
            .Where(i => i.ProductId == image.ProductId && i.IsPrimary)
            .ExecuteUpdateAsync(s => s.SetProperty(i => i.IsPrimary, false), ct);

        image.IsPrimary = true;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}
