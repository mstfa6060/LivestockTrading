namespace Shared.Contracts.Catalog;

/// <summary>ISO 4217 referans. RateToUsd/RateUpdatedAt nullable (kur henüz çekilmemiş olabilir).</summary>
public sealed record CurrencyDto(
    string Code,
    string NameEn,
    string Symbol,
    char SymbolPosition,
    int DecimalPlaces,
    char ThousandSep,
    char DecimalSep,
    bool IsActive,
    decimal? RateToUsd,
    DateTimeOffset? RateUpdatedAt);
