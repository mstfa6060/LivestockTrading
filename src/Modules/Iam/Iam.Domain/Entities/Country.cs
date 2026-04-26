namespace Iam.Domain.Entities;

public class Country
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string? Code3 { get; set; }
    public int? NumericCode { get; set; }
    public string? NativeName { get; set; }
    public string? PhoneCode { get; set; }
    public string CurrencyCode { get; set; } = default!;
    public string? CurrencySymbol { get; set; }
}
