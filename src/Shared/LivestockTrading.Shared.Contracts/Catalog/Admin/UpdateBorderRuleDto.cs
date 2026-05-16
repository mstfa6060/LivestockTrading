namespace Shared.Contracts.Catalog.Admin;

using Shared.ValueObjects;
using Shared.Contracts.Catalog;

/// <summary>PUT: BorderRule.UpdateRestrictions + notes/dates/kind (Faz 2 impl). from/to country stable.</summary>
public sealed record UpdateBorderRuleDto(
    BorderRuleKind Kind,
    string RestrictionsJson,
    Translations Notes,
    DateTimeOffset? EffectiveFrom,
    DateTimeOffset? EffectiveUntil);
