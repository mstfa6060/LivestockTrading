using LivestockTrading.Catalog.Application.Abstractions;
using LivestockTrading.Catalog.Domain.Entities;
using LivestockTrading.Catalog.Infrastructure.Persistence;
using LivestockTrading.Catalog.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;

namespace LivestockTrading.Catalog.Infrastructure.Scheduling;

/// <summary>
/// Daily 13:00 UTC currency rate update job (plan-doc 05-catalog.md §6:655-695 Cron Job grounded).
/// 3-tier chain: TCMB primary → ECB fallback → CurrencyApi (Fawazahmed0) last-resort.
/// First successful provider wins; all 3 fail -> InvalidOperationException (Quartz misfire policy).
/// Audit: every provider attempt (success or fail) appends a RateLog row (append-only, retention §6:716).
/// DI: IEnumerable&lt;IRateProvider&gt; W3.6.B AddTransient 3 factory delegate pattern preserved
/// (CatalogInfrastructureModule.cs:95-97); Tier sirasi DI kayit sirasiyla iterate edilir.
/// Plan-doc §6:663 `_tcmb/_ecb/_erh` ayri field ornegi pre-W3.6.B; fiili karar collection (KAYDET-7).
/// </summary>
[DisallowConcurrentExecution]
public sealed class CurrencyRateUpdateJob : IJob
{
    private readonly IEnumerable<IRateProvider> _providers;
    private readonly CatalogDbContext _db;
    private readonly ILogger<CurrencyRateUpdateJob> _logger;

    public CurrencyRateUpdateJob(
        IEnumerable<IRateProvider> providers,
        CatalogDbContext db,
        ILogger<CurrencyRateUpdateJob> logger)
    {
        ArgumentNullException.ThrowIfNull(providers);
        ArgumentNullException.ThrowIfNull(db);
        ArgumentNullException.ThrowIfNull(logger);
        _providers = providers;
        _db = db;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var ct = context.CancellationToken;
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
            throw new InvalidOperationException("All 3 currency rate providers failed");
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
    }
}
