namespace Shared.Contracts.Catalog.Admin;

using Shared.ValueObjects;
using Shared.Contracts.Catalog;

/// <summary>AddCategoryAttributeAsync payload (Update yok, doc-literal §5:549-551). CategoryId = metot param; Guid Id server. CategoryAttribute §3:281-301.</summary>
public sealed record AttributeDto(
    string Key,
    AttributeValueType ValueType,
    bool Required,
    bool Filterable,
    string? Unit,
    string? OptionsJson,
    Translations Label,
    Translations? HelpText,
    int DisplayOrder);
