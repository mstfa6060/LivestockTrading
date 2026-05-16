namespace Shared.Domain;

/// <summary>Domain invariant ihlali. Factory / AR method'larında iş kuralı bozulunca atılır.</summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
    public DomainException(string message, Exception innerException) : base(message, innerException) { }
}
