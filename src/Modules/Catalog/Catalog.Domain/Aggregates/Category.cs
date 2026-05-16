namespace LivestockTrading.Catalog.Domain.Aggregates;

using LivestockTrading.Catalog.Domain.Entities;
using LivestockTrading.Catalog.Domain.Events.Internal;
using LivestockTrading.Catalog.Domain.Events.Public;
using Shared.Domain;
using Shared.ValueObjects;

/// <summary>
/// Category AR (sealed, AggregateRoot türevi). Doc 05-catalog §2 birebir.
/// Factory-only (S3=A): CreateTopLevel / CreateSubcategory. code + parent_id + level
/// IMMUTABLE (ctor-only — move yasak, doc §2 invariant). Tree depth max 2.
/// CategoryMoved event YOK (doc §7 ↔ §2 çelişkisi; §2 invariant baskın — Sapma 37 adayı).
/// Translations doğrudan VO field (C1.1 emsali, Shared.Kernel JSONB=C3 Infra kararıyla uyumlu).
/// </summary>
public sealed class Category : AggregateRoot
{
    public int Id { get; private set; }
    public string Code { get; private set; }                // IMMUTABLE — ctor-only
    public int? ParentId { get; private set; }              // IMMUTABLE — ctor-only (move yasak)
    public int Level { get; private set; }                  // IMMUTABLE — 1 veya 2
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; }
    public string? IconKey { get; private set; }
    public Translations Name { get; private set; }
    public Translations? Description { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private readonly List<CategoryAttribute> _attributes = new();
    public IReadOnlyList<CategoryAttribute> Attributes => _attributes.AsReadOnly();

    // EF Core
    private Category()
    {
        Code = null!;
        Name = null!;
    }

    // Factory'lerin çağırdığı
    private Category(
        string code,
        int? parentId,
        int level,
        Translations name,
        Translations? description,
        string? iconKey,
        int displayOrder)
    {
        Code = code;
        ParentId = parentId;
        Level = level;
        Name = name;
        Description = description;
        IconKey = iconKey;
        DisplayOrder = displayOrder;
    }

    public static Category CreateTopLevel(
        string code,
        Translations name,
        Translations? description = null,
        string? iconKey = null,
        int displayOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Category code is required.");
        if (!name.TryGet("en", out _))
            throw new DomainException("Category name must include 'en' locale.");

        var category = new Category(code, null, level: 1, name, description, iconKey, displayOrder);
        category.IsActive = true;
        category.CreatedAt = category.UpdatedAt = DateTimeOffset.UtcNow;
        category.Raise(new CategoryCreated(0, code, 1, null));  // Id 0 — EF persist sonrası set olur
        return category;
    }

    public static Category CreateSubcategory(
        string code,
        Category parent,
        Translations name,
        Translations? description = null,
        string? iconKey = null,
        int displayOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Category code is required.");
        if (!name.TryGet("en", out _))
            throw new DomainException("Category name must include 'en' locale.");

        // Doc §2 invariant: tree depth max 2
        if (parent.Level != 1)
            throw new DomainException("Subcategory parent must be top-level (level 1).");

        var category = new Category(code, parent.Id, level: 2, name, description, iconKey, displayOrder);
        category.IsActive = true;
        category.CreatedAt = category.UpdatedAt = DateTimeOffset.UtcNow;
        category.Raise(new CategoryCreated(0, code, 2, parent.Id));
        return category;
    }

    public void UpdateTranslations(Translations name, Translations? description)
    {
        if (!name.TryGet("en", out _))
            throw new DomainException("Category name must include 'en' locale.");
        Name = name;
        Description = description;
        Touch();
        Raise(new CategoryRenamed(Id));
    }

    public void UpdateDisplayOrder(int order)
    {
        DisplayOrder = order;
        Touch();
    }

    public void UpdateIconKey(string? iconKey)
    {
        IconKey = iconKey;
        Touch();
    }

    public void Activate()
    {
        if (IsActive) return;  // idempotent
        IsActive = true;
        Touch();
        Raise(new CategoryReactivated(Id));
    }

    public void Deactivate()
    {
        if (!IsActive) return;  // idempotent
        IsActive = false;
        Touch();
        Raise(new CategoryDeactivated(Id));
    }

    public CategoryAttribute AddAttribute(
        string key,
        AttributeValueType valueType,
        bool required,
        bool filterable,
        string? unit,
        string? optionsJson,
        Translations label,
        Translations? helpText,
        int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new DomainException("Attribute key is required.");
        if (_attributes.Any(a => a.Key == key))
            throw new DomainException($"Attribute key '{key}' already exists in category {Id}.");
        if (valueType == AttributeValueType.Enum && string.IsNullOrWhiteSpace(optionsJson))
            throw new DomainException("Enum attribute requires options JSON.");
        if (!label.TryGet("en", out _))
            throw new DomainException("Attribute label must include 'en' locale.");

        var attr = new CategoryAttribute(Id, key, valueType, required, filterable,
            unit, optionsJson, label, helpText, displayOrder);
        _attributes.Add(attr);
        Touch();
        return attr;
    }

    public void RemoveAttribute(Guid attributeId)
    {
        var attr = _attributes.FirstOrDefault(a => a.Id == attributeId);
        if (attr == null)
            throw new DomainException($"Attribute {attributeId} not found in category {Id}.");
        _attributes.Remove(attr);
        Touch();
    }

    // UpdateAttribute: doc §2 yalnız placeholder gösteriyor — Faz 1'de implement EDİLMİYOR
    // (Frontend kararı; kafadan iç implementasyon yazma — Sapma 30). Faz 2 admin UI'da eklenir.

    private void Touch() => UpdatedAt = DateTimeOffset.UtcNow;

    protected override object IdentityValue => Id;
    protected override bool IsTransient => Id == 0;
}
