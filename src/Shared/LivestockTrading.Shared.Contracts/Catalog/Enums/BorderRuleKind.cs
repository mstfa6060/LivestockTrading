namespace Shared.Contracts.Catalog;

/// <summary>Gümrük geçiş kuralı tipi (§2:261-268). Faz 1 admin manuel, Faz 2 listing approval.</summary>
public enum BorderRuleKind { Banned = 1, RequiresCert = 2, RequiresQuarantine = 3, AdditionalFee = 4, QuantityLimit = 5 }
