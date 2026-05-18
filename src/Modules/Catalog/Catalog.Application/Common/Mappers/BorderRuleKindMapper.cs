using LivestockTrading.Catalog.Domain.Aggregates;
using Shared.Domain;

namespace LivestockTrading.Catalog.Application.Common.Mappers;

/// <summary>
/// Contracts→Domain BorderRuleKind mapping (KAYDET-22: explicit switch, ham cast yasak).
/// 2. duplike-enum (AttributeValueType 1.) → repeated-twice-rule Common/Mappers/ extraction.
/// ToContracts reverse: read service W2.6 driver — şimdilik write-only (KAYDET-17).
/// </summary>
public static class BorderRuleKindMapper
{
    public static BorderRuleKind ToDomain(Shared.Contracts.Catalog.BorderRuleKind contractsKind)
        => contractsKind switch
        {
            Shared.Contracts.Catalog.BorderRuleKind.Banned => BorderRuleKind.Banned,
            Shared.Contracts.Catalog.BorderRuleKind.RequiresCert => BorderRuleKind.RequiresCert,
            Shared.Contracts.Catalog.BorderRuleKind.RequiresQuarantine => BorderRuleKind.RequiresQuarantine,
            Shared.Contracts.Catalog.BorderRuleKind.AdditionalFee => BorderRuleKind.AdditionalFee,
            Shared.Contracts.Catalog.BorderRuleKind.QuantityLimit => BorderRuleKind.QuantityLimit,
            _ => throw new DomainException($"Unknown BorderRuleKind: {contractsKind}")
        };
}
