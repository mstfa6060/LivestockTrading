namespace LivestockTrading.Catalog.Domain.Entities;

using Shared.Domain;

/// <summary>
/// Brand ↔ Category many-to-many junction (Entity, AR DEĞİL). Doc 05-catalog §3 birebir.
/// PRIMARY KEY (brand_id, category_id) — composite, EF konfigürasyonu C3 Infra Faz 2.
/// Public ctor doc-literal (junction normalde Brand.AddCategory'den çağrılır, doc public).
/// Composite key → IdentityValue ValueTuple boxing (value-eq), Frontend kararı.
/// </summary>
public sealed class BrandCategory : Entity
{
    public Guid BrandId { get; private set; }
    public int CategoryId { get; private set; }

    // EF Core
    private BrandCategory() { }

    public BrandCategory(Guid brandId, int categoryId)
    {
        if (brandId == Guid.Empty)
            throw new DomainException("BrandId is required for BrandCategory junction.");
        if (categoryId <= 0)
            throw new DomainException("CategoryId must be positive for BrandCategory junction.");

        BrandId = brandId;
        CategoryId = categoryId;
    }

    // Composite key: hash/equality value-eq via ValueTuple boxing
    protected override object IdentityValue => (BrandId, CategoryId);
    protected override bool IsTransient => BrandId == Guid.Empty || CategoryId == 0;
}
