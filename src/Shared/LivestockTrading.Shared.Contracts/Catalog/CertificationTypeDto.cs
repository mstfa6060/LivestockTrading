namespace Shared.Contracts.Catalog;

using Shared.ValueObjects;

/// <summary>Sertifika tipi. Name/Description multi-locale Translations (entity NameTranslationsJson §4:400-412).</summary>
public sealed record CertificationTypeDto(
    string Code,
    Translations NameTranslations,
    Translations? DescriptionTranslations,
    bool IsActive,
    int DisplayOrder);
