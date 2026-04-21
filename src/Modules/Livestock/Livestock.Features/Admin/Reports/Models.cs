namespace Livestock.Features.Admin.Reports;

public record ReportListItem(
    Guid Id,
    Guid ProductId,
    string ProductTitle,
    Guid ReporterUserId,
    string Reason,
    string? Details,
    bool IsResolved,
    string? Resolution,
    DateTime? ResolvedAt,
    DateTime CreatedAt);

public record ResolveReportRequest(Guid Id, string Resolution);
