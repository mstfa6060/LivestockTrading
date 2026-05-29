namespace LivestockTrading.Identity.Domain.ValueObjects;
using Shared.Domain;

/// <summary>First + Last + opsiyonel Middle. Non-empty, max 50.</summary>
public sealed record PersonName
{
    public string First { get; }
    public string Last { get; }
    public string? Middle { get; }

    public PersonName(string first, string last, string? middle = null)
    {
        Validate(first, nameof(first));
        Validate(last, nameof(last));
        if (middle is not null) ValidateLength(middle, nameof(middle));
        First = first.Trim();
        Last = last.Trim();
        Middle = middle?.Trim();
    }

    private static void Validate(string value, string field)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException($"{field} bos olamaz.");
        ValidateLength(value, field);
    }

    private static void ValidateLength(string value, string field)
    {
        if (value.Trim().Length > 50)
            throw new DomainException($"{field} 50 karakteri asamaz.");
    }

    public string FullName => string.IsNullOrEmpty(Middle) ? $"{First} {Last}" : $"{First} {Middle} {Last}";

    public override string ToString() => FullName;
}
