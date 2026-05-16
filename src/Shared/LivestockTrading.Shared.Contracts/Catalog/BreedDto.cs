namespace Shared.Contracts.Catalog;

using Shared.ValueObjects;

/// <summary>Irk. CategoryCode (3e Code-only; entity CategoryId int). OriginCountryCode ISO 3166-1 alpha-2 string?.</summary>
public sealed record BreedDto(
    string Code,
    string CategoryCode,
    string? OriginCountryCode,
    bool IsActive,
    int DisplayOrder,
    Translations Name,
    Translations? Description);
