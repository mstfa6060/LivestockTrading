using Shared.Contracts.Catalog.Admin;

namespace LivestockTrading.Catalog.Application.Features.BorderRules;

/// <summary>
/// Command for creating a BorderRule AR (admin manuel, Faz 1).
/// dto.Kind (Contracts) → Domain via BorderRuleKindMapper (KAYDET-22).
/// CategoryCode/BreedCode → server-side GetByCodeAsync int? resolve (Code-only DTO).
/// actorAdminId → BorderRule.Create createdByUserId.
/// </summary>
public sealed record CreateBorderRuleCommand(
    CreateBorderRuleDto Dto,
    Guid ActorAdminId);
