namespace LivestockTrading.Catalog.Application.Features.Currencies;

/// <summary>
/// Command for toggling a Currency reference entity active flag.
/// Active=true → Activate(); false → Deactivate(). Id-only + bool; no DTO body.
/// </summary>
public sealed record ToggleCurrencyActiveCommand(
    int CurrencyId,
    bool Active,
    Guid ActorAdminId);
