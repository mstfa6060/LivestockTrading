namespace Shared.Contracts.Catalog.Admin;

using Shared.Contracts.Catalog;

/// <summary>§6:697-711 currency rate refresh audit (GetRateLogsAsync diagnostic). RatesJson dahil (admin tam tanı: raw provider response, future bug + manual override). DateOnly = rate günü, FetchedAt = fetch anı.</summary>
public sealed record RateLogEntry(
    DateOnly RateDate,
    RateProvider Source,
    string RatesJson,
    bool Success,
    string? Error,
    DateTimeOffset FetchedAt);
