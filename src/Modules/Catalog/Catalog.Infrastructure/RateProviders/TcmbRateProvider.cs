namespace LivestockTrading.Catalog.Infrastructure.RateProviders;

using System.Globalization;
using System.Xml.Linq;
using LivestockTrading.Catalog.Application.Abstractions;
using Shared.Contracts.Catalog;

/// <summary>
/// TCMB primary rate provider (Tier 1, plan-doc 05-catalog.md §6:629-635).
/// Source: https://www.tcmb.gov.tr/kurlar/today.xml (daily 15:30 TR / 12:30 UTC publish).
/// TCMB XML TRY-base verir (1 USD = X TRY); RatesToUsd kontratina invert edilir.
/// Hata durumunda exception throw ETMEZ, RateFetchResult(Success=false, Error=...) doner.
///
/// XML yapisi (B.2 Adim 2 fresh-fetch ile teyitli):
/// - Root element: &lt;Tarih_Date Tarih="DD.MM.YYYY" Date="MM/DD/YYYY" Bulten_No="..."&gt;
/// - Currency element: &lt;Currency Kod="USD" CurrencyCode="USD"&gt; ... &lt;ForexSelling&gt;X&lt;/ForexSelling&gt; &lt;Unit&gt;1&lt;/Unit&gt; ...
/// - Tarih attribute name = "Tarih" (Turkish DD.MM.YYYY); Date attribute name = "Date" (US MM/DD/YYYY fallback).
/// </summary>
public sealed class TcmbRateProvider : IRateProvider
{
    private readonly HttpClient _http;

    public TcmbRateProvider(HttpClient http)
    {
        _http = http;
    }

    public RateProvider Source => RateProvider.Tcmb;

    public async Task<RateFetchResult> FetchAsync(CancellationToken ct)
    {
        try
        {
            var xml = await _http.GetStringAsync("kurlar/today.xml", ct);
            var doc = XDocument.Parse(xml);
            var root = doc.Root;

            if (root is null)
            {
                return new RateFetchResult(
                    DateOnly.FromDateTime(DateTime.UtcNow),
                    new Dictionary<string, decimal>(),
                    Success: false,
                    Error: "TCMB XML root element bulunamadi");
            }

            // RateDate: root attribute Tarih="DD.MM.YYYY" (Turkish) veya Date="MM/DD/YYYY" (US fallback)
            var tarihAttr = root.Attribute("Tarih")?.Value;
            var rateDate = TryParseTcmbDate(tarihAttr) ?? DateOnly.FromDateTime(DateTime.UtcNow);

            // TRY-base → USD-base invert
            // Step 1: USD/TRY kurunu bul (1 USD = ? TRY)
            var usdElement = root.Elements("Currency")
                .FirstOrDefault(c => string.Equals(
                    c.Attribute("Kod")?.Value,
                    "USD",
                    StringComparison.OrdinalIgnoreCase));

            if (usdElement is null)
            {
                return new RateFetchResult(
                    rateDate,
                    new Dictionary<string, decimal>(),
                    Success: false,
                    Error: "TCMB XML USD Currency elementi bulunamadi");
            }

            var usdToTryRate = TryParseDecimal(usdElement.Element("ForexSelling")?.Value);
            if (usdToTryRate is null or 0m)
            {
                return new RateFetchResult(
                    rateDate,
                    new Dictionary<string, decimal>(),
                    Success: false,
                    Error: "TCMB USD ForexSelling parse edilemedi veya 0");
            }

            // Step 2: Her currency icin XXX/TRY -> XXX/USD invert
            // 1 XXX = ForexSelling/Unit TRY, dolayisiyla 1 USD = usdToTryRate / (ForexSelling/Unit) XXX
            //       = usdToTryRate * Unit / ForexSelling XXX
            // RatesToUsd[XXX] = X XXX per 1 USD bicimi (plan-doc §6:648 kontrat).
            var rates = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

            // TRY kendisi: 1 USD = usdToTryRate TRY
            rates["TRY"] = usdToTryRate.Value;
            // USD kendisi: 1 USD = 1 USD
            rates["USD"] = 1m;

            foreach (var currency in root.Elements("Currency"))
            {
                var kod = currency.Attribute("Kod")?.Value;
                if (string.IsNullOrWhiteSpace(kod) || kod.Equals("USD", StringComparison.OrdinalIgnoreCase))
                    continue;

                var forexSelling = TryParseDecimal(currency.Element("ForexSelling")?.Value);
                var unit = TryParseDecimal(currency.Element("Unit")?.Value) ?? 1m;

                if (forexSelling is null or 0m || unit == 0m)
                    continue;

                var xxxPerUsd = usdToTryRate.Value * unit / forexSelling.Value;
                rates[kod.ToUpperInvariant()] = xxxPerUsd;
            }

            return new RateFetchResult(rateDate, rates, Success: true, Error: null);
        }
        catch (HttpRequestException ex)
        {
            return new RateFetchResult(
                DateOnly.FromDateTime(DateTime.UtcNow),
                new Dictionary<string, decimal>(),
                Success: false,
                Error: $"TCMB HTTP hatasi: {ex.Message}");
        }
        catch (TaskCanceledException ex)
        {
            return new RateFetchResult(
                DateOnly.FromDateTime(DateTime.UtcNow),
                new Dictionary<string, decimal>(),
                Success: false,
                Error: $"TCMB timeout / iptal: {ex.Message}");
        }
        catch (System.Xml.XmlException ex)
        {
            return new RateFetchResult(
                DateOnly.FromDateTime(DateTime.UtcNow),
                new Dictionary<string, decimal>(),
                Success: false,
                Error: $"TCMB XML parse hatasi: {ex.Message}");
        }
        catch (Exception ex)
        {
            return new RateFetchResult(
                DateOnly.FromDateTime(DateTime.UtcNow),
                new Dictionary<string, decimal>(),
                Success: false,
                Error: $"TCMB beklenmedik hata: {ex.GetType().Name}: {ex.Message}");
        }
    }

    private static DateOnly? TryParseTcmbDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        // TCMB Tarih format: "DD.MM.YYYY"
        if (DateOnly.TryParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d))
            return d;
        if (DateOnly.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d2))
            return d2;
        return null;
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
