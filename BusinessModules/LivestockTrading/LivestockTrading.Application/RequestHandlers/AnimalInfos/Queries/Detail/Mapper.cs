using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.AnimalInfos.Queries.Detail;

public class Mapper
{
	public ResponseModel MapToResponse(AnimalInfo entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			ProductId = entity.ProductId,
			BreedName = entity.BreedName,
			Gender = (int)entity.Gender,
			DateOfBirth = entity.DateOfBirth,
			AgeMonths = entity.AgeMonths,
			WeightKg = entity.WeightKg,
			HeightCm = entity.HeightCm,
			Color = entity.Color,
			Markings = entity.Markings,
			TagNumber = entity.TagNumber,
			MicrochipNumber = entity.MicrochipNumber,
			PassportNumber = entity.PassportNumber,
			RegistrationNumber = entity.RegistrationNumber,
			HealthStatus = (int)entity.HealthStatus,
			LastHealthCheckDate = entity.LastHealthCheckDate,
			IsPregnant = entity.IsPregnant,
			ExpectedDueDate = entity.ExpectedDueDate,
			NumberOfBirths = entity.NumberOfBirths,
			DailyMilkProductionLiters = entity.DailyMilkProductionLiters,
			AverageDailyEggProduction = entity.AverageDailyEggProduction,
			SireDetails = entity.SireDetails,
			DamDetails = entity.DamDetails,
			Purpose = (int)entity.Purpose,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
