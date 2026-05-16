namespace Shared.Contracts.Catalog.Admin;

using Shared.ValueObjects;

/// <summary>PUT: §10 CertType admin-owned (name/desc/display_order). code stable; is_active=Deactivate.</summary>
public sealed record UpdateCertificationTypeDto(
    Translations NameTranslations,
    Translations? DescriptionTranslations,
    int DisplayOrder);
