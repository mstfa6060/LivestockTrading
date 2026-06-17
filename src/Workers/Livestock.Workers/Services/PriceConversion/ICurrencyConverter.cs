namespace Livestock.Workers.Services.PriceConversion;

public interface ICurrencyConverter
{
    Task<decimal> ConvertToUsdAsync(decimal amount, string fromCurrencyCode, CancellationToken ct = default);
}
