namespace Shared.Contracts.Catalog;

using Shared.ValueObjects;

/// <summary>Kategori-spesifik dinamik attribute (CategoryDto.Attributes içinde). Kimlik = Key (UNIQUE(category_id,key)); Guid Id gizli.</summary>
public sealed record CategoryAttributeDto(
    string Key,
    AttributeValueType ValueType,
    bool Required,
    bool Filterable,
    string? Unit,
    string? OptionsJson,
    Translations Label,
    Translations? HelpText,
    int DisplayOrder);
