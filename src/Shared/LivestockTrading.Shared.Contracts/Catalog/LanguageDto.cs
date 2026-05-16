namespace Shared.Contracts.Catalog;

/// <summary>ISO 639-1 referans. IsRtl: sağdan-sola diller (ar, he).</summary>
public sealed record LanguageDto(
    string Code,
    string NameEn,
    string NativeName,
    bool IsRtl,
    bool IsActive,
    int DisplayOrder);
