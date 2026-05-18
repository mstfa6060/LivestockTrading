namespace LivestockTrading.Catalog.Application.Features.Languages;

/// <summary>
/// Command for toggling a Language reference entity active flag.
/// Active=true → Activate(); false → Deactivate(). Id-only + bool; no DTO body.
/// </summary>
public sealed record ToggleLanguageActiveCommand(
    int LanguageId,
    bool Active,
    Guid ActorAdminId);
