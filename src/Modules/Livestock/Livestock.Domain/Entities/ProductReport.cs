namespace Livestock.Domain.Entities;

public class ProductReport : BaseEntity
{
    public Guid ProductId { get; set; }
    public Guid ReporterUserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Details { get; set; }
    public bool IsResolved { get; set; }
    public string? Resolution { get; set; }
    public DateTime? ResolvedAt { get; set; }

    public Product Product { get; set; } = null!;
}
