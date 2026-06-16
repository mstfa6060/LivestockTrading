namespace Livestock.Domain.Entities;

public class FavoriteProduct : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }

    public Product Product { get; set; } = null!;
}
