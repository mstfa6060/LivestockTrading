using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Sellers.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, Seller entity)
	{
		entity.UserId = request.UserId;
		entity.BusinessName = request.BusinessName;
		entity.BusinessType = request.BusinessType;
		entity.TaxNumber = request.TaxNumber;
		entity.RegistrationNumber = request.RegistrationNumber;
		entity.Description = request.Description;
		entity.LogoUrl = request.LogoUrl;
		entity.BannerUrl = request.BannerUrl;
		entity.Email = request.Email;
		entity.Phone = request.Phone;
		entity.Website = request.Website;
		entity.IsVerified = request.IsVerified;
		entity.IsActive = request.IsActive;
		entity.Status = (SellerStatus)request.Status;
		entity.BusinessHours = request.BusinessHours;
		entity.AcceptedPaymentMethods = request.AcceptedPaymentMethods;
		entity.ReturnPolicy = request.ReturnPolicy;
		entity.ShippingPolicy = request.ShippingPolicy;
		entity.SocialMediaLinks = request.SocialMediaLinks;
		entity.UpdatedAt = DateTime.UtcNow;
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
			UpdatedAt = entity.UpdatedAt
		};
	}
}
