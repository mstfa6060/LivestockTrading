namespace Livestock.Features.Admin.Currencies;

public record CurrencyAdminItem(
    Guid Id,
    string Code,
    string Name,
    string Symbol,
    bool IsActive,
    decimal ExchangeRateToUsd,
    DateTime? LastUpdatedAt,
    DateTime CreatedAt);

public record CreateCurrencyRequest(string Code, string Name, string Symbol, decimal ExchangeRateToUsd);

public record UpdateCurrencyRequest(
    Guid Id,
    string Name,
    string Symbol,
    bool IsActive,
    decimal ExchangeRateToUsd);

public record GetCurrencyAdminRequest(Guid Id);
public record DeleteCurrencyRequest(Guid Id);
public record CurrencyConvertRequest(string From, string To, decimal Amount);
public record CurrencyConvertResponse(string From, string To, decimal Amount, decimal ConvertedAmount, decimal ExchangeRate);
public record CurrencyPublicItem(Guid Id, string Code, string Name, string Symbol, decimal ExchangeRateToUsd);
