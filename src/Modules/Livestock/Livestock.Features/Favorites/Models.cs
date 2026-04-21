namespace Livestock.Features.Favorites;

public record FavoriteItem(Guid Id, Guid ProductId, string ProductTitle, string ProductSlug, decimal ProductPrice, string ProductCurrencyCode, DateTime CreatedAt);
public record AddFavoriteRequest(Guid ProductId);
public record RemoveFavoriteRequest(Guid ProductId);
