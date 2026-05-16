namespace LivestockTrading.Catalog.Domain.Entities;

using Shared.Domain;
using Shared.ValueObjects;

/// <summary>
/// Category child entity (AR DEĞİL). Guid PK. Yalnız Category.AddAttribute tarafından
/// yaratılır (internal ctor). Doc 05-catalog §3 birebir. AttributeValueType enum dosya-içi
/// (LocationLevel emsali — Shared.Contracts dublike kabul, S1=(i) Contracts ref YOK).
/// </summary>
public sealed class CategoryAttribute : Entity
{
    public Guid Id { get; private set; }
    public int CategoryId { get; private set; }
    public string Key { get; private set; }
    public AttributeValueType ValueType { get; private set; }
    public bool Required { get; private set; }
    public bool Filterable { get; private set; }
    public string? Unit { get; private set; }
    public string? OptionsJson { get; private set; }
    public Translations Label { get; private set; }
    public Translations? HelpText { get; private set; }
    public int DisplayOrder { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    // EF Core
    private CategoryAttribute()
    {
        Key = null!;
        Label = null!;
    }

    // Sadece Category.AddAttribute çağırır (internal — modül dışına sızdırılmaz)
    internal CategoryAttribute(
        int categoryId,
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
        Id = Guid.NewGuid();
        CategoryId = categoryId;
        Key = key;  // null/empty guard'ı Category.AddAttribute içinde
        ValueType = valueType;
        Required = required;
        Filterable = filterable;
        Unit = unit;
        OptionsJson = optionsJson;
        Label = label;
        HelpText = helpText;
        DisplayOrder = displayOrder;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    protected override object IdentityValue => Id;
    protected override bool IsTransient => Id == Guid.Empty;
}

public enum AttributeValueType
{
    Text = 1,
    Number = 2,
    Boolean = 3,
    Enum = 4,
    Date = 5,
    File = 6
}
