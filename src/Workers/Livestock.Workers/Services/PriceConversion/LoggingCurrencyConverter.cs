using Microsoft.Extensions.Logging;

namespace Livestock.Workers.Services.PriceConversion;

public sealed class LoggingCurrencyConverter(ILogger<LoggingCurrencyConverter> logger) : ICurrencyConverter
{
    public Task<decimal> ConvertToUsdAsync(decimal amount, string fromCurrencyCode, CancellationToken ct = default)
    {
        logger.LogInformation("[PRICE-CONVERSION] {Amount} {Currency} → USD (placeholder)", amount, fromCurrencyCode);
        return Task.FromResult(amount);
    }
}
