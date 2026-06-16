namespace Livestock.Domain.Entities;

public class ProductReview : BaseEntity
{
    public Guid ProductId { get; set; }
    public Guid ReviewerUserId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public bool IsVerifiedPurchase { get; set; }

    public Product Product { get; set; } = null!;
}
