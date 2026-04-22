namespace Livestock.Domain.Entities;

public class SearchHistory : BaseEntity
{
    public Guid UserId { get; set; }
    public string Query { get; set; } = string.Empty;
    public Guid? CategoryId { get; set; }
    public int ResultCount { get; set; }
}
