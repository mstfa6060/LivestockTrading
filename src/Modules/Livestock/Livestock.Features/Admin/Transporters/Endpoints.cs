using FastEndpoints;
using Livestock.Domain.Enums;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Livestock.Features.Admin.Transporters;

public class ListPendingTransportersEndpoint(LivestockDbContext db) : EndpointWithoutRequest<List<PendingTransporterItem>>
{
    public override void Configure()
    {
        Get("/Admin/Transporters/Pending");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Admin");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var transporters = await db.Transporters
            .AsNoTracking()
            .Where(t => t.Status == TransporterStatus.PendingVerification)
            .OrderBy(t => t.CreatedAt)
            .Select(t => new PendingTransporterItem(t.Id, t.UserId, t.CompanyName, t.Email, t.PhoneNumber, t.LicenseNumber, t.Status, t.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(transporters, 200, ct);
    }
}

public class RejectTransporterEndpoint(LivestockDbContext db) : Endpoint<RejectTransporterRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/Admin/Transporters/{Id}/Reject");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Admin");
    }

    public override async Task HandleAsync(RejectTransporterRequest req, CancellationToken ct)
    {
        var transporter = await db.Transporters.FirstOrDefaultAsync(t => t.Id == req.Id, ct);
        if (transporter is null)
        {
            AddError(LivestockErrors.TransportErrors.TransporterNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        transporter.Status = TransporterStatus.Suspended;
        transporter.SuspensionReason = req.Reason;
        transporter.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}
