using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Enums;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;
using Shared.Contracts.Events.Livestock;
using Shared.Infrastructure.Messaging;

namespace Livestock.Features.Products;

public class GetAllProductsEndpoint(LivestockDbContext db) : Endpoint<ProductSearchRequest, List<ProductListItem>>
{
    public override void Configure()
    {
        Post("/Products/Search");
        AllowAnonymous();
        Tags("Products");
    }

    public override async Task HandleAsync(ProductSearchRequest req, CancellationToken ct)
    {
        var query = db.Products
            .AsNoTracking()
            .Include(p => p.Seller)
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Location)
            .Where(p => p.Status == ProductStatus.Active);

        if (!string.IsNullOrWhiteSpace(req.Keyword))
        {
            query = query.Where(p => p.Title.Contains(req.Keyword) || (p.Description != null && p.Description.Contains(req.Keyword)));
        }

        if (req.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == req.CategoryId.Value);
        }

        if (req.BrandId.HasValue)
        {
            query = query.Where(p => p.BrandId == req.BrandId.Value);
        }

        if (!string.IsNullOrWhiteSpace(req.CountryCode))
        {
            query = query.Where(p => p.Location != null && p.Location.CountryCode == req.CountryCode);
        }

        if (req.MinPrice.HasValue)
        {
            query = query.Where(p => p.Price >= req.MinPrice.Value);
        }

        if (req.MaxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= req.MaxPrice.Value);
        }

        if (req.IsNegotiable.HasValue)
        {
            query = query.Where(p => p.IsNegotiable == req.IsNegotiable.Value);
        }

        if (req.Condition.HasValue)
        {
            query = query.Where(p => p.Condition == req.Condition.Value);
        }

        var pageSize = Math.Min(req.PageSize, 100);
        var skip = (req.Page - 1) * pageSize;

        var products = await query
            .OrderByDescending(p => p.IsFeatured)
            .ThenByDescending(p => p.CreatedAt)
            .Skip(skip).Take(pageSize)
            .Select(p => new ProductListItem(
                p.Id, p.Title, p.Slug, p.Price, p.CurrencyCode, p.Quantity, p.Unit,
                p.Status, p.Condition, p.IsNegotiable, p.IsFeatured,
                p.SellerId, p.Seller.BusinessName,
                p.CategoryId, p.Category.Name,
                p.BrandId, p.Brand != null ? p.Brand.Name : null,
                p.Location != null ? p.Location.CountryCode : null,
                p.Location != null ? p.Location.City : null,
                p.AverageRating, p.ReviewCount, p.ViewCount, p.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(products, 200, ct);
    }
}

public class GetProductEndpoint(LivestockDbContext db) : Endpoint<GetProductRequest, ProductDetail>
{
    public override void Configure()
    {
        Get("/Products/{Id}");
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
            p.AverageRating, p.ReviewCount, p.ViewCount, p.PublishedAt, p.CreatedAt), 200, ct);
    }
}

public class CreateProductEndpoint(LivestockDbContext db, IUserContext user, IEventPublisher publisher) : Endpoint<CreateProductRequest, ProductDetail>
{
    public override void Configure()
    {
        Post("/Products");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("Products");
    }

    public override async Task HandleAsync(CreateProductRequest req, CancellationToken ct)
    {
        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (seller is null)
        {
            seller = new Seller
            {
                UserId = user.UserId,
                BusinessName = user.Email,
                Email = user.Email,
                Status = SellerStatus.Active
            };
            db.Sellers.Add(seller);
            await db.SaveChangesAsync(ct);

            await publisher.PublishAsync(SellerRegisteredEvent.Subject, new SellerRegisteredEvent
            {
                SellerId = seller.Id,
                UserId = seller.UserId,
                BusinessName = seller.BusinessName
            }, ct);
        }

        if (seller.Status != SellerStatus.Active)
        {
            AddError(LivestockErrors.SellerErrors.SellerNotVerified);
            await SendErrorsAsync(403, ct);
            return;
        }

        var activeSub = await db.SellerSubscriptions
            .Include(s => s.Plan)
            .Where(s => s.SubscriberId == seller.Id && s.Status == SubscriptionStatus.Active && s.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(s => s.ExpiresAt)
            .FirstOrDefaultAsync(ct);

        if (activeSub is not null && activeSub.Plan.MaxListings > 0)
        {
            var productCount = await db.Products
                .CountAsync(p => p.SellerId == seller.Id && !p.IsDeleted, ct);

            if (productCount >= activeSub.Plan.MaxListings)
            {
                AddError(LivestockErrors.SubscriptionErrors.ListingLimitReached);
                await SendErrorsAsync(403, ct);
                return;
            }
        }

        var slugExists = await db.Products.AnyAsync(p => p.Slug == req.Slug, ct);
        if (slugExists)
        {
            AddError(x => x.Slug, LivestockErrors.ProductErrors.ProductNotFound);
            await SendErrorsAsync(409, ct);
            return;
        }

        var product = new Product
        {
            SellerId = seller.Id,
            CategoryId = req.CategoryId,
            BrandId = req.BrandId,
            FarmId = req.FarmId,
            Title = req.Title,
            Slug = req.Slug,
            Description = req.Description,
            Price = req.Price,
            CurrencyCode = req.CurrencyCode,
            Quantity = req.Quantity,
            Unit = req.Unit,
            Condition = req.Condition,
            IsNegotiable = req.IsNegotiable,
            Status = ProductStatus.PendingApproval
        };

        db.Products.Add(product);
        await db.SaveChangesAsync(ct);

        await publisher.PublishAsync(ProductCreatedEvent.Subject, new ProductCreatedEvent
        {
            ProductId = product.Id,
            SellerId = seller.Id,
            Title = product.Title,
            Slug = product.Slug,
            Price = product.Price,
            CurrencyCode = product.CurrencyCode
        }, ct);

        await SendAsync(new ProductDetail(
            product.Id, product.Title, product.Slug, product.Description, product.Price,
            product.CurrencyCode, product.Quantity, product.Unit, product.Status, product.Condition,
            product.IsNegotiable, product.IsFeatured,
            seller.Id, seller.BusinessName,
            product.CategoryId, string.Empty,
            product.BrandId, null, product.FarmId, null,
            null, null, null, null, null,
            0, 0, 0, null, product.CreatedAt), 201, ct);
    }
}

public class UpdateProductEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<UpdateProductRequest, ProductDetail>
{
    public override void Configure()
    {
        Put("/Products/{Id}");
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
            product.AverageRating, product.ReviewCount, product.ViewCount, product.PublishedAt, product.CreatedAt), 200, ct);
    }
}

public class DeleteProductEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<DeleteProductRequest, EmptyResponse>
{
    public override void Configure()
    {
        Delete("/Products/{Id}");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("Products");
    }

    public override async Task HandleAsync(DeleteProductRequest req, CancellationToken ct)
    {
        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == req.Id, ct);
        if (product is null)
        {
            AddError(LivestockErrors.ProductErrors.ProductNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (!user.IsInRole("LivestockTrading.Admin") && (seller is null || product.SellerId != seller.Id))
        {
            AddError(LivestockErrors.ProductErrors.ProductNotOwnedBySeller);
            await SendErrorsAsync(403, ct);
            return;
        }

        product.IsDeleted = true; product.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}

public class ApproveProductEndpoint(LivestockDbContext db, IEventPublisher publisher) : Endpoint<ApproveProductRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/Products/{Id}/Approve");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Products");
    }

    public override async Task HandleAsync(ApproveProductRequest req, CancellationToken ct)
    {
        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == req.Id, ct);
        if (product is null)
        {
            AddError(LivestockErrors.ProductErrors.ProductNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        var sellerLocation = await db.Locations
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.OwnerId == product.SellerId && l.OwnerType == "Seller", ct);

        product.Status = ProductStatus.Active;
        product.PublishedAt ??= DateTime.UtcNow;
        product.ExpiresAt = ComputeExpiry(sellerLocation?.CountryCode);
        product.RejectionReason = null;
        product.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        await publisher.PublishAsync(ProductApprovedEvent.Subject, new ProductApprovedEvent
        {
            ProductId = product.Id,
            SellerId = product.SellerId
        }, ct);

        await SendNoContentAsync(ct);
    }

    private static DateTime ComputeExpiry(string? countryCode)
    {
        var tzId = GetTimezoneForCountry(countryCode);
        try
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(tzId);
            var nowInTz = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
            var expiryDateInTz = nowInTz.Date.AddDays(30);
            var expiryInTz = new DateTime(expiryDateInTz.Year, expiryDateInTz.Month, expiryDateInTz.Day, 23, 59, 59, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(expiryInTz, tz);
        }
        catch
        {
            return DateTime.UtcNow.Date.AddDays(30).AddHours(23).AddMinutes(59).AddSeconds(59);
        }
    }

    private static string GetTimezoneForCountry(string? countryCode) => countryCode switch
    {
        "TR" => "Europe/Istanbul",
        "DE" => "Europe/Berlin",
        "FR" => "Europe/Paris",
        "GB" => "Europe/London",
        "RU" => "Europe/Moscow",
        "US" => "America/New_York",
        "CN" => "Asia/Shanghai",
        "JP" => "Asia/Tokyo",
        "AU" => "Australia/Sydney",
        "AE" => "Asia/Dubai",
        "SA" => "Asia/Riyadh",
        "KZ" => "Asia/Almaty",
        "PL" => "Europe/Warsaw",
        "UA" => "Europe/Kyiv",
        _ => "UTC"
    };
}

public class RejectProductEndpoint(LivestockDbContext db, IEventPublisher publisher) : Endpoint<RejectProductRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/Products/{Id}/Reject");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Products");
    }

    public override async Task HandleAsync(RejectProductRequest req, CancellationToken ct)
    {
        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == req.Id, ct);
        if (product is null)
        {
            AddError(LivestockErrors.ProductErrors.ProductNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        product.Status = ProductStatus.Rejected;
        product.RejectionReason = req.Reason;
        product.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        await publisher.PublishAsync(ProductRejectedEvent.Subject, new ProductRejectedEvent
        {
            ProductId = product.Id,
            SellerId = product.SellerId,
            Reason = req.Reason
        }, ct);

        await SendNoContentAsync(ct);
    }
}
