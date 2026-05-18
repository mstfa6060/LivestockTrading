using Shared.Contracts.Catalog.Admin;

namespace LivestockTrading.Catalog.Application.Features.BorderRules;

/// <summary>
/// Command for updating a BorderRule AR (monolithic Update, W2.5-A).
/// Updates Kind/RestrictionsJson/Notes/dates; FromCountry/ToCountry stable.
/// </summary>
public sealed record UpdateBorderRuleCommand(
    Guid BorderRuleId,
    UpdateBorderRuleDto Dto,
    Guid ActorAdminId);
