using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Enums;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;
using Shared.Contracts.Events.Livestock;
using Shared.Infrastructure.Messaging;

namespace Livestock.Features.Products.Create;

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
