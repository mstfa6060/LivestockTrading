using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using LivestockTrading.Infrastructure.RelationalDB;
using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Workers.NotificationSender.Services;

/// <summary>
/// Scheduled background service that periodically fetches exchange rates
/// from external APIs and updates the ExchangeRates and Currencies tables.
/// Runs every hour with fallback between multiple rate providers.
/// </summary>
public class ExchangeRateUpdaterService : BackgroundService
{
    private readonly ILogger<ExchangeRateUpdaterService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly HttpClient _httpClient;
    private readonly TimeSpan _updateInterval = TimeSpan.FromHours(1);

    public ExchangeRateUpdaterService(
        ILogger<ExchangeRateUpdaterService> logger,
        IServiceProvider serviceProvider,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _httpClient = httpClientFactory.CreateClient();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ExchangeRateUpdaterService started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await UpdateExchangeRates(stoppingToken);
                _logger.LogInformation("Exchange rates updated successfully at {Time}", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating exchange rates");
            }

            await Task.Delay(_updateInterval, stoppingToken);
        }
    }

    private async Task UpdateExchangeRates(CancellationToken ct)
    {
        // Try primary API (Frankfurter - based on ECB data)
        var rates = await FetchFromFrankfurter(ct);

        if (rates == null || rates.Count == 0)
        {
            _logger.LogWarning("Frankfurter API failed, trying fallback");
            rates = await FetchFromExchangeRateHost(ct);
        }

        if (rates == null || rates.Count == 0)
        {
            _logger.LogWarning("All exchange rate APIs failed, keeping existing rates");
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LivestockTradingModuleDbContext>();

        foreach (var (currencyCode, rate) in rates)
        {
            // Update or insert ExchangeRate record
            var existingRate = await dbContext.ExchangeRates
                .FirstOrDefaultAsync(r => r.FromCurrency == "USD" && r.ToCurrency == currencyCode, ct);

            if (existingRate != null)
            {
                existingRate.Rate = rate;
                existingRate.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                dbContext.ExchangeRates.Add(new ExchangeRate
                {
                    FromCurrency = "USD",
                    ToCurrency = currencyCode,
                    Rate = rate,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            // Also update Currency entity's ExchangeRateToUSD
            var currency = await dbContext.Currencies
                .FirstOrDefaultAsync(c => c.Code == currencyCode, ct);
            if (currency != null)
            {
                currency.ExchangeRateToUSD = rate;
                currency.LastUpdated = DateTime.UtcNow;
            }
        }

        await dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Updated {Count} exchange rates from external API", rates.Count);
    }

    /// <summary>
    /// Fetches exchange rates from Frankfurter API (based on ECB data).
    /// Primary rate source - free, no API key required.
    /// </summary>
    private async Task<Dictionary<string, decimal>?> FetchFromFrankfurter(CancellationToken ct)
    {
        try
        {
            var response = await _httpClient.GetAsync("https://api.frankfurter.app/latest?base=USD", ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Frankfurter API returned status {StatusCode}", response.StatusCode);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync(ct);
            var doc = JsonDocument.Parse(json);

            var rates = new Dictionary<string, decimal>();
            if (doc.RootElement.TryGetProperty("rates", out var ratesElement))
            {
                foreach (var property in ratesElement.EnumerateObject())
                {
                    if (property.Value.TryGetDecimal(out var rate))
                    {
                        rates[property.Name] = rate;
                    }
                }
            }

            // Add USD = 1.0 (base currency)
            rates["USD"] = 1.0m;

            _logger.LogInformation("Fetched {Count} rates from Frankfurter API", rates.Count);
            return rates;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to fetch rates from Frankfurter API");
            return null;
        }
    }

    /// <summary>
    /// Fetches exchange rates from ExchangeRate.host API.
    /// Fallback rate source when Frankfurter is unavailable.
    /// </summary>
    private async Task<Dictionary<string, decimal>?> FetchFromExchangeRateHost(CancellationToken ct)
    {
        try
        {
            var response = await _httpClient.GetAsync("https://api.exchangerate.host/latest?base=USD", ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("ExchangeRate.host API returned status {StatusCode}", response.StatusCode);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync(ct);
            var doc = JsonDocument.Parse(json);

            var rates = new Dictionary<string, decimal>();
            if (doc.RootElement.TryGetProperty("rates", out var ratesElement))
            {
                foreach (var property in ratesElement.EnumerateObject())
                {
                    if (property.Value.TryGetDecimal(out var rate))
                    {
                        rates[property.Name] = rate;
                    }
                }
            }

            // Add USD = 1.0 (base currency)
            rates["USD"] = 1.0m;

            _logger.LogInformation("Fetched {Count} rates from ExchangeRate.host API", rates.Count);
            return rates;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to fetch rates from ExchangeRate.host API");
            return null;
        }
    }
}
