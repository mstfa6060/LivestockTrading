namespace LivestockTrading.Catalog.Domain.Aggregates;

using Shared.Domain;
using Shared.ValueObjects;

/// <summary>
/// BorderRule AR (sealed, AggregateRoot türevi) — v2 YENİ, Faz 2 Feature. Doc 05-catalog
/// §2:223-272 birebir. Guid PK (v7). Tek Create factory. FromCountry/ToCountry CountryCode
/// VO non-nullable. RestrictionsJson Faz 1'de opaque (validation YOK; Faz 2 schema).
/// Faz 1: admin manuel CRUD, Listings sadece uyarı (block etmez). Event YOK Faz 1
/// (doc §7: BorderRuleCreated/Updated/Deactivated Faz 2 activate — tip dosyası yaratılmaz).
/// </summary>
public sealed class BorderRule : AggregateRoot
{
    public Guid Id { get; private set; }
    public CountryCode FromCountry { get; private set; }
    public CountryCode ToCountry { get; private set; }
    public int? CategoryId { get; private set; }                // null = all categories
    public int? BreedId { get; private set; }                   // breed-spesifik
    public BorderRuleKind Kind { get; private set; }
    public string RestrictionsJson { get; private set; }        // Faz 1 opaque, Faz 2 schema
    public Translations Notes { get; private set; }
    public DateTimeOffset? EffectiveFrom { get; private set; }
    public DateTimeOffset? EffectiveUntil { get; private set; }
    public bool IsActive { get; private set; }
    public Guid CreatedByUserId { get; private set; }           // immutable audit
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    // EF Core (FromCountry/ToCountry struct → default geçerli, CS8618 yok)
    private BorderRule()
    {
        RestrictionsJson = null!;
        Notes = null!;
    }

    // Factory'nin çağırdığı
    private BorderRule(
        CountryCode fromCountry,
        CountryCode toCountry,
        int? categoryId,
        int? breedId,
        BorderRuleKind kind,
        string restrictionsJson,
        Translations notes,
        DateTimeOffset? effectiveFrom,
        DateTimeOffset? effectiveUntil,
        Guid createdByUserId)
    {
        Id = Guid.CreateVersion7();
        FromCountry = fromCountry;
        ToCountry = toCountry;
        CategoryId = categoryId;
        BreedId = breedId;
        Kind = kind;
        RestrictionsJson = restrictionsJson;
        Notes = notes;
        EffectiveFrom = effectiveFrom;
        EffectiveUntil = effectiveUntil;
        IsActive = true;
        CreatedByUserId = createdByUserId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public static BorderRule Create(
        CountryCode fromCountry,
        CountryCode toCountry,
        BorderRuleKind kind,
        string restrictionsJson,
        Translations notes,
        Guid createdByUserId,
        int? categoryId = null,
        int? breedId = null,
        DateTimeOffset? effectiveFrom = null,
        DateTimeOffset? effectiveUntil = null)
    {
        if (string.IsNullOrWhiteSpace(restrictionsJson))
            throw new DomainException("RestrictionsJson is required (placeholder JSON allowed in Faz 1).");
        if (createdByUserId == Guid.Empty)
            throw new DomainException("CreatedByUserId is required.");
        if (!notes.TryGet("en", out _))
            throw new DomainException("BorderRule notes must include 'en' locale.");
        if (effectiveFrom.HasValue && effectiveUntil.HasValue && effectiveFrom >= effectiveUntil)
            throw new DomainException("EffectiveFrom must be earlier than EffectiveUntil.");
        if (fromCountry.Value == toCountry.Value)
            throw new DomainException("FromCountry and ToCountry cannot be the same.");

        return new BorderRule(fromCountry, toCountry, categoryId, breedId,
            kind, restrictionsJson, notes, effectiveFrom, effectiveUntil, createdByUserId);
        // Event YOK Faz 1 (BorderRuleCreated Faz 2'de aktive)
    }

    public void Update(
        BorderRuleKind kind,
        string restrictionsJson,
        Translations notes,
        DateTimeOffset? effectiveFrom,
        DateTimeOffset? effectiveUntil,
        Guid actorAdminId)
    {
        if (actorAdminId == Guid.Empty)
            throw new DomainException("ActorAdminId is required.");
        if (string.IsNullOrWhiteSpace(restrictionsJson))
            throw new DomainException("RestrictionsJson is required.");
        if (!notes.TryGet("en", out _))
            throw new DomainException("BorderRule notes must include 'en' locale.");
        if (effectiveFrom.HasValue && effectiveUntil.HasValue && effectiveFrom >= effectiveUntil)
            throw new DomainException("EffectiveFrom must be earlier than EffectiveUntil.");

        Kind = kind;
        RestrictionsJson = restrictionsJson;
        Notes = notes;
        EffectiveFrom = effectiveFrom;
        EffectiveUntil = effectiveUntil;
        Touch();
        // Event YOK Faz 1 (BorderRuleUpdated Faz 2'de aktive)
    }

    public void Deactivate(Guid actorAdminId)
    {
        if (actorAdminId == Guid.Empty)
            throw new DomainException("ActorAdminId is required.");
        if (!IsActive) return;  // idempotent

        IsActive = false;
        Touch();
        // Event YOK Faz 1 (BorderRuleDeactivated Faz 2'de aktive)
    }

    private void Touch() => UpdatedAt = DateTimeOffset.UtcNow;

    protected override object IdentityValue => Id;
    protected override bool IsTransient => Id == Guid.Empty;
}

public enum BorderRuleKind
{
    Banned = 1,                  // ihracat yasak
    RequiresCert = 2,
    RequiresQuarantine = 3,
    AdditionalFee = 4,
    QuantityLimit = 5
}
