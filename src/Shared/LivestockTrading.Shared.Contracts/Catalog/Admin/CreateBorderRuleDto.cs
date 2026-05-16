namespace Shared.Contracts.Catalog.Admin;

using Shared.ValueObjects;
using Shared.Contracts.Catalog;

/// <summary>
/// BorderRule.Create §2:226-259 (Faz 1 admin manuel, impl C2 placeholder). Category/BreedCode Code-only
/// (server→int resolve); nullable = belirli kural (kategori/ırk) vs genel (tüm) ayrımı doc-fact.
/// </summary>
public sealed record CreateBorderRuleDto(
    string FromCountryCode,
    string ToCountryCode,
    string? CategoryCode,
    string? BreedCode,
    BorderRuleKind Kind,
    string RestrictionsJson,
    Translations Notes,
    DateTimeOffset? EffectiveFrom,
    DateTimeOffset? EffectiveUntil);
