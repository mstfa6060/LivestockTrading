namespace LivestockTrading.Application.RequestHandlers.Sellers.Queries.Nearby;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<(SellerLocation Seller, double DistanceKm)> items)
	{
		return items.Select(x => new ResponseModel
		{
			Id = x.Seller.SellerId,
			UserId = x.Seller.UserId,
			BusinessName = x.Seller.BusinessName,
			BusinessType = x.Seller.BusinessType,
			LogoUrl = x.Seller.LogoUrl,
			IsVerified = x.Seller.IsVerified,
			Status = x.Seller.Status,
			AverageRating = x.Seller.AverageRating,
			TotalReviews = x.Seller.TotalReviews,
			TotalSales = x.Seller.TotalSales,
			City = x.Seller.City,
			CountryCode = x.Seller.CountryCode,
			Latitude = x.Seller.Latitude,
			Longitude = x.Seller.Longitude,
			DistanceKm = Math.Round(x.DistanceKm, 1)
		}).ToList();
	}
}
