namespace LivestockTrading.Catalog.Domain.Entities;

using Shared.Domain;

/// <summary>
/// Currency reference (ISO 4217). Entity türevi, AR DEĞİL. Doc 05-catalog §4 birebir.
/// </summary>
public sealed class Currency : Entity
{
    public int Id { get; private set; }
    public string Code { get; private set; }                    // ISO 4217 — IMMUTABLE
    public string NameEn { get; private set; }
    public string Symbol { get; private set; }                  // ₺, $, د.إ
    public char SymbolPosition { get; private set; }            // 'L' / 'R'
    public int DecimalPlaces { get; private set; }              // 0/2/3/4 per ISO 4217
    public char ThousandSep { get; private set; }
    public char DecimalSep { get; private set; }
    public bool IsActive { get; private set; }
    public decimal? RateToUsd { get; private set; }             // son güncellenen kur
    public DateTimeOffset? RateUpdatedAt { get; private set; }

    public Currency(
        string code,
        string nameEn,
        string symbol,
        char symbolPosition,
        int decimalPlaces,
        char thousandSep,
        char decimalSep)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Currency code is required.");
        if (string.IsNullOrWhiteSpace(nameEn))
            throw new DomainException("Currency nameEn is required.");
        if (string.IsNullOrWhiteSpace(symbol))
            throw new DomainException("Currency symbol is required.");

        Code = code;
        NameEn = nameEn;
        Symbol = symbol;
        SymbolPosition = symbolPosition;
        DecimalPlaces = decimalPlaces;
        ThousandSep = thousandSep;
        DecimalSep = decimalSep;
        IsActive = true;
    }

    public void UpdateRate(decimal newRate)
    {
        if (newRate <= 0)
            throw new DomainException("Currency rate must be positive.");
        RateToUsd = newRate;
        RateUpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    protected override object IdentityValue => Id;
    protected override bool IsTransient => Id == 0;
}
