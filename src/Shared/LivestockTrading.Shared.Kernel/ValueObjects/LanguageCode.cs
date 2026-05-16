namespace Shared.ValueObjects;

using Shared.Domain;

/// <summary>
/// ISO 639-1 (lowercase 2 char). Format-validated; aktif-whitelist runtime Catalog.
/// default(LanguageCode) ctor'u atlar → Value getter fail-fast (Translations Dictionary-key güvencesi).
/// </summary>
public readonly record struct LanguageCode
{
    private readonly string? _value;

    public string Value => _value ?? throw new DomainException(
        "LanguageCode used without construction (default(LanguageCode)).");

    public LanguageCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length != 2
            || !char.IsAsciiLetter(value[0]) || !char.IsAsciiLetter(value[1]))
            throw new DomainException($"Invalid ISO 639-1 language code: '{value}'.");
        _value = value.ToLowerInvariant();
    }

    public override string ToString() => Value;
}
