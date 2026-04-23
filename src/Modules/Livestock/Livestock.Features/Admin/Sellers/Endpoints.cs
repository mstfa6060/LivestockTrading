using FastEndpoints;
using Livestock.Domain.Enums;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Livestock.Features.Admin.Sellers;

public class ListPendingSellersEndpoint(LivestockDbContext db) : EndpointWithoutRequest<List<PendingSellerItem>>
{
    public override void Configure()
    {
        Get("/Admin/Sellers/Pending");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Admin");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var sellers = await db.Sellers
            .AsNoTracking()
            .Where(s => s.Status == SellerStatus.PendingVerification)
            .OrderBy(s => s.CreatedAt)
            .Select(s => new PendingSellerItem(s.Id, s.UserId, s.BusinessName, s.Email, s.PhoneNumber, s.TaxNumber, s.Status, s.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(sellers, 200, ct);
    }
}

public class RejectSellerEndpoint(LivestockDbContext db) : Endpoint<RejectSellerRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/Admin/Sellers/{Id}/Reject");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Admin");
    }

    public override async Task HandleAsync(RejectSellerRequest req, CancellationToken ct)
    {
        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.Id == req.Id, ct);
        if (seller is null)
        {
            AddError(LivestockErrors.SellerErrors.SellerNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        seller.Status = SellerStatus.Suspended;
        seller.SuspensionReason = req.Reason;
        seller.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}
