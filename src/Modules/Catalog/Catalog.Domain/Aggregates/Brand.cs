namespace LivestockTrading.Catalog.Domain.Aggregates;

using LivestockTrading.Catalog.Domain.Entities;
using LivestockTrading.Catalog.Domain.Events.Internal;
using LivestockTrading.Catalog.Domain.Events.Public;
using Shared.Domain;
using Shared.Text;
using Shared.ValueObjects;

/// <summary>
/// Brand AR (sealed, AggregateRoot türevi). Doc 05-catalog §2 birebir. Guid PK (v7).
/// İki factory: SuggestBySeller (Suggested) / CreateByAdmin (Approved) — object initializer
/// pattern doc-literal. BrandStatus state machine: Suggested→Approved|Rejected(terminal),
/// Approved→Deactivated, Deactivated→Approved. Slug SlugHelper.Normalize, immutable sonrası.
/// BrandRenamed event YOK (doc §7 listelememiş — UpdateTranslations event yaymaz).
/// </summary>
public sealed class Brand : AggregateRoot
{
    public Guid Id { get; private set; }
    public string Slug { get; private set; }                // unique kebab-case, immutable after creation
    public Translations Name { get; private set; }
    public Translations? Description { get; private set; }
    public string? LogoUrl { get; private set; }
    public string? Website { get; private set; }
    public CountryCode? OriginCountry { get; private set; }

    public BrandStatus Status { get; private set; }
    public Guid? SuggestedByUserId { get; private set; }    // immutable after factory
    public DateTimeOffset? SuggestedAt { get; private set; } // immutable after factory
    public Guid? ReviewedByUserId { get; private set; }
    public DateTimeOffset? ReviewedAt { get; private set; }
    public string? RejectionReason { get; private set; }

    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private readonly List<BrandCategory> _categories = new();
    public IReadOnlyList<BrandCategory> Categories => _categories.AsReadOnly();

    // EF Core
    private Brand()
    {
        Slug = null!;
        Name = null!;
    }

    public static Brand SuggestBySeller(
        string slug,
        Translations name,
        Guid sellerUserId,
        IReadOnlyList<int> categoryIds)
    {
        if (string.IsNullOrWhiteSpace(slug))
            throw new DomainException("Brand slug is required.");
        if (!name.TryGet("en", out _))
            throw new DomainException("Brand name must include 'en' locale.");
        if (sellerUserId == Guid.Empty)
            throw new DomainException("SellerUserId is required for seller-suggested brand.");
        if (categoryIds == null || categoryIds.Count == 0)
            throw new DomainException("At least one category is required.");

        var normalizedSlug = SlugHelper.Normalize(slug);

        var brand = new Brand
        {
            Id = Guid.CreateVersion7(),
            Slug = normalizedSlug,
            Name = name,
            Status = BrandStatus.Suggested,
            SuggestedByUserId = sellerUserId,
            SuggestedAt = DateTimeOffset.UtcNow,
            IsActive = false,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };

        foreach (var cid in categoryIds)
            brand._categories.Add(new BrandCategory(brand.Id, cid));

        brand.Raise(new BrandSuggested(brand.Id, normalizedSlug, sellerUserId));
        return brand;
    }

    public static Brand CreateByAdmin(
        string slug,
        Translations name,
        Guid adminUserId,
        IReadOnlyList<int> categoryIds,
        string? logoUrl = null,
        string? website = null,
        CountryCode? originCountry = null,
        Translations? description = null,
        int displayOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(slug))
            throw new DomainException("Brand slug is required.");
        if (!name.TryGet("en", out _))
            throw new DomainException("Brand name must include 'en' locale.");
        if (adminUserId == Guid.Empty)
            throw new DomainException("AdminUserId is required.");
        if (categoryIds == null || categoryIds.Count == 0)
            throw new DomainException("At least one category is required.");

        var normalizedSlug = SlugHelper.Normalize(slug);
        var now = DateTimeOffset.UtcNow;

        var brand = new Brand
        {
            Id = Guid.CreateVersion7(),
            Slug = normalizedSlug,
            Name = name,
            Description = description,
            LogoUrl = logoUrl,
            Website = website,
            OriginCountry = originCountry,
            Status = BrandStatus.Approved,
            ReviewedByUserId = adminUserId,
            ReviewedAt = now,
            IsActive = true,
            DisplayOrder = displayOrder,
            CreatedAt = now,
            UpdatedAt = now,
        };

        foreach (var cid in categoryIds)
            brand._categories.Add(new BrandCategory(brand.Id, cid));

        brand.Raise(new BrandApproved(brand.Id, adminUserId, normalizedSlug));
        return brand;
    }

    public void Approve(Guid adminUserId)
    {
        if (Status != BrandStatus.Suggested)
            throw new DomainException($"Cannot approve brand in status {Status}. Must be Suggested.");
        if (adminUserId == Guid.Empty)
            throw new DomainException("AdminUserId is required.");

        Status = BrandStatus.Approved;
        IsActive = true;
        ReviewedByUserId = adminUserId;
        ReviewedAt = DateTimeOffset.UtcNow;
        Touch();
        Raise(new BrandApproved(Id, adminUserId, Slug));
    }

    public void Reject(Guid adminUserId, string reason)
    {
        if (Status != BrandStatus.Suggested)
            throw new DomainException($"Cannot reject brand in status {Status}. Must be Suggested.");
        if (adminUserId == Guid.Empty)
            throw new DomainException("AdminUserId is required.");
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Rejection reason is required.");

        Status = BrandStatus.Rejected;
        ReviewedByUserId = adminUserId;
        ReviewedAt = DateTimeOffset.UtcNow;
        RejectionReason = reason;
        Touch();
        Raise(new BrandRejected(Id, adminUserId, reason));
    }

    public void Deactivate(Guid adminUserId, string? reason = null)
    {
        if (Status != BrandStatus.Approved)
            throw new DomainException($"Cannot deactivate brand in status {Status}. Must be Approved.");
        if (adminUserId == Guid.Empty)
            throw new DomainException("AdminUserId is required.");

        Status = BrandStatus.Deactivated;
        IsActive = false;
        Touch();
        Raise(new BrandDeactivated(Id, adminUserId));
    }

    public void Reactivate(Guid adminUserId)
    {
        if (Status != BrandStatus.Deactivated)
            throw new DomainException($"Cannot reactivate brand in status {Status}. Must be Deactivated.");
        if (adminUserId == Guid.Empty)
            throw new DomainException("AdminUserId is required.");

        Status = BrandStatus.Approved;
        IsActive = true;
        Touch();
        Raise(new BrandReactivated(Id, adminUserId));
    }

    public void UpdateTranslations(Translations name, Translations? description)
    {
        if (!name.TryGet("en", out _))
            throw new DomainException("Brand name must include 'en' locale.");
        Name = name;
        Description = description;
        Touch();
        // Event YOK — doc §7 BrandRenamed listelememiş (Breed emsali, doc-literal)
    }

    public void UpdateLogo(string? logoUrl)
    {
        LogoUrl = logoUrl;
        Touch();
    }

    public void UpdateWebsite(string? website)
    {
        Website = website;
        Touch();
    }

    public void UpdateOriginCountry(CountryCode? originCountry)
    {
        OriginCountry = originCountry;
        Touch();
    }

    public void UpdateDisplayOrder(int order)
    {
        DisplayOrder = order;
        Touch();
    }

    public void AddCategory(int categoryId)
    {
        if (categoryId <= 0)
            throw new DomainException("CategoryId must be positive.");
        if (_categories.Any(c => c.CategoryId == categoryId))
            throw new DomainException($"Category {categoryId} already added to brand {Id}.");

        _categories.Add(new BrandCategory(Id, categoryId));
        Touch();
    }

    public void RemoveCategory(int categoryId)
    {
        var existing = _categories.FirstOrDefault(c => c.CategoryId == categoryId);
        if (existing == null)
            throw new DomainException($"Category {categoryId} not found in brand {Id}.");

        _categories.Remove(existing);
        Touch();
    }

    private void Touch() => UpdatedAt = DateTimeOffset.UtcNow;

    protected override object IdentityValue => Id;
    protected override bool IsTransient => Id == Guid.Empty;
}

public enum BrandStatus
{
    Suggested = 1,    // seller suggested, awaiting admin
    Approved = 2,
    Rejected = 3,
    Deactivated = 4
}
