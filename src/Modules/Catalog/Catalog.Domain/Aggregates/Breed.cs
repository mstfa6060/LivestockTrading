namespace LivestockTrading.Catalog.Domain.Aggregates;

using LivestockTrading.Catalog.Domain.Events.Internal;
using LivestockTrading.Catalog.Domain.Events.Public;
using Shared.Domain;
using Shared.ValueObjects;

/// <summary>
/// Breed AR (sealed, AggregateRoot türevi). Doc 05-catalog §2 birebir. INT PK.
/// Factory-only: tek <see cref="Create"/> (Breed'de top-level/sub ayrımı yok).
/// code + categoryId IMMUTABLE (move yasak — yanlış kategori = deactivate + recreate, doc §2).
/// OriginCountryCode plain string? (doc-literal asimetri — Location'daki CountryCode VO değil).
/// BreedRenamed event YOK (doc §7 listelememiş — UpdateTranslations event yaymaz).
/// </summary>
public sealed class Breed : AggregateRoot
{
    public int Id { get; private set; }
    public string Code { get; private set; }                // kebab-case — IMMUTABLE (ctor-only)
    public int CategoryId { get; private set; }             // IMMUTABLE — move yasak (ctor-only)
    public string? OriginCountryCode { get; private set; }  // ISO 3166-1 alpha-2 (plain string)
    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }
    public Translations Name { get; private set; }
    public Translations? Description { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    // EF Core
    private Breed()
    {
        Code = null!;
        Name = null!;
    }

    // Factory'nin çağırdığı
    private Breed(
        string code,
        int categoryId,
        string? originCountryCode,
        Translations name,
        Translations? description,
        int displayOrder)
    {
        Code = code;
        CategoryId = categoryId;
        OriginCountryCode = originCountryCode;
        Name = name;
        Description = description;
        DisplayOrder = displayOrder;
        IsActive = true;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public static Breed Create(
        string code,
        Category category,
        Translations name,
        Translations? description = null,
        string? originCountryCode = null,
        int displayOrder = 0)
    {
        // Doc §2 invariant: parent category Level=2 zorunlu
        if (category.Level != 2)
            throw new DomainException("Breed parent category must be level-2 (subcategory).");

        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Breed code is required.");

        if (!name.TryGet("en", out _))
            throw new DomainException("Breed name must include 'en' locale.");

        if (originCountryCode != null && originCountryCode.Length != 2)
            throw new DomainException("OriginCountryCode must be ISO 3166-1 alpha-2 (2 characters).");

        var breed = new Breed(code, category.Id, originCountryCode, name, description, displayOrder);
        breed.Raise(new BreedCreated(0, code, category.Id));  // Id=0 transient, persist sonrası set olur
        return breed;
    }

    public void UpdateTranslations(Translations name, Translations? description)
    {
        if (!name.TryGet("en", out _))
            throw new DomainException("Breed name must include 'en' locale.");
        Name = name;
        Description = description;
        Touch();
        // Event YOK — doc §7 BreedRenamed listelememiş (Category ile asimetri, doc-literal)
    }

    public void UpdateDisplayOrder(int order)
    {
        DisplayOrder = order;
        Touch();
    }

    public void UpdateOriginCountryCode(string? originCountryCode)
    {
        if (originCountryCode != null && originCountryCode.Length != 2)
            throw new DomainException("OriginCountryCode must be ISO 3166-1 alpha-2 (2 characters).");
        OriginCountryCode = originCountryCode;
        Touch();
    }

    public void Activate()
    {
        if (IsActive) return;  // idempotent
        IsActive = true;
        Touch();
        Raise(new BreedReactivated(Id));
    }

    public void Deactivate()
    {
        if (!IsActive) return;  // idempotent
        IsActive = false;
        Touch();
        Raise(new BreedDeactivated(Id));
    }

    private void Touch() => UpdatedAt = DateTimeOffset.UtcNow;

    protected override object IdentityValue => Id;
    protected override bool IsTransient => Id == 0;
}
