using LivestockTrading.Catalog.Application.Abstractions;
using Microsoft.Extensions.Logging;
using Quartz;

namespace LivestockTrading.Catalog.Infrastructure.Scheduling;

/// <summary>
/// Daily 13:00 UTC currency rate update job (Quartz cron trigger).
/// Delegates 3-tier provider chain orchestration to ICurrencyRateRefresher (W3.6.D D.1)
/// — single source of truth, also called by RefreshExchangeRatesHandler manual admin trigger
/// (W3.6.D D.3 swap). Job sorumlulugu yalniz: cron tetikleme + start/end log + Result.Failure
/// ise InvalidOperationException throw (Quartz misfire policy + DisallowConcurrentExecution
/// semantik). Plan-doc grounding: 05-catalog.md §6:655-697 + W3.6.D DRY delege karari.
/// </summary>
[DisallowConcurrentExecution]
public sealed class CurrencyRateUpdateJob : IJob
{
    private readonly ICurrencyRateRefresher _refresher;
    private readonly ILogger<CurrencyRateUpdateJob> _logger;

    public CurrencyRateUpdateJob(
        ICurrencyRateRefresher refresher,
        ILogger<CurrencyRateUpdateJob> logger)
    {
        ArgumentNullException.ThrowIfNull(refresher);
        ArgumentNullException.ThrowIfNull(logger);
        _refresher = refresher;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("CurrencyRateUpdateJob started at {FireTime}", context.FireTimeUtc);

        var result = await _refresher.RefreshAsync(context.CancellationToken);

        if (!result.IsSuccess)
        {
            throw new InvalidOperationException(
                $"Currency rate refresh failed: {result.Error?.Code} {result.Error?.Message}");
        }

        _logger.LogInformation("CurrencyRateUpdateJob completed successfully");
    }
}
