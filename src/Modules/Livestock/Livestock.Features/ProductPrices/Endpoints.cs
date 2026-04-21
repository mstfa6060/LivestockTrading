using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;

namespace Livestock.Features.ProductPrices;

public class GetProductPricesEndpoint(LivestockDbContext db) : Endpoint<GetProductPricesRequest, List<ProductPriceItem>>
{
    public override void Configure()
    {
        Get("/Products/{ProductId}/Prices");
        AllowAnonymous();
        Tags("ProductPrices");
    }

    public override async Task HandleAsync(GetProductPricesRequest req, CancellationToken ct)
    {
        var prices = await db.ProductPrices
            .AsNoTracking()
            .Where(p => p.ProductId == req.ProductId && !p.IsDeleted)
            .OrderByDescending(p => p.IsActive)
            .ThenByDescending(p => p.CreatedAt)
            .Select(p => new ProductPriceItem(p.Id, p.ProductId, p.CurrencyCode, p.Price, p.DiscountedPrice, p.CountryCodes, p.IsActive, p.IsAutomaticConversion, p.ValidFrom, p.ValidUntil, p.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(prices, 200, ct);
    }
}

public class GetProductPriceEndpoint(LivestockDbContext db) : Endpoint<GetProductPriceRequest, ProductPriceItem>
{
    public override void Configure()
    {
        Get("/ProductPrices/{Id}");
        AllowAnonymous();
        Tags("ProductPrices");
    }

    public override async Task HandleAsync(GetProductPriceRequest req, CancellationToken ct)
    {
        var p = await db.ProductPrices.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (p is null)
        {
            AddError(LivestockErrors.ProductPriceErrors.ProductPriceNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        await SendAsync(new ProductPriceItem(p.Id, p.ProductId, p.CurrencyCode, p.Price, p.DiscountedPrice, p.CountryCodes, p.IsActive, p.IsAutomaticConversion, p.ValidFrom, p.ValidUntil, p.CreatedAt), 200, ct);
    }
}

public class CreateProductPriceEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<CreateProductPriceRequest, ProductPriceItem>
{
    public override void Configure()
    {
        Post("/ProductPrices");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("ProductPrices");
    }

    public override async Task HandleAsync(CreateProductPriceRequest req, CancellationToken ct)
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
            AddError(LivestockErrors.ProductPriceErrors.ProductPriceNotOwnedBySeller);
            await SendErrorsAsync(403, ct);
            return;
        }

        var price = new ProductPrice
        {
            ProductId = req.ProductId,
            CurrencyCode = req.CurrencyCode.ToUpperInvariant(),
            Price = req.Price,
            DiscountedPrice = req.DiscountedPrice,
            CountryCodes = req.CountryCodes,
            IsActive = req.IsActive,
            IsAutomaticConversion = req.IsAutomaticConversion,
            ValidFrom = req.ValidFrom,
            ValidUntil = req.ValidUntil
        };

        db.ProductPrices.Add(price);
        await db.SaveChangesAsync(ct);

        await SendAsync(new ProductPriceItem(price.Id, price.ProductId, price.CurrencyCode, price.Price, price.DiscountedPrice, price.CountryCodes, price.IsActive, price.IsAutomaticConversion, price.ValidFrom, price.ValidUntil, price.CreatedAt), 201, ct);
    }
}

public class UpdateProductPriceEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<UpdateProductPriceRequest, ProductPriceItem>
{
    public override void Configure()
    {
        Put("/ProductPrices/{Id}");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("ProductPrices");
    }

    public override async Task HandleAsync(UpdateProductPriceRequest req, CancellationToken ct)
    {
        var price = await db.ProductPrices.Include(p => p.Product).FirstOrDefaultAsync(p => p.Id == req.Id && !p.IsDeleted, ct);
        if (price is null)
        {
            AddError(LivestockErrors.ProductPriceErrors.ProductPriceNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        var seller = await db.Sellers.AsNoTracking().FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (!user.IsInRole("LivestockTrading.Admin") && (seller is null || price.Product.SellerId != seller.Id))
        {
            AddError(LivestockErrors.ProductPriceErrors.ProductPriceNotOwnedBySeller);
            await SendErrorsAsync(403, ct);
            return;
        }

        price.CurrencyCode = req.CurrencyCode.ToUpperInvariant();
        price.Price = req.Price;
        price.DiscountedPrice = req.DiscountedPrice;
        price.CountryCodes = req.CountryCodes;
        price.IsActive = req.IsActive;
        price.IsAutomaticConversion = req.IsAutomaticConversion;
        price.ValidFrom = req.ValidFrom;
        price.ValidUntil = req.ValidUntil;
        price.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        await SendAsync(new ProductPriceItem(price.Id, price.ProductId, price.CurrencyCode, price.Price, price.DiscountedPrice, price.CountryCodes, price.IsActive, price.IsAutomaticConversion, price.ValidFrom, price.ValidUntil, price.CreatedAt), 200, ct);
    }
}

public class DeleteProductPriceEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<DeleteProductPriceRequest, EmptyResponse>
{
    public override void Configure()
    {
        Delete("/ProductPrices/{Id}");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("ProductPrices");
    }

    public override async Task HandleAsync(DeleteProductPriceRequest req, CancellationToken ct)
    {
        var price = await db.ProductPrices.Include(p => p.Product).FirstOrDefaultAsync(p => p.Id == req.Id && !p.IsDeleted, ct);
        if (price is null)
        {
            AddError(LivestockErrors.ProductPriceErrors.ProductPriceNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        var seller = await db.Sellers.AsNoTracking().FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (!user.IsInRole("LivestockTrading.Admin") && (seller is null || price.Product.SellerId != seller.Id))
        {
            AddError(LivestockErrors.ProductPriceErrors.ProductPriceNotOwnedBySeller);
            await SendErrorsAsync(403, ct);
            return;
        }

        price.IsDeleted = true;
        price.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}
