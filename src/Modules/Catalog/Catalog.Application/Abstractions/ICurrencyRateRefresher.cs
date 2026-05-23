namespace LivestockTrading.Catalog.Application.Abstractions;

using Shared.Results;

/// <summary>
/// RefreshExchangeRatesHandler swap portu. Impl Catalog.Infrastructure'da W3.6.D'de gelecek
/// (CurrencyRateRefresher concrete sinifi 3-tier IRateProvider chain'i + RateLog audit'i orkestrasyon).
/// W3.6.B'de SADECE port tanimi; Handler bu portu inject ederek Infrastructure ref ihlali olmadan
/// chain'i tetikler. Plan-doc 05-catalog.md §6:629-730 Cron Job davranisinin manuel admin tetigi karsiligi.
/// </summary>
public interface ICurrencyRateRefresher
{
    Task<Result> RefreshAsync(CancellationToken ct);
}
