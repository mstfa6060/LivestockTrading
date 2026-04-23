using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.AnimalInfos.Commands.Create;

public class Mapper
{
	public AnimalInfo MapToEntity(RequestModel request)
	{
		return new AnimalInfo
		{
			Id = Guid.NewGuid(),
			ProductId = request.ProductId,
			BreedName = request.BreedName,
			Gender = (AnimalGender)request.Gender,
			DateOfBirth = request.DateOfBirth,
			AgeMonths = request.AgeMonths,
			WeightKg = request.WeightKg,
			HeightCm = request.HeightCm,
			Color = request.Color,
			Markings = request.Markings,
			TagNumber = request.TagNumber,
			MicrochipNumber = request.MicrochipNumber,
			PassportNumber = request.PassportNumber,
			RegistrationNumber = request.RegistrationNumber,
			HealthStatus = (HealthStatus)request.HealthStatus,
			LastHealthCheckDate = request.LastHealthCheckDate,
			IsPregnant = request.IsPregnant,
			ExpectedDueDate = request.ExpectedDueDate,
			NumberOfBirths = request.NumberOfBirths,
			DailyMilkProductionLiters = request.DailyMilkProductionLiters,
			AverageDailyEggProduction = request.AverageDailyEggProduction,
			SireDetails = request.SireDetails,
			DamDetails = request.DamDetails,
			Purpose = (AnimalPurpose)request.Purpose,
			CreatedAt = DateTime.UtcNow
		};
	}

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
			CreatedAt = entity.CreatedAt
		};
	}
}
