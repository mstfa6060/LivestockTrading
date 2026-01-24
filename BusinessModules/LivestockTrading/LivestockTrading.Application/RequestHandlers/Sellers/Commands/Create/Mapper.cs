using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Sellers.Commands.Create;

public class Mapper
{
	public Seller MapToEntity(RequestModel request)
	{
		return new Seller
		{
			Id = Guid.NewGuid(),
			UserId = request.UserId,
			BusinessName = request.BusinessName,
			BusinessType = request.BusinessType,
			TaxNumber = request.TaxNumber,
			RegistrationNumber = request.RegistrationNumber,
			Description = request.Description,
			LogoUrl = request.LogoUrl,
			BannerUrl = request.BannerUrl,
			Email = request.Email,
			Phone = request.Phone,
			Website = request.Website,
			IsVerified = false,
			IsActive = request.IsActive,
			Status = (SellerStatus)request.Status,
			BusinessHours = request.BusinessHours,
			AcceptedPaymentMethods = request.AcceptedPaymentMethods,
			ReturnPolicy = request.ReturnPolicy,
			ShippingPolicy = request.ShippingPolicy,
			SocialMediaLinks = request.SocialMediaLinks,
			CreatedAt = DateTime.UtcNow
		};
	}

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
			IsActive = entity.IsActive,
			Status = (int)entity.Status,
			BusinessHours = entity.BusinessHours,
			AcceptedPaymentMethods = entity.AcceptedPaymentMethods,
			ReturnPolicy = entity.ReturnPolicy,
			ShippingPolicy = entity.ShippingPolicy,
			SocialMediaLinks = entity.SocialMediaLinks,
			CreatedAt = entity.CreatedAt
		};
	}
}
