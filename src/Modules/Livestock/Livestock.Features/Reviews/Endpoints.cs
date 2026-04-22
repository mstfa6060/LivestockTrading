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

public class UpdateProductReviewEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<UpdateProductReviewRequest, ProductReviewItem>
{
    public override void Configure()
    {
        Post("/livestocktrading/Reviews/UpdateForProduct");
        Tags("Reviews");
    }

    public override async Task HandleAsync(UpdateProductReviewRequest req, CancellationToken ct)
    {
        var review = await db.ProductReviews.FirstOrDefaultAsync(r => r.Id == req.Id, ct);
        if (review is null)
        {
            AddError(LivestockErrors.ReviewErrors.ReviewNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        if (review.ReviewerUserId != user.UserId)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        review.Rating = req.Rating;
        review.Comment = req.Comment;
        review.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
        await RecomputeProductRatingAsync(db, review.ProductId, ct);

        await SendAsync(new ProductReviewItem(review.Id, review.ProductId, review.ReviewerUserId, review.Rating, review.Comment, review.IsVerifiedPurchase, review.CreatedAt), 200, ct);
    }

    internal static async Task RecomputeProductRatingAsync(LivestockDbContext db, Guid productId, CancellationToken ct)
    {
        var stats = await db.ProductReviews
            .Where(r => r.ProductId == productId && !r.IsDeleted)
            .GroupBy(r => 1)
            .Select(g => new { Avg = g.Average(r => (double)r.Rating), Count = g.Count() })
            .FirstOrDefaultAsync(ct);

        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == productId, ct);
        if (product is null) { return; }

        product.AverageRating = stats?.Avg ?? 0;
        product.ReviewCount = stats?.Count ?? 0;
        await db.SaveChangesAsync(ct);
    }
}

public class DeleteProductReviewEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<DeleteReviewRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/livestocktrading/Reviews/DeleteForProduct");
        Tags("Reviews");
    }

    public override async Task HandleAsync(DeleteReviewRequest req, CancellationToken ct)
    {
        var review = await db.ProductReviews.FirstOrDefaultAsync(r => r.Id == req.Id, ct);
        if (review is null)
        {
            AddError(LivestockErrors.ReviewErrors.ReviewNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        if (review.ReviewerUserId != user.UserId)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        review.IsDeleted = true;
        review.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await UpdateProductReviewEndpoint.RecomputeProductRatingAsync(db, review.ProductId, ct);

        await SendNoContentAsync(ct);
    }
}

public class UpdateSellerReviewEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<UpdateSellerReviewRequest, SellerReviewItem>
{
    public override void Configure()
    {
        Post("/livestocktrading/Reviews/UpdateForSeller");
        Tags("Reviews");
    }

    public override async Task HandleAsync(UpdateSellerReviewRequest req, CancellationToken ct)
    {
        var review = await db.SellerReviews.FirstOrDefaultAsync(r => r.Id == req.Id, ct);
        if (review is null)
        {
            AddError(LivestockErrors.ReviewErrors.ReviewNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        if (review.ReviewerUserId != user.UserId)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        review.Rating = req.Rating;
        review.Comment = req.Comment;
        review.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
        await RecomputeSellerRatingAsync(db, review.SellerId, ct);

        await SendAsync(new SellerReviewItem(review.Id, review.SellerId, review.ReviewerUserId, review.Rating, review.Comment, review.CreatedAt), 200, ct);
    }

    internal static async Task RecomputeSellerRatingAsync(LivestockDbContext db, Guid sellerId, CancellationToken ct)
    {
        var stats = await db.SellerReviews
            .Where(r => r.SellerId == sellerId && !r.IsDeleted)
            .GroupBy(r => 1)
            .Select(g => new { Avg = g.Average(r => (double)r.Rating), Count = g.Count() })
            .FirstOrDefaultAsync(ct);

        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.Id == sellerId, ct);
        if (seller is null) { return; }

        seller.AverageRating = stats?.Avg ?? 0;
        seller.ReviewCount = stats?.Count ?? 0;
        await db.SaveChangesAsync(ct);
    }
}

public class DeleteSellerReviewEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<DeleteReviewRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/livestocktrading/Reviews/DeleteForSeller");
        Tags("Reviews");
    }

    public override async Task HandleAsync(DeleteReviewRequest req, CancellationToken ct)
    {
        var review = await db.SellerReviews.FirstOrDefaultAsync(r => r.Id == req.Id, ct);
        if (review is null)
        {
            AddError(LivestockErrors.ReviewErrors.ReviewNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        if (review.ReviewerUserId != user.UserId)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        review.IsDeleted = true;
        review.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await UpdateSellerReviewEndpoint.RecomputeSellerRatingAsync(db, review.SellerId, ct);

        await SendNoContentAsync(ct);
    }
}

public class CreateTransporterReviewEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<CreateTransporterReviewRequest, TransporterReviewItem>
{
    public override void Configure()
    {
        Post("/livestocktrading/Reviews/CreateForTransporter");
        Tags("Reviews");
    }

    public override async Task HandleAsync(CreateTransporterReviewRequest req, CancellationToken ct)
    {
        var transporter = await db.Transporters.FirstOrDefaultAsync(t => t.Id == req.TransporterId, ct);
        if (transporter is null)
        {
            AddError(LivestockErrors.TransportErrors.TransporterNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        var existing = await db.TransporterReviews.AnyAsync(r => r.TransporterId == req.TransporterId && r.ReviewerUserId == user.UserId, ct);
        if (existing)
        {
            AddError(LivestockErrors.ReviewErrors.ReviewAlreadySubmitted);
            await SendErrorsAsync(409, ct);
            return;
        }

        var review = new TransporterReview
        {
            TransporterId = req.TransporterId,
            ReviewerUserId = user.UserId,
            Rating = req.Rating,
            Comment = req.Comment,
            TransportRequestId = req.TransportRequestId
        };
        db.TransporterReviews.Add(review);
        await db.SaveChangesAsync(ct);
        await RecomputeTransporterRatingAsync(db, req.TransporterId, ct);

        await SendAsync(new TransporterReviewItem(review.Id, review.TransporterId, review.ReviewerUserId, review.Rating, review.Comment, review.CreatedAt), 201, ct);
    }

    internal static async Task RecomputeTransporterRatingAsync(LivestockDbContext db, Guid transporterId, CancellationToken ct)
    {
        var stats = await db.TransporterReviews
            .Where(r => r.TransporterId == transporterId && !r.IsDeleted)
            .GroupBy(r => 1)
            .Select(g => new { Avg = g.Average(r => (double)r.Rating), Count = g.Count() })
            .FirstOrDefaultAsync(ct);

        var transporter = await db.Transporters.FirstOrDefaultAsync(t => t.Id == transporterId, ct);
        if (transporter is null) { return; }

        transporter.AverageRating = stats?.Avg ?? 0;
        transporter.ReviewCount = stats?.Count ?? 0;
        await db.SaveChangesAsync(ct);
    }
}

public class UpdateTransporterReviewEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<UpdateTransporterReviewRequest, TransporterReviewItem>
{
    public override void Configure()
    {
        Post("/livestocktrading/Reviews/UpdateForTransporter");
        Tags("Reviews");
    }

    public override async Task HandleAsync(UpdateTransporterReviewRequest req, CancellationToken ct)
    {
        var review = await db.TransporterReviews.FirstOrDefaultAsync(r => r.Id == req.Id, ct);
        if (review is null)
        {
            AddError(LivestockErrors.ReviewErrors.ReviewNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        if (review.ReviewerUserId != user.UserId)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        review.Rating = req.Rating;
        review.Comment = req.Comment;
        review.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
        await CreateTransporterReviewEndpoint.RecomputeTransporterRatingAsync(db, review.TransporterId, ct);

        await SendAsync(new TransporterReviewItem(review.Id, review.TransporterId, review.ReviewerUserId, review.Rating, review.Comment, review.CreatedAt), 200, ct);
    }
}

public class DeleteTransporterReviewEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<DeleteReviewRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/livestocktrading/Reviews/DeleteForTransporter");
        Tags("Reviews");
    }

    public override async Task HandleAsync(DeleteReviewRequest req, CancellationToken ct)
    {
        var review = await db.TransporterReviews.FirstOrDefaultAsync(r => r.Id == req.Id, ct);
        if (review is null)
        {
            AddError(LivestockErrors.ReviewErrors.ReviewNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        if (review.ReviewerUserId != user.UserId)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        review.IsDeleted = true;
        review.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await CreateTransporterReviewEndpoint.RecomputeTransporterRatingAsync(db, review.TransporterId, ct);

        await SendNoContentAsync(ct);
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
