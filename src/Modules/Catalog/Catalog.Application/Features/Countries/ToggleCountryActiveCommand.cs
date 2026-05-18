namespace LivestockTrading.Catalog.Application.Features.Countries;

/// <summary>
/// Command for toggling a Country reference entity active flag.
/// Active=true → Activate(); false → Deactivate(). Id-only + bool; no DTO body.
/// </summary>
public sealed record ToggleCountryActiveCommand(
    int CountryId,
    bool Active,
    Guid ActorAdminId);
