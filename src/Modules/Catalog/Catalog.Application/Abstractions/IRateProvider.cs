namespace LivestockTrading.Catalog.Application.Abstractions;

using Shared.Contracts.Catalog;

/// <summary>
/// 3-tier currency rate provider port (plan-doc 05-catalog.md §6:629-730).
/// TCMB → ECB → exchangerate.host fallback chain.
/// Impl konumu: Catalog.Infrastructure/RateProviders/ (W3.6.B sub-batch B.2/B.3/B.4).
/// Plan-doc §6:640 "Catalog.Infrastructure.RateProviders" ns onerirdi; Frontend karari
/// Application port pattern (W3.5A ICacheService emsali) — Karar 1.
/// </summary>
public interface IRateProvider
{
    RateProvider Source { get; }
    Task<RateFetchResult> FetchAsync(CancellationToken ct);
}

/// <summary>
/// Plan-doc §6:648-652 birebir. RatesToUsd: ISO 4217 code → USD-base decimal rate.
/// Success=false ise Error doldurulur, RatesToUsd empty olabilir.
/// </summary>
public sealed record RateFetchResult(
    DateOnly RateDate,
    IReadOnlyDictionary<string, decimal> RatesToUsd,
    bool Success,
    string? Error);
