using FastEndpoints;
using Livestock.Domain.Enums;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Livestock.Features.Admin.Products;

public class ListPendingProductsEndpoint(LivestockDbContext db) : EndpointWithoutRequest<List<PendingProductItem>>
{
    public override void Configure()
    {
        Post("/livestocktrading/Admin/Products/Pending");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Admin");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var products = await db.Products
            .AsNoTracking()
            .Include(p => p.Seller)
            .Include(p => p.Category)
            .Where(p => p.Status == ProductStatus.PendingApproval && !p.IsDeleted)
            .OrderBy(p => p.CreatedAt)
            .Select(p => new PendingProductItem(
                p.Id, p.Title, p.Slug, p.Price, p.CurrencyCode,
                p.SellerId, p.Seller.BusinessName,
                p.CategoryId, p.Category.Name,
                p.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(products, 200, ct);
    }
}
