namespace Common.Definitions.Base.Entity;

public abstract class CoreEntity : CoreEntity<Guid>
{

}

public abstract class CoreEntity<TKey> where TKey : IEquatable<TKey>
{
    public TKey Id { get; set; } = typeof(TKey) == typeof(Guid) ? (TKey)(object)Guid.NewGuid() : default;
}
