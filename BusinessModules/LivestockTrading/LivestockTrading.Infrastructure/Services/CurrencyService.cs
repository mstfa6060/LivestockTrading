using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;
using Common.Services.Caching;
using Arfware.ArfBlocks.Core;

namespace LivestockTrading.Infrastructure.Services;

public interface ICurrencyService
{
    Task<decimal> Convert(decimal amount, string fromCurrency, string toCurrency);
    Task<ExchangeRate> GetRate(string fromCurrency, string toCurrency);
    Task<List<Currency>> GetAll();
}

public class CurrencyService : ICurrencyService
{
    private readonly LivestockTradingModuleDbContext _dbContext;
    private readonly ICacheService _cacheService;
    private const string CachePrefix = "currency:";

    public CurrencyService(ArfBlocksDependencyProvider dependencyProvider)
    {
        _dbContext = dependencyProvider.GetInstance<LivestockTradingModuleDbContext>();
        _cacheService = dependencyProvider.GetInstance<ICacheService>();
    }

    public async Task<decimal> Convert(decimal amount, string fromCurrency, string toCurrency)
    {
        if (string.Equals(fromCurrency, toCurrency, StringComparison.OrdinalIgnoreCase))
            return amount;

        var rate = await GetRate(fromCurrency, toCurrency);
        if (rate == null)
            return amount;

        return amount * rate.Rate;
    }

    public async Task<ExchangeRate> GetRate(string fromCurrency, string toCurrency)
    {
        if (string.Equals(fromCurrency, toCurrency, StringComparison.OrdinalIgnoreCase))
            return new ExchangeRate { FromCurrency = fromCurrency, ToCurrency = toCurrency, Rate = 1m };

        var cacheKey = $"{CachePrefix}rate:{fromCurrency}:{toCurrency}";

        // Try direct rate
        var directRate = await _dbContext.ExchangeRates
            .AsNoTracking()
            .Where(r => r.FromCurrency == fromCurrency && r.ToCurrency == toCurrency)
            .OrderByDescending(r => r.UpdatedAt)
            .FirstOrDefaultAsync();

        if (directRate != null)
            return directRate;

        // Try via USD pivot: from -> USD -> to
        var fromToUsd = await _dbContext.ExchangeRates
            .AsNoTracking()
            .Where(r => r.FromCurrency == fromCurrency && r.ToCurrency == "USD")
            .OrderByDescending(r => r.UpdatedAt)
            .FirstOrDefaultAsync();

        var usdToTarget = await _dbContext.ExchangeRates
            .AsNoTracking()
            .Where(r => r.FromCurrency == "USD" && r.ToCurrency == toCurrency)
            .OrderByDescending(r => r.UpdatedAt)
            .FirstOrDefaultAsync();

        if (fromToUsd != null && usdToTarget != null)
        {
            return new ExchangeRate
            {
                FromCurrency = fromCurrency,
                ToCurrency = toCurrency,
                Rate = fromToUsd.Rate * usdToTarget.Rate,
                UpdatedAt = fromToUsd.UpdatedAt < usdToTarget.UpdatedAt ? fromToUsd.UpdatedAt : usdToTarget.UpdatedAt
            };
        }

        return null;
    }

    public async Task<List<Currency>> GetAll()
    {
        return await _dbContext.Currencies
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.Code)
            .ToListAsync();
    }
}
