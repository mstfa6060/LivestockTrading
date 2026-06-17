namespace Livestock.Domain.Entities;

public class Faq : BaseEntity
{
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public string? Category { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public string? LanguageCode { get; set; }
}
