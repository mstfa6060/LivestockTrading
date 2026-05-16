namespace LivestockTrading.Catalog.Domain.Entities;

using Shared.Domain;

/// <summary>
/// Country reference (ISO 3166-1 alpha-2). Entity türevi, AR DEĞİL — cross-module event yaymaz.
/// Doc 05-catalog §4 birebir. UpdateStableFieldsFromSeed Wave 4c seed loader'a ertelendi.
/// </summary>
public sealed class Country : Entity
{
    public int Id { get; private set; }
    public string Code { get; private set; }                    // ISO 3166-1 alpha-2 — IMMUTABLE
    public string NameEn { get; private set; }
    public string NativeName { get; private set; }
    public string Region { get; private set; }                  // "MENA", "EU", "CIS"
    public string DefaultCurrencyCode { get; private set; }     // ISO 4217
    public string DefaultLanguageCode { get; private set; }     // ISO 639-1
    public string PhonePrefix { get; private set; }
    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public Country(
        string code,
        string nameEn,
        string nativeName,
        string region,
        string defaultCurrencyCode,
        string defaultLanguageCode,
        string phonePrefix,
        int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Country code is required.");
        if (string.IsNullOrWhiteSpace(nameEn))
            throw new DomainException("Country nameEn is required.");
        if (string.IsNullOrWhiteSpace(nativeName))
            throw new DomainException("Country nativeName is required.");
        if (string.IsNullOrWhiteSpace(region))
            throw new DomainException("Country region is required.");
        if (string.IsNullOrWhiteSpace(defaultCurrencyCode))
            throw new DomainException("Country defaultCurrencyCode is required.");
        if (string.IsNullOrWhiteSpace(defaultLanguageCode))
            throw new DomainException("Country defaultLanguageCode is required.");

        Code = code;
        NameEn = nameEn;
        NativeName = nativeName;
        Region = region;
        DefaultCurrencyCode = defaultCurrencyCode;
        DefaultLanguageCode = defaultLanguageCode;
        PhonePrefix = phonePrefix;
        DisplayOrder = displayOrder;
        IsActive = true;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateDisplayOrder(int order)
    {
        DisplayOrder = order;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateNativeName(string name)
    {
        NativeName = name;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    protected override object IdentityValue => Id;
    protected override bool IsTransient => Id == 0;
}
