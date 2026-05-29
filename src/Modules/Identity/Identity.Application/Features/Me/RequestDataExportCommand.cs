namespace LivestockTrading.Identity.Application.Features.Me;

/// <summary>
/// POST /identity/users/me/data-export — accepts a data export request and
/// emits DataExportRequested for the async worker (W4.3 Quartz job) to fulfil.
/// Faz 1 only supports the JSON format because plan-doc §11 shows the worker
/// serialising with JsonSerializer.SerializeToUtf8Bytes — no other format
/// pipeline exists yet.
/// </summary>
public sealed record DataExportRequest(string Format);

public sealed record RequestDataExportCommand(Guid UserId, DataExportRequest Dto);

public sealed record DataExportAccepted(Guid JobId, DateTimeOffset EstimatedReadyAt);
