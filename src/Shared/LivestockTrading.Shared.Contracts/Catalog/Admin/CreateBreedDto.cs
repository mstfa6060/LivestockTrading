namespace Shared.Contracts.Catalog.Admin;

using Shared.ValueObjects;

/// <summary>Breed.Create. CategoryCode (Code-only, server→int resolve); OriginCountryCode ISO 3166-1 alpha-2.</summary>
public sealed record CreateBreedDto(
    string Code,
    string CategoryCode,
    string? OriginCountryCode,
    Translations Name,
    Translations? Description,
    int DisplayOrder);
