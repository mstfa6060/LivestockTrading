namespace LivestockTrading.Catalog.Application.Features.Currencies;

/// <summary>
/// Command for manually triggering an exchange-rate refresh (admin trigger).
/// Real implementation Wave 3 (Catalog.Infrastructure.RateProviders).
/// </summary>
public sealed record RefreshExchangeRatesCommand(Guid ActorAdminId);
