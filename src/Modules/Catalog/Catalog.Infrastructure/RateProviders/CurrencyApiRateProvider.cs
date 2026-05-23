namespace LivestockTrading.Catalog.Infrastructure.RateProviders;

using System.Text.Json;
using System.Text.Json.Serialization;
using LivestockTrading.Catalog.Application.Abstractions;
using Shared.Contracts.Catalog;

/// <summary>
/// Fawazahmed0 currency-api last-resort rate provider (Tier 3, plan-doc 05-catalog.md §6:629-635 swap).
/// Source: https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@latest/v1/currencies/usd.json (jsdelivr CDN, Unlicense).
/// Multi-source aggregator (ECB + Forex Python + Currency-API), 150+ currency (crypto + traditional karisik), daily refresh.
/// Plan-doc §6:635 (2025) exchangerate.host'tan swap edildi (apilayer API key paywall, fiili 2026-05 fail) — W3.6.B Frontend karari.
/// USD-base direct response (lowercase nested 'usd: {eur: 0.92, ...}') — invert mantigi YOK.
/// JSON parse: BCL System.Text.Json.
/// Hata durumunda exception throw ETMEZ, RateFetchResult(Success=false, Error=...) doner.
///
/// JSON sema (B.4 Adim 2 fresh-fetch teyitli):
/// { "date": "YYYY-MM-DD", "usd": { "eur": 0.92, "try": 45.6, "1inch": 10.7, ... } }
/// "usd" property nested Dictionary, lowercase ISO codes + crypto coin'ler dahil.
/// Caller (CurrencyRateUpdateJob) Catalog DB Currency.Code match ile selective consume.
/// </summary>
public sealed class CurrencyApiRateProvider : IRateProvider
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly HttpClient _http;

    public CurrencyApiRateProvider(HttpClient http)
    {
        _http = http;
    }

    public RateProvider Source => RateProvider.CurrencyApi;

    public async Task<RateFetchResult> FetchAsync(CancellationToken ct)
    {
        try
        {
            // BaseAddress B.5'te set edilecek: https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@latest/
            // Bu turda relative path: v1/currencies/usd.json
            var json = await _http.GetStringAsync("v1/currencies/usd.json", ct);
            var response = JsonSerializer.Deserialize<CurrencyApiResponse>(json, JsonOpts);

            if (response is null)
            {
                return new RateFetchResult(
                    DateOnly.FromDateTime(DateTime.UtcNow),
                    new Dictionary<string, decimal>(),
                    Success: false,
                    Error: "currency-api JSON deserialize null donundu");
            }

            if (response.Usd is null or { Count: 0 })
            {
                return new RateFetchResult(
                    DateOnly.FromDateTime(DateTime.UtcNow),
                    new Dictionary<string, decimal>(),
                    Success: false,
                    Error: "currency-api 'usd' rates dictionary bos veya null");
            }

            // RateDate
            DateOnly rateDate;
            if (!string.IsNullOrWhiteSpace(response.Date)
                && DateOnly.TryParse(response.Date, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var parsed))
            {
                rateDate = parsed;
            }
            else
            {
                rateDate = DateOnly.FromDateTime(DateTime.UtcNow);
            }

            // USD-base direct: response.Usd zaten 1 USD = X XXX formatinda (lowercase keys).
            // RatesToUsd kontrati ile birebir uyumlu, invert YOK. Keys uppercase'e normalize.
            var rates = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

            // USD kendine: 1 USD = 1 USD (response.Usd icinde olmayabilir, defensive)
            rates["USD"] = 1m;

            foreach (var kv in response.Usd)
            {
                if (string.IsNullOrWhiteSpace(kv.Key) || kv.Value <= 0m)
                    continue;
                rates[kv.Key.ToUpperInvariant()] = kv.Value;
            }

            return new RateFetchResult(rateDate, rates, Success: true, Error: null);
        }
        catch (HttpRequestException ex)
        {
            return new RateFetchResult(
                DateOnly.FromDateTime(DateTime.UtcNow),
                new Dictionary<string, decimal>(),
                Success: false,
                Error: $"currency-api HTTP hatasi: {ex.Message}");
        }
        catch (TaskCanceledException ex)
        {
            return new RateFetchResult(
                DateOnly.FromDateTime(DateTime.UtcNow),
                new Dictionary<string, decimal>(),
                Success: false,
                Error: $"currency-api timeout / iptal: {ex.Message}");
        }
        catch (JsonException ex)
        {
            return new RateFetchResult(
                DateOnly.FromDateTime(DateTime.UtcNow),
                new Dictionary<string, decimal>(),
                Success: false,
                Error: $"currency-api JSON parse hatasi: {ex.Message}");
        }
        catch (Exception ex)
        {
            return new RateFetchResult(
                DateOnly.FromDateTime(DateTime.UtcNow),
                new Dictionary<string, decimal>(),
                Success: false,
                Error: $"currency-api beklenmedik hata: {ex.GetType().Name}: {ex.Message}");
        }
    }

    /// <summary>JSON DTO — Fawazahmed0 currency-api /usd.json response sema'sina karsi.</summary>
    private sealed class CurrencyApiResponse
    {
        [JsonPropertyName("date")]
        public string? Date { get; set; }

        [JsonPropertyName("usd")]
        public Dictionary<string, decimal>? Usd { get; set; }
    }
}
