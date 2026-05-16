namespace Shared.Contracts.Catalog.Admin;

using Shared.ValueObjects;

/// <summary>PUT: §10 Breed admin-owned (name/desc/display_order). code/category/origin stable; is_active=Deactivate.</summary>
public sealed record UpdateBreedDto(
    Translations Name,
    Translations? Description,
    int DisplayOrder);
