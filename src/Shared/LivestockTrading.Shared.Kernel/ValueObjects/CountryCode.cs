namespace Shared.ValueObjects;

using Shared.Domain;

/// <summary>
/// ISO 3166-1 alpha-2 (uppercase 2 char). Format-validated; aktif-whitelist runtime Catalog (3e Bölüm 6).
/// default(CountryCode) ctor'u atlar → Value getter fail-fast (EF/value-converter güvencesi).
/// </summary>
public readonly record struct CountryCode
{
    private readonly string? _value;

    public string Value => _value ?? throw new DomainException(
        "CountryCode used without construction (default(CountryCode)).");

    public CountryCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length != 2
            || !char.IsAsciiLetter(value[0]) || !char.IsAsciiLetter(value[1]))
            throw new DomainException($"Invalid ISO 3166-1 alpha-2 country code: '{value}'.");
        _value = value.ToUpperInvariant();
    }

    public override string ToString() => Value;
}
