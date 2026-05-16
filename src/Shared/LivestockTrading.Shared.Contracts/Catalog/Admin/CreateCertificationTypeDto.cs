namespace Shared.Contracts.Catalog.Admin;

using Shared.ValueObjects;

/// <summary>CertificationType.Create §4:400-412. Code stable; NameTranslations zorunlu (en garanti).</summary>
public sealed record CreateCertificationTypeDto(
    string Code,
    Translations NameTranslations,
    Translations? DescriptionTranslations,
    int DisplayOrder);
