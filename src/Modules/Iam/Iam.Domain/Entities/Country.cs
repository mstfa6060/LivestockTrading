namespace Iam.Domain.Entities;

public class Country
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string CurrencyCode { get; set; } = default!;
    public string? CurrencySymbol { get; set; }
}
