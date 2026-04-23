using FastEndpoints;
using Livestock.Domain.Enums;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;

namespace Livestock.Features.Dashboard;

public class GetAdminDashboardEndpoint(LivestockDbContext db) : EndpointWithoutRequest<DashboardStats>
{
    public override void Configure()
    {
        Get("/Dashboard/Stats");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Dashboard");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var totalProducts = await db.Products.CountAsync(p => !p.IsDeleted, ct);
        var activeProducts = await db.Products.CountAsync(p => !p.IsDeleted && p.Status == ProductStatus.Active, ct);
        var pendingProducts = await db.Products.CountAsync(p => !p.IsDeleted && p.Status == ProductStatus.PendingApproval, ct);
        var totalSellers = await db.Sellers.CountAsync(s => !s.IsDeleted, ct);
        var verifiedSellers = await db.Sellers.CountAsync(s => !s.IsDeleted && s.Status == SellerStatus.Active, ct);
        var totalOffers = await db.Offers.CountAsync(o => !o.IsDeleted, ct);
        var pendingOffers = await db.Offers.CountAsync(o => !o.IsDeleted && o.Status == OfferStatus.Pending, ct);
        var totalConversations = await db.Conversations.CountAsync(c => !c.IsDeleted, ct);
        var totalDeals = await db.Deals.CountAsync(d => !d.IsDeleted, ct);
        var totalTransportRequests = await db.TransportRequests.CountAsync(t => !t.IsDeleted, ct);

        await SendAsync(new DashboardStats(
            totalProducts, activeProducts, pendingProducts,
            totalSellers, verifiedSellers,
            totalOffers, pendingOffers,
            totalConversations, totalDeals, totalTransportRequests), 200, ct);
    }
}

public class GetSellerDashboardEndpoint(LivestockDbContext db, IUserContext user) : EndpointWithoutRequest<SellerDashboardStats>
{
    public override void Configure()
    {
        Get("/Dashboard/SellerStats");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("Dashboard");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var seller = await db.Sellers.AsNoTracking().FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (seller is null) { await SendNotFoundAsync(ct); return; }

        var total = await db.Products.CountAsync(p => p.SellerId == seller.Id && !p.IsDeleted, ct);
        var active = await db.Products.CountAsync(p => p.SellerId == seller.Id && !p.IsDeleted && p.Status == ProductStatus.Active, ct);
        var pending = await db.Products.CountAsync(p => p.SellerId == seller.Id && !p.IsDeleted && p.Status == ProductStatus.PendingApproval, ct);
        var totalViews = await db.Products.Where(p => p.SellerId == seller.Id && !p.IsDeleted).SumAsync(p => p.ViewCount, ct);
        var productIds = await db.Products.Where(p => p.SellerId == seller.Id && !p.IsDeleted).Select(p => p.Id).ToListAsync(ct);
        var ratings = await db.ProductReviews.Where(r => productIds.Contains(r.ProductId) && !r.IsDeleted).ToListAsync(ct);
        var totalOffers = await db.Offers.CountAsync(o => !o.IsDeleted && productIds.Contains(o.ProductId), ct);
        var pendingOffers = await db.Offers.CountAsync(o => !o.IsDeleted && o.Status == OfferStatus.Pending && productIds.Contains(o.ProductId), ct);
        var totalDeals = await db.Deals.CountAsync(d => !d.IsDeleted && d.SellerId == seller.Id, ct);
        var totalFavorites = await db.FavoriteProducts.CountAsync(f => !f.IsDeleted && productIds.Contains(f.ProductId), ct);

        var avgRating = ratings.Count > 0 ? ratings.Average(r => r.Rating) : 0;

        await SendAsync(new SellerDashboardStats(
            total, active, pending, totalViews,
            avgRating, ratings.Count,
            totalOffers, pendingOffers, totalDeals, totalFavorites), 200, ct);
    }
}
