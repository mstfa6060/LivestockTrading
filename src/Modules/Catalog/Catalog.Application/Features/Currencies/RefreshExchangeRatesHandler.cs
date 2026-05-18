using MassTransit;

namespace LivestockTrading.Catalog.Application.Features.Currencies;

public sealed class RefreshExchangeRatesHandler : IConsumer<RefreshExchangeRatesCommand>
{
    public Task Consume(ConsumeContext<RefreshExchangeRatesCommand> context)
    {
        throw new NotImplementedException(
            "Catalog.Infrastructure.RateProviders bağımlılığı Wave 3 — issue #161/165 backlog. " +
            "Real implementation: IRateProvider 3-tier (TCMB/ECB/exchangerate.host) + Quartz CurrencyRateUpdateJob + RateLog audit.");
    }
}
