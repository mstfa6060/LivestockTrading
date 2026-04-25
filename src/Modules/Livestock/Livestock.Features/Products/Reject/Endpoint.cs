using FastEndpoints;
using Livestock.Domain.Enums;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.Events.Livestock;
using Shared.Infrastructure.Messaging;

namespace Livestock.Features.Products.Reject;

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
