namespace Shared.Contracts.Catalog.Admin;

using Shared.ValueObjects;

/// <summary>PUT: §10 Category admin-owned (name/desc/display_order/icon_key). code/parent stable; is_active=Deactivate; attribute=Add/Remove.</summary>
public sealed record UpdateCategoryDto(
    Translations Name,
    Translations? Description,
    int DisplayOrder,
    string? IconKey);
