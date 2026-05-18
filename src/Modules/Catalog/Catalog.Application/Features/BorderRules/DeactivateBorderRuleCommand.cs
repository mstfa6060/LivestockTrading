namespace LivestockTrading.Catalog.Application.Features.BorderRules;

/// <summary>
/// Command for deactivating a BorderRule AR (idempotent soft toggle, Wave 1 Deactivate).
/// Id-only; no DTO body.
/// </summary>
public sealed record DeactivateBorderRuleCommand(
    Guid BorderRuleId,
    Guid ActorAdminId);
