namespace LivestockTrading.Catalog.Domain.Entities;

using Shared.Domain;
using Shared.ValueObjects;

/// <summary>
/// CertificationType reference. Entity türevi, AR DEĞİL.
/// SAPMA (Frontend Seç-2): doc §4 'NameTranslationsJson:string + Translations.Deserialize'
/// tasarımı Shared.Kernel Translations.cs:5 'Domain saf, JSONB = C3 Infra EF converter'
/// kararıyla çeliştiği için Location pattern'ine hizalandı — doğrudan Translations VO field,
/// JSON string YOK. (Sapma 36 adayı: 05-catalog.md:404-409 Wave 1 sonu docs commit'i.)
/// </summary>
public sealed class CertificationType : Entity
{
    public int Id { get; private set; }
    public string Code { get; private set; }                        // kebab-case — IMMUTABLE
    public Translations NameTranslations { get; private set; }
    public Translations DescriptionTranslations { get; private set; }
    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }

    public CertificationType(
        string code,
        Translations nameTranslations,
        Translations descriptionTranslations,
        int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("CertificationType code is required.");
        if (!nameTranslations.TryGet("en", out _))
            throw new DomainException("CertificationType name translations must include 'en' locale.");

        Code = code;
        NameTranslations = nameTranslations;
        DescriptionTranslations = descriptionTranslations;
        DisplayOrder = displayOrder;
        IsActive = true;
    }

    public void UpdateTranslations(Translations translations)
    {
        if (!translations.TryGet("en", out _))
            throw new DomainException("CertificationType translations must include 'en' locale.");
        NameTranslations = translations;
    }

    protected override object IdentityValue => Id;
    protected override bool IsTransient => Id == 0;
}
