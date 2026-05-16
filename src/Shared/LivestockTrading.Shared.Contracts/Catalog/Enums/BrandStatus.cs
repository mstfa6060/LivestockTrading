namespace Shared.Contracts.Catalog;

/// <summary>Marka yaşam döngüsü durumu (workflow state — IsActive soft-delete'ten ayrı).</summary>
public enum BrandStatus { Suggested = 1, Approved = 2, Rejected = 3, Deactivated = 4 }
