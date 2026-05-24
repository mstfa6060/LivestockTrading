using LivestockTrading.Catalog.Application.Abstractions;
using LivestockTrading.Catalog.Domain.Entities;
using LivestockTrading.Catalog.Infrastructure.Persistence;
using LivestockTrading.Catalog.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Results;

namespace LivestockTrading.Catalog.Infrastructure.Currencies;

/// <summary>
/// Currency rate refresher concrete implementation (W3.6.D).
/// 3-tier provider chain orchestration: TCMB primary, ECB fallback, CurrencyApi last resort.
/// First successful provider wins; all 3 fail returns Result.Failure after RateLog audit persist.
/// Every provider attempt (success or fail) appends a RateLog row (append-only audit, §6:716 retention).
/// Caller paths:
///  - CurrencyRateUpdateJob (Quartz cron, daily 13:00 UTC) — W3.6.C, delege D.2 revize
///  - RefreshExchangeRatesHandler (admin manual MassTransit IConsumer) — D.3 swap
/// DI: IEnumerable&lt;IRateProvider&gt; W3.6.B AddTransient 3 factory delegate (Tier sirasi DI kayit
/// sirasiyla iterate, CatalogInfrastructureModule.cs:95-97).
/// Plan-doc grounding: 05-catalog.md §6:655-697 Cron Job + §5 IAdminCatalogCommands.RefreshExchangeRatesAsync.
/// </summary>
public sealed class CurrencyRateRefresher : ICurrencyRateRefresher
{
    private readonly IEnumerable<IRateProvider> _providers;
    private readonly CatalogDbContext _db;
    private readonly ILogger<CurrencyRateRefresher> _logger;

    public CurrencyRateRefresher(
        IEnumerable<IRateProvider> providers,
        CatalogDbContext db,
        ILogger<CurrencyRateRefresher> logger)
    {
        ArgumentNullException.ThrowIfNull(providers);
        ArgumentNullException.ThrowIfNull(db);
        ArgumentNullException.ThrowIfNull(logger);
        _providers = providers;
        _db = db;
        _logger = logger;
    }

    public async Task<Result> RefreshAsync(CancellationToken ct)
    {
        RateFetchResult? successful = null;

        foreach (var provider in _providers)
        {
            var result = await provider.FetchAsync(ct);
            _db.Set<RateLog>().Add(RateLog.Create(provider.Source, result));

            if (result.Success)
            {
                successful = result;
                _logger.LogInformation(
                    "Currency rate fetch succeeded: provider={Source} rateDate={RateDate} count={Count}",
                    provider.Source, result.RateDate, result.RatesToUsd.Count);
                break;
            }

            _logger.LogWarning(
                "Currency rate fetch failed: provider={Source} error={Error}",
                provider.Source, result.Error);
        }

        if (successful is null)
        {
            await _db.SaveChangesAsync(ct);
            var error = new Error("RATE_REFRESH_ALL_PROVIDERS_FAILED", "All 3 currency rate providers failed");
            return Result.Failure(error);
        }

        var activeCurrencies = await _db.Set<Currency>()
            .Where(c => c.IsActive)
            .ToListAsync(ct);

        foreach (var currency in activeCurrencies)
        {
            if (successful.RatesToUsd.TryGetValue(currency.Code, out var rate))
            {
                currency.UpdateRate(rate);
            }
        }

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
