using FastEndpoints;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;

namespace Livestock.Features.Products.Delete;

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
