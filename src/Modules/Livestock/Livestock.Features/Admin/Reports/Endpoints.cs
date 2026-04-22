using FastEndpoints;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Livestock.Features.Admin.Reports;

public class ListReportsEndpoint(LivestockDbContext db) : EndpointWithoutRequest<List<ReportListItem>>
{
    public override void Configure()
    {
        Post("/livestocktrading/Admin/Reports/All");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Admin");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var reports = await db.ProductReports
            .AsNoTracking()
            .Include(r => r.Product)
            .OrderBy(r => r.IsResolved)
            .ThenByDescending(r => r.CreatedAt)
            .Select(r => new ReportListItem(
                r.Id, r.ProductId, r.Product.Title, r.ReporterUserId,
                r.Reason, r.Details, r.IsResolved, r.Resolution, r.ResolvedAt, r.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(reports, 200, ct);
    }
}

public class ResolveReportEndpoint(LivestockDbContext db) : Endpoint<ResolveReportRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/livestocktrading/Admin/Reports/Resolve");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Admin");
    }

    public override async Task HandleAsync(ResolveReportRequest req, CancellationToken ct)
    {
        var report = await db.ProductReports.FirstOrDefaultAsync(r => r.Id == req.Id, ct);
        if (report is null)
        {
            AddError(LivestockErrors.ReportErrors.ReportNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        if (report.IsResolved)
        {
            AddError(LivestockErrors.ReportErrors.ReportAlreadyResolved);
            await SendErrorsAsync(409, ct);
            return;
        }

        report.IsResolved = true;
        report.Resolution = req.Resolution;
        report.ResolvedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}
