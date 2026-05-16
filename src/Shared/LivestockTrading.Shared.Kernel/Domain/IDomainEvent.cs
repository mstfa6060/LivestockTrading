namespace Shared.Domain;

/// <summary>Bir AR'da olmuş, domain açısından anlamlı olgu. Karar 3b event taban arabirimi.</summary>
public interface IDomainEvent
{
    DateTimeOffset OccurredOn { get; }
}
