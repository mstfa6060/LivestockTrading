namespace LivestockTrading.Catalog.Domain.Entities;

using Shared.Domain;

/// <summary>
/// Language reference (ISO 639-1). Entity türevi, AR DEĞİL. Doc 05-catalog §4 birebir
/// (behavior method yok — doc §4 göstermiyor).
/// </summary>
public sealed class Language : Entity
{
    public int Id { get; private set; }
    public string Code { get; private set; }                    // ISO 639-1
    public string NameEn { get; private set; }
    public string NativeName { get; private set; }
    public bool IsRtl { get; private set; }
    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }

    public Language(
        string code,
        string nameEn,
        string nativeName,
        bool isRtl,
        int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Language code is required.");
        if (string.IsNullOrWhiteSpace(nameEn))
            throw new DomainException("Language nameEn is required.");
        if (string.IsNullOrWhiteSpace(nativeName))
            throw new DomainException("Language nativeName is required.");

        Code = code;
        NameEn = nameEn;
        NativeName = nativeName;
        IsRtl = isRtl;
        DisplayOrder = displayOrder;
        IsActive = true;
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
