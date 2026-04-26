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

public class GetAllProductsEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<ProductSearchRequest, List<ProductListItem>>
{
    public override void Configure()
    {
        Post("/livestocktrading/Products/All");
        AllowAnonymous();
        Tags("Products");
    }

    public override async Task HandleAsync(ProductSearchRequest req, CancellationToken ct)
    {
        // Resolve the seller filter from the flat SellerId OR the legacy
        // `filters: [{ key:"sellerId", values:[...] }]` shape.
        var sellerId = req.SellerId;
        if (sellerId is null && req.Filters is { Count: > 0 })
        {
            var sellerFilter = req.Filters.FirstOrDefault(f =>
                string.Equals(f.Key, "sellerId", StringComparison.OrdinalIgnoreCase));
            if (sellerFilter?.Values is { Count: > 0 } vals
                && Guid.TryParse(vals[0]?.ToString(), out var sid))
            {
                sellerId = sid;
            }
        }

        // Seller's own my-listings view must see Draft/Pending/Active, not
        // just Active. Detect "I am this seller" by matching the current
        // authenticated user to the seller's UserId.
        var currentUserId = user.UserId;
        var isOwnerView = false;
        if (sellerId.HasValue && currentUserId != Guid.Empty)
        {
            isOwnerView = await db.Sellers
                .AsNoTracking()
                .AnyAsync(s => s.Id == sellerId.Value && s.UserId == currentUserId, ct);
        }

        var query = db.Products
            .AsNoTracking()
            .Include(p => p.Seller)
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Location)
            .Where(p => !p.IsDeleted);

        if (!isOwnerView)
        {
            query = query.Where(p => p.Status == ProductStatus.Active);
        }

        if (sellerId.HasValue)
        {
            query = query.Where(p => p.SellerId == sellerId.Value);
        }

        if (!string.IsNullOrWhiteSpace(req.Slug))
        {
            query = query.Where(p => p.Slug == req.Slug);
        }

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

        // Accept both the flat {Page,PageSize} and the legacy nested
        // {pageRequest:{currentPage,perPageCount,listAll}} shapes.
        var pageRaw = req.PageRequest?.CurrentPage ?? req.Page;
        var pageSizeRaw = req.PageRequest?.PerPageCount ?? req.PageSize;
        var page = pageRaw > 0 ? pageRaw : 1;
        var pageSize = Math.Min(pageSizeRaw > 0 ? pageSizeRaw : 20, 100);
        var skip = (page - 1) * pageSize;

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
                p.AverageRating, p.ReviewCount, p.ViewCount, p.CreatedAt,
                // Legacy aliases the frontend maps against.
                p.Price,                   // BasePrice
                p.CurrencyCode,            // Currency
                p.Quantity,                // StockQuantity
                p.Quantity > 0,            // IsInStock
                (string?)null,             // ShortDescription — not on entity yet
                p.LocationId,
                p.Location != null ? p.Location.CountryCode : null,
                p.Location != null ? p.Location.City : null,
                (decimal?)null,            // DiscountedPrice — not on entity yet
                p.BucketId))
            .ToListAsync(ct);

        await SendAsync(products, 200, ct);
    }
}

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

public class CreateProductEndpoint(LivestockDbContext db, IUserContext user, IEventPublisher publisher) : Endpoint<CreateProductRequest, ProductDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/Products/Create");
        // No role gate: the handler auto-creates an Active seller profile
        // for a caller who doesn't have one yet, then enforces subscription
        // limits and the Active-status rule. A JWT-level Roles() check
        // would 403 fresh buyers whose token still carries "Buyer" even
        // after Sellers/Create ran — forcing a pointless re-login.
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

        // Enforce subscription listing limits
        var subscription = await db.SellerSubscriptions
            .Include(s => s.Plan)
            .Where(s => s.SubscriberId == seller.Id &&
                (s.Status == SubscriptionStatus.Active || s.Status == SubscriptionStatus.GracePeriod))
            .OrderByDescending(s => s.ExpiresAt)
            .FirstOrDefaultAsync(ct);

        const int defaultFreeListings = 5;
        var maxListings = subscription?.Plan?.MaxListings ?? defaultFreeListings;

        var activeListingCount = await db.Products.CountAsync(p =>
            p.SellerId == seller.Id && !p.IsDeleted && p.Status != ProductStatus.Rejected, ct);

        if (activeListingCount >= maxListings)
        {
            AddError(LivestockErrors.SubscriptionErrors.ListingLimitReached);
            await SendErrorsAsync(403, ct);
            return;
        }

        var slugExists = await db.Products.AnyAsync(p => p.Slug == req.Slug, ct);
        if (slugExists)
        {
            AddError(x => x.Slug, LivestockErrors.ProductErrors.ProductNotFound);
            await SendErrorsAsync(409, ct);
            return;
        }

        // Normalize legacy aliases (BasePrice/Currency/StockQuantity/StockUnit/
        // MediaBucketId) the old frontend client still sends, and persist
        // LocationId / BucketId from the request body.
        var price = req.Price > 0 ? req.Price : (req.BasePrice ?? 0m);
        var currencyCode = !string.IsNullOrWhiteSpace(req.CurrencyCode)
            ? req.CurrencyCode
            : (req.Currency ?? "USD");
        var quantity = req.Quantity > 0 ? req.Quantity : (req.StockQuantity ?? 0);
        var unit = !string.IsNullOrWhiteSpace(req.Unit) ? req.Unit : req.StockUnit;

        var product = new Product
        {
            SellerId = seller.Id,
            CategoryId = req.CategoryId,
            BrandId = req.BrandId,
            FarmId = req.FarmId,
            LocationId = req.LocationId,
            BucketId = req.MediaBucketId,
            Title = req.Title,
            Slug = req.Slug,
            Description = req.Description,
            Price = price,
            CurrencyCode = currencyCode,
            Quantity = quantity,
            Unit = unit,
            Condition = req.Condition,
            IsNegotiable = req.IsNegotiable,
            Status = ProductStatus.PendingApproval
        };

        db.Products.Add(product);
        await db.SaveChangesAsync(ct);

        // Create automatic price conversions for all active currencies
        var sourceCurrency = await db.Currencies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Code == product.CurrencyCode && c.IsActive, ct);

        if (sourceCurrency != null && sourceCurrency.ExchangeRateToUsd > 0)
        {
            var targetCurrencies = await db.Currencies
                .AsNoTracking()
                .Where(c => c.IsActive && c.Code != product.CurrencyCode && c.ExchangeRateToUsd > 0)
                .ToListAsync(ct);

            if (targetCurrencies.Count > 0)
            {
                var priceInUsd = product.Price / sourceCurrency.ExchangeRateToUsd;
                var conversions = targetCurrencies.Select(tc => new ProductPrice
                {
                    ProductId = product.Id,
                    CurrencyCode = tc.Code,
                    Price = Math.Round(priceInUsd * tc.ExchangeRateToUsd, 2),
                    IsActive = true,
                    IsAutomaticConversion = true
                }).ToList();

                db.ProductPrices.AddRange(conversions);
                await db.SaveChangesAsync(ct);
            }
        }

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
            0, 0, 0, null, product.CreatedAt,
            product.BucketId), 201, ct);
    }
}

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

public class DeleteProductEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<DeleteProductRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/livestocktrading/Products/Delete");
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
        Post("/livestocktrading/Products/Approve");
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
        Post("/livestocktrading/Products/Reject");
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
