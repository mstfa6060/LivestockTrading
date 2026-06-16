namespace Livestock.Domain.Entities;

public class SearchHistory : BaseEntity
{
    public Guid UserId { get; set; }
    public string SearchQuery { get; set; } = string.Empty;
    public string? Filters { get; set; }
    public int ResultsCount { get; set; }
    public DateTime SearchedAt { get; set; } = DateTime.UtcNow;
}
