using LivestockTrading.Catalog.Application.Abstractions;
using MassTransit;

namespace LivestockTrading.Catalog.Application.Features.Currencies;

/// <summary>
/// Manual currency rate refresh trigger via admin endpoint (W3.6.D D.3).
/// Delegates to ICurrencyRateRefresher (single source of truth, also called by Quartz
/// CurrencyRateUpdateJob daily 13:00 UTC). MassTransit IConsumer pattern korunur;
/// Result.Success / Result.Failure RespondAsync ile endpoint client.GetResponse&lt;Result&gt;
/// (RefreshExchangeRatesEndpoint.cs) tarafindan tuketilir.
/// Plan-doc grounding: 05-catalog.md §5 IAdminCatalogCommands.RefreshExchangeRatesAsync +
/// W3.6.D DRY delege karari. NotImpl stub Wave 2 W2.4-D issue #161/165 backlog kapanis.
/// </summary>
public sealed class RefreshExchangeRatesHandler : IConsumer<RefreshExchangeRatesCommand>
{
    private readonly ICurrencyRateRefresher _refresher;

    public RefreshExchangeRatesHandler(ICurrencyRateRefresher refresher)
    {
        ArgumentNullException.ThrowIfNull(refresher);
        _refresher = refresher;
    }

    public async Task Consume(ConsumeContext<RefreshExchangeRatesCommand> context)
    {
        var result = await _refresher.RefreshAsync(context.CancellationToken);
        await context.RespondAsync(result);
    }
}
