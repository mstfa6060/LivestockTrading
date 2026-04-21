using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.MachineryInfos.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, MachineryInfo entity)
	{
		entity.ProductId = request.ProductId;
		entity.Type = (MachineryType)request.Type;
		entity.Model = request.Model;
		entity.YearOfManufacture = request.YearOfManufacture;
		entity.SerialNumber = request.SerialNumber;
		entity.PowerSource = request.PowerSource;
		entity.PowerHp = request.PowerHp;
		entity.PowerKw = request.PowerKw;
		entity.EngineCapacity = request.EngineCapacity;
		entity.LengthCm = request.LengthCm;
		entity.WidthCm = request.WidthCm;
		entity.HeightCm = request.HeightCm;
		entity.WeightKg = request.WeightKg;
		entity.WorkingWidthCm = request.WorkingWidthCm;
		entity.CapacityLiters = request.CapacityLiters;
		entity.LoadCapacityKg = request.LoadCapacityKg;
		entity.SpeedKmh = request.SpeedKmh;
		entity.HoursUsed = request.HoursUsed;
		entity.LastServiceDate = request.LastServiceDate;
		entity.ServiceHistory = request.ServiceHistory;
		entity.IncludedAttachments = request.IncludedAttachments;
		entity.CompatibleAttachments = request.CompatibleAttachments;
		entity.HasWarranty = request.HasWarranty;
		entity.WarrantyExpiryDate = request.WarrantyExpiryDate;
		entity.WarrantyDetails = request.WarrantyDetails;
		entity.Certifications = request.Certifications;
		entity.SafetyFeatures = request.SafetyFeatures;
		entity.UpdatedAt = DateTime.UtcNow;
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
			UpdatedAt = entity.UpdatedAt
		};
	}
}
