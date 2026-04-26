namespace Shared.Abstractions.Results;

public sealed record Error(string Code, string Description)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NotFound = new("General.NotFound", "Resource not found.");
    public static readonly Error Unauthorized = new("General.Unauthorized", "Unauthorized.");
    public static readonly Error Forbidden = new("General.Forbidden", "Forbidden.");

    public static Error Validation(string code, string description) => new(code, description);
}
