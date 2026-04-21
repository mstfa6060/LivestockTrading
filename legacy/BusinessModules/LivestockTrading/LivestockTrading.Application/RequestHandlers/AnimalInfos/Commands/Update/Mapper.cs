using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.AnimalInfos.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, AnimalInfo entity)
	{
		entity.ProductId = request.ProductId;
		entity.BreedName = request.BreedName;
		entity.Gender = (AnimalGender)request.Gender;
		entity.DateOfBirth = request.DateOfBirth;
		entity.AgeMonths = request.AgeMonths;
		entity.WeightKg = request.WeightKg;
		entity.HeightCm = request.HeightCm;
		entity.Color = request.Color;
		entity.Markings = request.Markings;
		entity.TagNumber = request.TagNumber;
		entity.MicrochipNumber = request.MicrochipNumber;
		entity.PassportNumber = request.PassportNumber;
		entity.RegistrationNumber = request.RegistrationNumber;
		entity.HealthStatus = (HealthStatus)request.HealthStatus;
		entity.LastHealthCheckDate = request.LastHealthCheckDate;
		entity.IsPregnant = request.IsPregnant;
		entity.ExpectedDueDate = request.ExpectedDueDate;
		entity.NumberOfBirths = request.NumberOfBirths;
		entity.DailyMilkProductionLiters = request.DailyMilkProductionLiters;
		entity.AverageDailyEggProduction = request.AverageDailyEggProduction;
		entity.SireDetails = request.SireDetails;
		entity.DamDetails = request.DamDetails;
		entity.Purpose = (AnimalPurpose)request.Purpose;
		entity.UpdatedAt = DateTime.UtcNow;
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
			UpdatedAt = entity.UpdatedAt
		};
	}
}
