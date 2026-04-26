namespace Livestock.Domain.Entities;

public class ProductViewHistory : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
    public string? ViewSource { get; set; }

    public Product Product { get; set; } = null!;
}
