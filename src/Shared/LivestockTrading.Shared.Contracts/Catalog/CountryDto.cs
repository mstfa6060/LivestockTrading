namespace Shared.Contracts.Catalog;

/// <summary>ISO 3166-1 referans. Dış kimlik Code (3e Bölüm 6); int Id gizli. NameEn/NativeName plain string (entity §4).</summary>
public sealed record CountryDto(
    string Code,
    string NameEn,
    string NativeName,
    string Region,
    string DefaultCurrencyCode,
    string DefaultLanguageCode,
    string PhonePrefix,
    bool IsActive,
    int DisplayOrder);
