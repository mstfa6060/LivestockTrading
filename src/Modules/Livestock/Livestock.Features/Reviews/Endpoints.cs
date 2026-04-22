using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;

namespace Livestock.Features.Reviews;

public class GetProductReviewsEndpoint(LivestockDbContext db) : Endpoint<GetProductReviewsRequest, List<ProductReviewItem>>
{
    public override void Configure()
    {
        Post("/livestocktrading/Reviews/ByProduct");
        AllowAnonymous();
        Tags("Reviews");
    }

    public override async Task HandleAsync(GetProductReviewsRequest req, CancellationToken ct)
    {
        var pageSize = Math.Min(req.PageSize, 100);
        var reviews = await db.ProductReviews
            .AsNoTracking()
            .Where(r => r.ProductId == req.ProductId)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((req.Page - 1) * pageSize).Take(pageSize)
            .Select(r => new ProductReviewItem(r.Id, r.ProductId, r.ReviewerUserId, r.Rating, r.Comment, r.IsVerifiedPurchase, r.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(reviews, 200, ct);
    }
}

public class CreateProductReviewEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<CreateProductReviewRequest, ProductReviewItem>
{
    public override void Configure()
    {
        Post("/livestocktrading/Reviews/CreateForProduct");
        Tags("Reviews");
    }

    public override async Task HandleAsync(CreateProductReviewRequest req, CancellationToken ct)
    {
        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == req.ProductId, ct);
        if (product is null)
        {
            AddError(LivestockErrors.ProductErrors.ProductNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        var existing = await db.ProductReviews.AnyAsync(r => r.ProductId == req.ProductId && r.ReviewerUserId == user.UserId, ct);
        if (existing)
        {
            AddError(LivestockErrors.ReviewErrors.ReviewAlreadySubmitted);
            await SendErrorsAsync(409, ct);
            return;
        }

        var review = new ProductReview
        {
            ProductId = req.ProductId,
            ReviewerUserId = user.UserId,
            Rating = req.Rating,
            Comment = req.Comment
        };
        db.ProductReviews.Add(review);

        var count = await db.ProductReviews.CountAsync(r => r.ProductId == req.ProductId, ct);
        product.AverageRating = ((product.AverageRating * count) + req.Rating) / (count + 1);
        product.ReviewCount = count + 1;

        await db.SaveChangesAsync(ct);

        await SendAsync(new ProductReviewItem(review.Id, review.ProductId, review.ReviewerUserId, review.Rating, review.Comment, review.IsVerifiedPurchase, review.CreatedAt), 201, ct);
    }
}

public class GetSellerReviewsEndpoint(LivestockDbContext db) : Endpoint<GetSellerReviewsRequest, List<SellerReviewItem>>
{
    public override void Configure()
    {
        Post("/livestocktrading/Reviews/BySeller");
        AllowAnonymous();
        Tags("Reviews");
    }

    public override async Task HandleAsync(GetSellerReviewsRequest req, CancellationToken ct)
    {
        var pageSize = Math.Min(req.PageSize, 100);
        var reviews = await db.SellerReviews
            .AsNoTracking()
            .Where(r => r.SellerId == req.SellerId)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((req.Page - 1) * pageSize).Take(pageSize)
            .Select(r => new SellerReviewItem(r.Id, r.SellerId, r.ReviewerUserId, r.Rating, r.Comment, r.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(reviews, 200, ct);
    }
}

public class CreateSellerReviewEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<CreateSellerReviewRequest, SellerReviewItem>
{
    public override void Configure()
    {
        Post("/livestocktrading/Reviews/CreateForSeller");
        Tags("Reviews");
    }

    public override async Task HandleAsync(CreateSellerReviewRequest req, CancellationToken ct)
    {
        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.Id == req.SellerId, ct);
        if (seller is null)
        {
            AddError(LivestockErrors.SellerErrors.SellerNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        var existing = await db.SellerReviews.AnyAsync(r => r.SellerId == req.SellerId && r.ReviewerUserId == user.UserId, ct);
        if (existing)
        {
            AddError(LivestockErrors.ReviewErrors.ReviewAlreadySubmitted);
            await SendErrorsAsync(409, ct);
            return;
        }

        var review = new SellerReview
        {
            SellerId = req.SellerId,
            ReviewerUserId = user.UserId,
            Rating = req.Rating,
            Comment = req.Comment,
            DealId = req.DealId
        };
        db.SellerReviews.Add(review);

        var count = await db.SellerReviews.CountAsync(r => r.SellerId == req.SellerId, ct);
        seller.AverageRating = ((seller.AverageRating * count) + req.Rating) / (count + 1);
        seller.ReviewCount = count + 1;

        await db.SaveChangesAsync(ct);

        await SendAsync(new SellerReviewItem(review.Id, review.SellerId, review.ReviewerUserId, review.Rating, review.Comment, review.CreatedAt), 201, ct);
    }
}

public class GetTransporterReviewsEndpoint(LivestockDbContext db) : Endpoint<GetTransporterReviewsRequest, List<TransporterReviewItem>>
{
    public override void Configure()
    {
        Post("/livestocktrading/Reviews/ByTransporter");
        AllowAnonymous();
        Tags("Reviews");
    }

    public override async Task HandleAsync(GetTransporterReviewsRequest req, CancellationToken ct)
    {
        var pageSize = Math.Min(req.PageSize, 100);
        var reviews = await db.TransporterReviews
            .AsNoTracking()
            .Where(r => r.TransporterId == req.TransporterId)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((req.Page - 1) * pageSize).Take(pageSize)
            .Select(r => new TransporterReviewItem(r.Id, r.TransporterId, r.ReviewerUserId, r.Rating, r.Comment, r.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(reviews, 200, ct);
    }
}
