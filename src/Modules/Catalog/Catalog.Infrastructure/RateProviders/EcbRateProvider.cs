namespace LivestockTrading.Catalog.Infrastructure.RateProviders;

using System.Globalization;
using System.Xml.Linq;
using LivestockTrading.Catalog.Application.Abstractions;
using Shared.Contracts.Catalog;

/// <summary>
/// ECB fallback rate provider (Tier 2, plan-doc 05-catalog.md §6:629-635).
/// Source: https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml (daily 16:00 CET publish).
/// ECB XML EUR-base verir (1 EUR = X foreign currency); RatesToUsd kontratina invert edilir.
/// XML namespace LocalName filtering ile parse edilir (3rd-party ns degisikligine dayanikli).
/// Hata durumunda exception throw ETMEZ, RateFetchResult(Success=false, Error=...) doner.
///
/// XML yapisi (B.3 Adim 2 fresh-fetch ile teyitli):
/// - Root: &lt;gesmes:Envelope xmlns:gesmes="..." xmlns="..."&gt;
/// - Hiyerarsi: Envelope &gt; Cube (outer) &gt; Cube[@time='YYYY-MM-DD'] &gt; Cube[@currency='XXX' @rate='Y.YYY']
/// - EUR explicit YOK (referans birim, 1 EUR = 1 EUR convention).
/// </summary>
public sealed class EcbRateProvider : IRateProvider
{
    private readonly HttpClient _http;

    public EcbRateProvider(HttpClient http)
    {
        _http = http;
    }

    public RateProvider Source => RateProvider.Ecb;

    public async Task<RateFetchResult> FetchAsync(CancellationToken ct)
    {
        try
        {
            var xml = await _http.GetStringAsync("stats/eurofxref/eurofxref-daily.xml", ct);
            var doc = XDocument.Parse(xml);

            // ECB XML: gesmes:Envelope > Cube > Cube[@time] > Cube[@currency,@rate]
            // LocalName filtering ile namespace ignore: pragmatic defensive parsing.
            var allCubes = doc.Descendants()
                .Where(e => e.Name.LocalName == "Cube")
                .ToList();

            // Time-bearing Cube (gun Cube'i): time attribute olan
            var timeCube = allCubes.FirstOrDefault(c => c.Attribute("time") is not null);

            DateOnly rateDate;
            if (timeCube?.Attribute("time")?.Value is { } timeStr
                && DateOnly.TryParse(timeStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
            {
                rateDate = parsed;
            }
            else
            {
                rateDate = DateOnly.FromDateTime(DateTime.UtcNow);
            }

            // Currency Cube'lari: currency + rate attribute olan
            var currencyCubes = allCubes
                .Where(c => c.Attribute("currency") is not null && c.Attribute("rate") is not null)
                .ToList();

            if (currencyCubes.Count == 0)
            {
                return new RateFetchResult(
                    rateDate,
                    new Dictionary<string, decimal>(),
                    Success: false,
                    Error: "ECB XML currency Cube elementleri bulunamadi");
            }

            // Step 1: EUR/USD kurunu bul (1 EUR = ? USD)
            var usdCube = currencyCubes.FirstOrDefault(c => string.Equals(
                c.Attribute("currency")?.Value,
                "USD",
                StringComparison.OrdinalIgnoreCase));

            if (usdCube is null)
            {
                return new RateFetchResult(
                    rateDate,
                    new Dictionary<string, decimal>(),
                    Success: false,
                    Error: "ECB XML USD currency Cube bulunamadi");
            }

            var eurToUsdRate = TryParseDecimal(usdCube.Attribute("rate")?.Value);
            if (eurToUsdRate is null or 0m)
            {
                return new RateFetchResult(
                    rateDate,
                    new Dictionary<string, decimal>(),
                    Success: false,
                    Error: "ECB EUR/USD rate parse edilemedi veya 0");
            }

            // Step 2: Her currency icin EUR-base -> USD-base invert
            // ECB verir: 1 EUR = rate XXX (yani XXX/EUR = rate)
            // RatesToUsd kontrati: 1 USD = ? XXX
            // 1 USD = (1/eurToUsdRate) EUR = (1/eurToUsdRate) * rate XXX = rate / eurToUsdRate XXX
            var rates = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

            // USD kendine: 1 USD = 1 USD
            rates["USD"] = 1m;
            // EUR: 1 USD = (1/eurToUsdRate) EUR
            rates["EUR"] = 1m / eurToUsdRate.Value;

            foreach (var cube in currencyCubes)
            {
                var code = cube.Attribute("currency")?.Value;
                if (string.IsNullOrWhiteSpace(code) || code.Equals("USD", StringComparison.OrdinalIgnoreCase))
                    continue;

                var rate = TryParseDecimal(cube.Attribute("rate")?.Value);
                if (rate is null or 0m)
                    continue;

                // 1 USD = rate / eurToUsdRate XXX
                rates[code.ToUpperInvariant()] = rate.Value / eurToUsdRate.Value;
            }

            return new RateFetchResult(rateDate, rates, Success: true, Error: null);
        }
        catch (HttpRequestException ex)
        {
            return new RateFetchResult(
                DateOnly.FromDateTime(DateTime.UtcNow),
                new Dictionary<string, decimal>(),
                Success: false,
                Error: $"ECB HTTP hatasi: {ex.Message}");
        }
        catch (TaskCanceledException ex)
        {
            return new RateFetchResult(
                DateOnly.FromDateTime(DateTime.UtcNow),
                new Dictionary<string, decimal>(),
                Success: false,
                Error: $"ECB timeout / iptal: {ex.Message}");
        }
        catch (System.Xml.XmlException ex)
        {
            return new RateFetchResult(
                DateOnly.FromDateTime(DateTime.UtcNow),
                new Dictionary<string, decimal>(),
                Success: false,
                Error: $"ECB XML parse hatasi: {ex.Message}");
        }
        catch (Exception ex)
        {
            return new RateFetchResult(
                DateOnly.FromDateTime(DateTime.UtcNow),
                new Dictionary<string, decimal>(),
                Success: false,
                Error: $"ECB beklenmedik hata: {ex.GetType().Name}: {ex.Message}");
        }
    }

    private static decimal? TryParseDecimal(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var d))
            return d;
        return null;
    }
}
