using FastEndpoints;
using Livestock.Domain.Enums;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;

namespace Livestock.Features.Products.All;

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
