namespace Shared.Contracts.Catalog.Admin;

using Shared.ValueObjects;

/// <summary>PUT: Location admin-editable (name/nativeName/population/display_order). code/path/level/country stable.</summary>
public sealed record UpdateLocationDto(
    Translations Name,
    Translations? NativeName,
    int Population,
    int DisplayOrder);
