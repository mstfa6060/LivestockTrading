using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.MachineryInfos.Queries.Detail;

public class Mapper
{
	public ResponseModel MapToResponse(MachineryInfo entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			ProductId = entity.ProductId,
			Type = (int)entity.Type,
			Model = entity.Model,
			YearOfManufacture = entity.YearOfManufacture,
			SerialNumber = entity.SerialNumber,
			PowerSource = entity.PowerSource,
			PowerHp = entity.PowerHp,
			PowerKw = entity.PowerKw,
			EngineCapacity = entity.EngineCapacity,
			LengthCm = entity.LengthCm,
			WidthCm = entity.WidthCm,
			HeightCm = entity.HeightCm,
			WeightKg = entity.WeightKg,
			WorkingWidthCm = entity.WorkingWidthCm,
			CapacityLiters = entity.CapacityLiters,
			LoadCapacityKg = entity.LoadCapacityKg,
			SpeedKmh = entity.SpeedKmh,
			HoursUsed = entity.HoursUsed,
			LastServiceDate = entity.LastServiceDate,
			ServiceHistory = entity.ServiceHistory,
			IncludedAttachments = entity.IncludedAttachments,
			CompatibleAttachments = entity.CompatibleAttachments,
			HasWarranty = entity.HasWarranty,
			WarrantyExpiryDate = entity.WarrantyExpiryDate,
			WarrantyDetails = entity.WarrantyDetails,
			Certifications = entity.Certifications,
			SafetyFeatures = entity.SafetyFeatures,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
