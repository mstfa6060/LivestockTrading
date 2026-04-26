using FastEndpoints;
using Livestock.Domain.Enums;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;

namespace Livestock.Features.Offers.Reject;

public class RejectOfferEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<RejectOfferRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/livestocktrading/Offers/Reject");
        Tags("Offers");
    }

    public override async Task HandleAsync(RejectOfferRequest req, CancellationToken ct)
    {
        var offer = await db.Offers.FirstOrDefaultAsync(o => o.Id == req.Id, ct);
        if (offer is null)
        {
            AddError(LivestockErrors.OfferErrors.OfferNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (seller is null || offer.SellerId != seller.Id)
        {
            AddError(LivestockErrors.Common.Unauthorized);
            await SendErrorsAsync(403, ct);
            return;
        }

        if (offer.Status != OfferStatus.Pending)
        {
            AddError(LivestockErrors.OfferErrors.OfferAlreadyProcessed);
            await SendErrorsAsync(409, ct);
            return;
        }

        offer.Status = OfferStatus.Rejected;
        offer.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}
