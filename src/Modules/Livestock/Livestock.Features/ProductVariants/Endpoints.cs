using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;

namespace Livestock.Features.ProductVariants;

public class GetProductVariantsEndpoint(LivestockDbContext db) : Endpoint<GetProductVariantsRequest, List<ProductVariantDetail>>
{
    public override void Configure()
    {
        Get("/ProductVariants/ByProduct/{ProductId}");
        AllowAnonymous();
        Tags("ProductVariants");
    }

    public override async Task HandleAsync(GetProductVariantsRequest req, CancellationToken ct)
    {
        var variants = await db.ProductVariants.AsNoTracking()
            .Where(v => v.ProductId == req.ProductId && !v.IsDeleted)
            .Select(v => ProductVariantMapper.Map(v))
            .ToListAsync(ct);
        await SendAsync(variants, 200, ct);
    }
}

public class GetProductVariantEndpoint(LivestockDbContext db) : Endpoint<GetProductVariantRequest, ProductVariantDetail>
{
    public override void Configure()
    {
        Get("/ProductVariants/{Id}");
        AllowAnonymous();
        Tags("ProductVariants");
    }

    public override async Task HandleAsync(GetProductVariantRequest req, CancellationToken ct)
    {
        var v = await db.ProductVariants.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (v is null) { AddError(LivestockErrors.ProductVariantErrors.ProductVariantNotFound); await SendErrorsAsync(404, ct); return; }
        await SendAsync(ProductVariantMapper.Map(v), 200, ct);
    }
}

public class CreateProductVariantEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<CreateProductVariantRequest, ProductVariantDetail>
{
    public override void Configure()
    {
        Post("/ProductVariants");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("ProductVariants");
    }

    public override async Task HandleAsync(CreateProductVariantRequest req, CancellationToken ct)
    {
        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == req.ProductId && !p.IsDeleted, ct);
        if (product is null) { AddError(LivestockErrors.ProductErrors.ProductNotFound); await SendErrorsAsync(404, ct); return; }

        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (!user.IsInRole("LivestockTrading.Admin") && (seller is null || product.SellerId != seller.Id))
        { AddError(LivestockErrors.ProductVariantErrors.ProductVariantNotOwnedBySeller); await SendErrorsAsync(403, ct); return; }

        var v = new ProductVariant
        {
            ProductId = req.ProductId, Name = req.Name, Sku = req.Sku,
            PriceAdjustment = req.PriceAdjustment, Quantity = req.Quantity,
            Attributes = req.Attributes, IsActive = req.IsActive
        };
        db.ProductVariants.Add(v);
        await db.SaveChangesAsync(ct);
        await SendAsync(ProductVariantMapper.Map(v), 201, ct);
    }
}

public class UpdateProductVariantEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<UpdateProductVariantRequest, ProductVariantDetail>
{
    public override void Configure()
    {
        Put("/ProductVariants/{Id}");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("ProductVariants");
    }

    public override async Task HandleAsync(UpdateProductVariantRequest req, CancellationToken ct)
    {
        var v = await db.ProductVariants.Include(x => x.Product).FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (v is null) { AddError(LivestockErrors.ProductVariantErrors.ProductVariantNotFound); await SendErrorsAsync(404, ct); return; }

        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (!user.IsInRole("LivestockTrading.Admin") && (seller is null || v.Product.SellerId != seller.Id))
        { AddError(LivestockErrors.ProductVariantErrors.ProductVariantNotOwnedBySeller); await SendErrorsAsync(403, ct); return; }

        v.Name = req.Name; v.Sku = req.Sku; v.PriceAdjustment = req.PriceAdjustment;
        v.Quantity = req.Quantity; v.Attributes = req.Attributes; v.IsActive = req.IsActive;
        v.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendAsync(ProductVariantMapper.Map(v), 200, ct);
    }
}

public class DeleteProductVariantEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<DeleteProductVariantRequest, EmptyResponse>
{
    public override void Configure()
    {
        Delete("/ProductVariants/{Id}");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("ProductVariants");
    }

    public override async Task HandleAsync(DeleteProductVariantRequest req, CancellationToken ct)
    {
        var v = await db.ProductVariants.Include(x => x.Product).FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (v is null) { AddError(LivestockErrors.ProductVariantErrors.ProductVariantNotFound); await SendErrorsAsync(404, ct); return; }

        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (!user.IsInRole("LivestockTrading.Admin") && (seller is null || v.Product.SellerId != seller.Id))
        { AddError(LivestockErrors.ProductVariantErrors.ProductVariantNotOwnedBySeller); await SendErrorsAsync(403, ct); return; }

        v.IsDeleted = true; v.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}

file static class ProductVariantMapper
{
    internal static ProductVariantDetail Map(ProductVariant v) => new(
        v.Id, v.ProductId, v.Name, v.Sku,
        v.PriceAdjustment, v.Quantity, v.Attributes, v.IsActive, v.CreatedAt);
}
