using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Sellers.Queries.Detail;

public class Mapper
{
	public ResponseModel MapToResponse(Seller entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			UserId = entity.UserId,
			BusinessName = entity.BusinessName,
			BusinessType = entity.BusinessType,
			TaxNumber = entity.TaxNumber,
			RegistrationNumber = entity.RegistrationNumber,
			Description = entity.Description,
			LogoUrl = entity.LogoUrl,
			BannerUrl = entity.BannerUrl,
			Email = entity.Email,
			Phone = entity.Phone,
			Website = entity.Website,
			IsVerified = entity.IsVerified,
			VerifiedAt = entity.VerifiedAt,
			IsActive = entity.IsActive,
			Status = (int)entity.Status,
			AverageRating = entity.AverageRating,
			TotalReviews = entity.TotalReviews,
			TotalSales = entity.TotalSales,
			TotalRevenue = entity.TotalRevenue,
			BusinessHours = entity.BusinessHours,
			AcceptedPaymentMethods = entity.AcceptedPaymentMethods,
			ReturnPolicy = entity.ReturnPolicy,
			ShippingPolicy = entity.ShippingPolicy,
			SocialMediaLinks = entity.SocialMediaLinks,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
