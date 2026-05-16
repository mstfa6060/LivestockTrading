namespace Shared.Domain;

/// <summary>
/// Identity-eşitlikli domain entity tabanı. AggregateRoot ve child entity'ler
/// (CategoryAttribute, BrandCategory) bundan türer. Non-generic — her AR kendi
/// Id tipini declare eder (Catalog INT/Guid karışık PK).
/// </summary>
public abstract class Entity
{
    /// <summary>Eşitlik için kimlik bileşeni (boxed). Derived typed Id'sini sağlar.</summary>
    protected abstract object IdentityValue { get; }

    /// <summary>Id henüz atanmamış (INT PK = 0, Guid.Empty). İki transient entity asla eşit değil.</summary>
    protected abstract bool IsTransient { get; }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity other || GetType() != other.GetType()) return false;
        if (ReferenceEquals(this, other)) return true;
        if (IsTransient || other.IsTransient) return false;
        return IdentityValue.Equals(other.IdentityValue);
    }

    public override int GetHashCode()
        => IsTransient ? base.GetHashCode() : HashCode.Combine(GetType(), IdentityValue);

    public static bool operator ==(Entity? left, Entity? right)
        => left is null ? right is null : left.Equals(right);

    public static bool operator !=(Entity? left, Entity? right) => !(left == right);
}
