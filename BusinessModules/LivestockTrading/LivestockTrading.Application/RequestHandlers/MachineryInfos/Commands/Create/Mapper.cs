using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.MachineryInfos.Commands.Create;

public class Mapper
{
	public MachineryInfo MapToEntity(RequestModel request)
	{
		return new MachineryInfo
		{
			Id = Guid.NewGuid(),
			ProductId = request.ProductId,
			Type = (MachineryType)request.Type,
			Model = request.Model,
			YearOfManufacture = request.YearOfManufacture,
			SerialNumber = request.SerialNumber,
			PowerSource = request.PowerSource,
			PowerHp = request.PowerHp,
			PowerKw = request.PowerKw,
			EngineCapacity = request.EngineCapacity,
			LengthCm = request.LengthCm,
			WidthCm = request.WidthCm,
			HeightCm = request.HeightCm,
			WeightKg = request.WeightKg,
			WorkingWidthCm = request.WorkingWidthCm,
			CapacityLiters = request.CapacityLiters,
			LoadCapacityKg = request.LoadCapacityKg,
			SpeedKmh = request.SpeedKmh,
			HoursUsed = request.HoursUsed,
			LastServiceDate = request.LastServiceDate,
			ServiceHistory = request.ServiceHistory,
			IncludedAttachments = request.IncludedAttachments,
			CompatibleAttachments = request.CompatibleAttachments,
			HasWarranty = request.HasWarranty,
			WarrantyExpiryDate = request.WarrantyExpiryDate,
			WarrantyDetails = request.WarrantyDetails,
			Certifications = request.Certifications,
			SafetyFeatures = request.SafetyFeatures,
			CreatedAt = DateTime.UtcNow
		};
	}

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
			CreatedAt = entity.CreatedAt
		};
	}
}
