namespace Livestock.Domain.Entities;

public class SellerReview : BaseEntity
{
    public Guid SellerId { get; set; }
    public Guid ReviewerUserId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public Guid? DealId { get; set; }

    public Seller Seller { get; set; } = null!;
}
